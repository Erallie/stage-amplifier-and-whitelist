#if UNITY_EDITOR && !COMPILER_UDONSHARP
using System;
using System.Reflection;
using System.Collections.Generic;
using UdonSharp;
using UnityEditor.SceneManagement;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;

namespace Erallie.StageAmplifierInspectors
{
    public abstract class ErallieInspector : Editor
    {
        protected abstract string Description { get; }
        private readonly Dictionary<string, string> referenceSearchMessages = new Dictionary<string, string>();

        public override void OnInspectorGUI()
        {
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                wordWrap = true
            };

            var descriptionStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };

            EditorGUILayout.Space(8);

            EditorGUILayout.LabelField(
                ObjectNames.NicifyVariableName(target.GetType().Name),
                titleStyle
            );

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(Description, descriptionStyle);
            EditorGUILayout.Space(12);

            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(targets))
            {
                return;
            }

            serializedObject.Update();

            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (property.propertyPath == "m_Script")
                {
                    continue;
                }

                if (target is global::WhitelistedToggle &&
                    property.propertyPath == "childObjects")
                {
                    SerializedProperty transfer = serializedObject.FindProperty("transferOwnership");
                    if (transfer != null && !transfer.hasMultipleDifferentValues && !transfer.boolValue)
                    {
                        continue;
                    }
                }
                if (target is global::WhitelistedToggle &&
                    (property.propertyPath == "interactTextWhenOn" ||
                     property.propertyPath == "interactTextWhenOff"))
                {
                    SerializedProperty customize =
                        serializedObject.FindProperty("customizeInteractionText");

                    if (customize != null &&
                        !customize.hasMultipleDifferentValues &&
                        !customize.boolValue)
                    {
                        continue;
                    }
                }
                Type referenceType = GetReferenceSearchType(property);
                bool isFindableReference = referenceType != null;

                if (isFindableReference)
                {
                    DrawSceneReference(property, referenceType);
                }
                else
                {
                    EditorGUILayout.PropertyField(property, true);
                }

                string tooltip = GetTooltip(
                    target.GetType(),
                    property.name
                );

                if (!string.IsNullOrWhiteSpace(tooltip))
                {
                    EditorGUILayout.HelpBox(tooltip, MessageType.Info);
                    EditorGUILayout.Space(6);
                }

                if (isFindableReference &&
                    referenceSearchMessages.TryGetValue(property.propertyPath, out string searchMessage))
                {
                    EditorGUILayout.HelpBox(searchMessage, MessageType.Info);
                }

                DrawAfterProperty(property);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private Type GetReferenceSearchType(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference ||
                (property.propertyPath != "whitelist" &&
                 property.propertyPath != "tvWhitelist" &&
                 property.propertyPath != "tvManager" &&
                 property.propertyPath != "updateListener"))
            {
                return null;
            }

            // Resolve the field's existing compiled type. This Editor assembly
            // never needs to reference a ProTV class directly.
            Type ownerType = target.GetType();
            while (ownerType != null)
            {
                FieldInfo field = ownerType.GetField(property.name,
                    BindingFlags.Instance | BindingFlags.Public |
                    BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (field != null)
                {
                    return typeof(Component).IsAssignableFrom(field.FieldType)
                        ? field.FieldType
                        : null;
                }

                ownerType = ownerType.BaseType;
            }

            return null;
        }

        private void DrawSceneReference(SerializedProperty property, Type referenceType)
        {
            // Reserve the property's full height so Header attributes remain intact.
            float height = EditorGUI.GetPropertyHeight(property, true);
            Rect row = EditorGUILayout.GetControlRect(true, height);
            Rect field = row;
            field.width -= 54f;
            EditorGUI.PropertyField(field, property, true);

            Rect button = new Rect(row.xMax - 48f,
                row.yMax - EditorGUIUtility.singleLineHeight,
                48f, EditorGUIUtility.singleLineHeight);

            bool canSearch = CanFindSceneReference();
            using (new EditorGUI.DisabledScope(!canSearch))
            {
                if (GUI.Button(button, new GUIContent("Find",
                    "Find a " + ObjectNames.NicifyVariableName(referenceType.Name) + " in this scene, including inactive objects. " +
                    "Multiple matches open a selection menu. Select components in the same scene and exit Play Mode.")))
                {
                    serializedObject.ApplyModifiedProperties();
                    FindSceneReference(property.propertyPath, referenceType);
                    serializedObject.Update();
                }
            }
        }

        private bool CanFindSceneReference()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return false;
            }

            var first = target as Component;
            if (first == null)
            {
                return false;
            }

            foreach (UnityEngine.Object selected in targets)
            {
                var component = selected as Component;
                if (component == null || EditorUtility.IsPersistent(component) ||
                    !component.gameObject.scene.IsValid() ||
                    !component.gameObject.scene.isLoaded ||
                    component.gameObject.scene != first.gameObject.scene)
                {
                    return false;
                }
            }

            return true;
        }

        private void FindSceneReference(string propertyPath, Type referenceType)
        {
            var component = target as Component;
            var found = new List<Component>();
            foreach (GameObject root in component.gameObject.scene.GetRootGameObjects())
            {
                found.AddRange(root.GetComponentsInChildren(referenceType, true));
            }

            // Capture the owners so a menu callback never uses an Inspector iterator.
            UnityEngine.Object[] owners = (UnityEngine.Object[])targets.Clone();
            if (found.Count == 0)
            {
                referenceSearchMessages[propertyPath] = "No " + ObjectNames.NicifyVariableName(referenceType.Name) + " component was found in this scene. The current reference was kept.";
            }
            else if (found.Count == 1)
            {
                AssignSceneReference(owners, propertyPath, found[0]);
                referenceSearchMessages.Remove(propertyPath);
            }
            else
            {
                referenceSearchMessages.Remove(propertyPath);
                var menu = new GenericMenu();
                for (int i = 0; i < found.Count; i++)
                {
                    Component candidate = found[i];
                    string label = (i + 1) + ". " + GetHierarchyPath(candidate.transform);
                    menu.AddItem(new GUIContent(label), false,
                        () => AssignSceneReference(owners, propertyPath, candidate));
                }

                menu.ShowAsContext();
            }
        }

        private static string GetHierarchyPath(Transform item)
        {
            string result = item.name;
            while (item.parent != null)
            {
                item = item.parent;
                result = item.name + " > " + result;
            }

            return result;
        }

        private static void AssignSceneReference(UnityEngine.Object[] owners,
            string propertyPath, Component candidate)
        {
            if (candidate == null)
            {
                return;
            }

            Undo.SetCurrentGroupName("Find Scene Reference");
            foreach (UnityEngine.Object owner in owners)
            {
                var component = owner as Component;
                if (component == null || EditorUtility.IsPersistent(component) ||
                    component.gameObject.scene != candidate.gameObject.scene)
                {
                    continue;
                }

                var componentObject = new SerializedObject(owner);
                componentObject.Update();
                SerializedProperty reference = componentObject.FindProperty(propertyPath);
                if (reference != null &&
                    reference.propertyType == SerializedPropertyType.ObjectReference &&
                    reference.objectReferenceValue != candidate)
                {
                    reference.objectReferenceValue = candidate;
                    // Serialized properties handle Undo and prefab overrides.
                    if (componentObject.ApplyModifiedProperties())
                    {
                        EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
                    }
                }
            }
        }

        protected virtual void DrawAfterProperty(SerializedProperty property)
        {
        }

        private static string GetTooltip(Type type, string fieldName)
        {
            while (type != null)
            {
                FieldInfo field = type.GetField(
                    fieldName,
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly
                );

                if (field != null)
                {
                    var tooltip = Attribute.GetCustomAttribute(
                        field,
                        typeof(TooltipAttribute)
                    ) as TooltipAttribute;

                    return tooltip != null ? tooltip.tooltip : null;
                }

                type = type.BaseType;
            }

            return null;
        }
    }

    [CustomEditor(typeof(global::GlobalAmplifier))]
    [CanEditMultipleObjects]
    public class GlobalAmplifierEditor : ErallieInspector
    {
        protected override string Description =>
            "Lets a whitelisted user interact to amplify their voice throughout the world, no matter where they are.";
    }

    [CustomEditor(typeof(global::Microphone))]
    [CanEditMultipleObjects]
    public class MicrophoneEditor : ErallieInspector
    {
        protected override string Description =>
            "Amplifies the voice of the player when they are holding this pickup.";
    }

    [CustomEditor(typeof(global::MinimalAmplifier))]
    [CanEditMultipleObjects]
    public class MinimalAmplifierEditor : ErallieInspector
    {
        protected override string Description =>
            "Amplifies players while they are inside this object's trigger.";
    }

    [CustomEditor(typeof(global::StageAmplifier))]
    [CanEditMultipleObjects]
    public class StageAmplifierEditor : ErallieInspector
    {
        protected override string Description =>
            "Amplifies performers inside this object's trigger. By default, anyone can be onstage, but only one person at a time. Whitelisted players can always enter the stage regardless of whether it is occupied. To restrict stage access to only whitelisted people, add a Stage Whitelist Toggle to the scene.";
    }

    [CustomEditor(typeof(global::StageWhitelistToggle))]
    [CanEditMultipleObjects]
    public class StageWhitelistToggleEditor : ErallieInspector
    {
        protected override string Description =>
            "Lets whitelisted users switch a linked Stage Amplifier between normal access and whitelist-only access.";
    }

    [CustomEditor(typeof(global::ToggleOthersOnActiveSelf))]
    [CanEditMultipleObjects]
    public class ToggleOthersOnActiveSelfEditor : ErallieInspector
    {
        protected override string Description =>
            "Enables the listed objects locally when this component is enabled and disables them when it is disabled.";
    }

    [CustomEditor(typeof(global::ToggleWhenWhitelisted))]
    [CanEditMultipleObjects]
    public class ToggleWhenWhitelistedEditor : ErallieInspector
    {
        protected override string Description =>
            "Shows or hides the listed objects locally according to whether the current player is whitelisted. This script must NOT be in the same object as the Whitelist script.";
    }

    [CustomEditor(typeof(global::VolumeSyncToggle))]
    [CanEditMultipleObjects]
    public class VolumeSyncToggleEditor : ErallieInspector
    {
        protected override string Description =>
            "Lets whitelisted users enable or disable ProTV's shared volume control. This does nothing when ProTV is not installed.";
    }

    [CustomEditor(typeof(global::Whitelist))]
    [CanEditMultipleObjects]
    public class WhitelistEditor : ErallieInspector
    {
        private string assignmentResult;

        protected override string Description =>
            "Handles the whitelist for the Stage Amplifier system. Can be integrated with ProTV's whitelist system.";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var whitelist = target as global::Whitelist;

            bool canAssign =
                targets.Length == 1 &&
                whitelist != null &&
                !EditorApplication.isPlayingOrWillChangePlaymode &&
                !EditorUtility.IsPersistent(whitelist) &&
                whitelist.gameObject.scene.IsValid() &&
                whitelist.gameObject.scene.isLoaded;

            EditorGUILayout.Space(8);

            EditorGUILayout.HelpBox(
                "Assign this whitelist to every serialized Whitelist field in this scene, " +
                "including inactive objects. Existing assignments will be replaced. " +
                "Can be undone with Ctrl+ Z",
                MessageType.Info);

            using (new EditorGUI.DisabledScope(!canAssign))
            {
                if (GUILayout.Button("Assign This Whitelist Throughout Scene"))
                {
                    serializedObject.ApplyModifiedProperties();

                    int count = AssignThroughoutScene(whitelist);

                    serializedObject.Update();

                    assignmentResult = count == 0
                        ? "No references needed changing."
                        : "Assigned this whitelist to " + count + " reference(s).";
                }
            }

            if (targets.Length != 1)
            {
                EditorGUILayout.HelpBox(
                    "Select one Whitelist component to assign throughout its scene.",
                    MessageType.Info);
            }

            if (!string.IsNullOrEmpty(assignmentResult))
            {
                EditorGUILayout.HelpBox(assignmentResult, MessageType.Info);
            }
        }

        private static int AssignThroughoutScene(global::Whitelist whitelist)
        {
            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Assign Whitelist Throughout Scene");

            int count = 0;

            foreach (GameObject root in whitelist.gameObject.scene.GetRootGameObjects())
            {
                foreach (MonoBehaviour component in
                    root.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (component == null)
                    {
                        continue;
                    }

                    var componentObject = new SerializedObject(component);
                    componentObject.Update();

                    Type componentType = component.GetType();

                    while (componentType != null &&
                        componentType != typeof(MonoBehaviour))
                    {
                        FieldInfo[] fields = componentType.GetFields(
                            BindingFlags.Instance |
                            BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.DeclaredOnly);

                        foreach (FieldInfo field in fields)
                        {
                            if (field.FieldType != typeof(global::Whitelist))
                            {
                                continue;
                            }

                            SerializedProperty reference =
                                componentObject.FindProperty(field.Name);

                            if (reference != null &&
                                reference.propertyType ==
                                    SerializedPropertyType.ObjectReference &&
                                reference.objectReferenceValue != whitelist)
                            {
                                reference.objectReferenceValue = whitelist;
                                count++;
                            }
                        }

                        componentType = componentType.BaseType;
                    }

                    // Serialized properties support Undo and prefab overrides.
                    componentObject.ApplyModifiedProperties();
                }
            }

            if (count > 0)
            {
                EditorSceneManager.MarkSceneDirty(whitelist.gameObject.scene);
            }

            Undo.CollapseUndoOperations(undoGroup);

            return count;
        }
    }

    [CustomEditor(typeof(global::WhitelistedToggle))]
    [CanEditMultipleObjects]
    public class WhitelistedToggleEditor : ErallieInspector
    {
        protected override string Description =>
            "Whitelisted people can interact with this script to toggle objects on or off for everyone in the instance.";
    }

    [CustomEditor(typeof(global::WhitelistUpdateListener))]
    [CanEditMultipleObjects]
    public class WhitelistUpdateListenerEditor : ErallieInspector
    {
        private string findResult;

        protected override string Description =>
            "Handles updates to the whitelist. If ProTV is integrated with this system's whitelist, this component MUST be attached to an object in the world.";

        protected override void DrawAfterProperty(SerializedProperty property)
        {
            if (property.propertyPath != "listeners")
            {
                return;
            }

            bool canSearch = !EditorApplication.isPlayingOrWillChangePlaymode;
            foreach (UnityEngine.Object selected in targets)
            {
                var listener = selected as global::WhitelistUpdateListener;
                if (listener == null || EditorUtility.IsPersistent(listener) ||
                    !listener.gameObject.scene.IsValid() || !listener.gameObject.scene.isLoaded)
                {
                    canSearch = false;
                }
            }

            using (new EditorGUI.DisabledScope(!canSearch))
            {
                if (GUILayout.Button("Find and Add Whitelist Listeners"))
                {
                    // Commit pending field edits before updating each selected component.
                    serializedObject.ApplyModifiedProperties();
                    Undo.SetCurrentGroupName("Find Whitelist Listeners");
                    int totalAdded = 0;
                    foreach (UnityEngine.Object selected in targets)
                    {
                        var listener = selected as global::WhitelistUpdateListener;
                        if (listener != null)
                        {
                            totalAdded += AddSceneListeners(listener);
                        }
                    }

                    serializedObject.Update();
                    findResult = totalAdded == 0
                        ? "No new listeners found. Existing assignments were kept."
                        : "Added " + totalAdded + " listener reference(s).";
                }
            }

            if (!canSearch)
            {
                EditorGUILayout.HelpBox(
                    "Open a scene or Prefab Mode and exit Play Mode to find listeners.",
                    MessageType.Info);
            }

            if (!string.IsNullOrEmpty(findResult))
            {
                EditorGUILayout.HelpBox(findResult, MessageType.Info);
            }

            EditorGUILayout.Space(6);
        }

        private static int AddSceneListeners(global::WhitelistUpdateListener listener)
        {
            var found = new List<UdonSharpBehaviour>();

            // Explicit list of this package's supported listener types.
            // Search this scene only, including inactive GameObjects and disabled components.
            foreach (GameObject root in listener.gameObject.scene.GetRootGameObjects())
            {
                found.AddRange(root.GetComponentsInChildren<global::GlobalAmplifier>(true));
                found.AddRange(root.GetComponentsInChildren<global::StageWhitelistToggle>(true));
                found.AddRange(root.GetComponentsInChildren<global::ToggleWhenWhitelisted>(true));
                found.AddRange(root.GetComponentsInChildren<global::VolumeSyncToggle>(true));
                found.AddRange(root.GetComponentsInChildren<global::WhitelistedToggle>(true));
                found.AddRange(root.GetComponentsInChildren<global::StageAmplifier>(true));
            }

            var componentObject = new SerializedObject(listener);
            componentObject.Update();
            SerializedProperty list = componentObject.FindProperty("listeners");
            if (list == null || !list.isArray)
            {
                return 0;
            }

            var assigned = new HashSet<UnityEngine.Object>();
            for (int i = 0; i < list.arraySize; i++)
            {
                UnityEngine.Object existing = list.GetArrayElementAtIndex(i).objectReferenceValue;
                if (existing != null)
                {
                    assigned.Add(existing);
                }
            }

            int added = 0;
            foreach (UdonSharpBehaviour candidate in found)
            {
                if (candidate == null || !assigned.Add(candidate))
                {
                    continue;
                }

                int slot = -1;
                for (int i = 0; i < list.arraySize; i++)
                {
                    if (list.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    {
                        slot = i;
                        break;
                    }
                }

                if (slot < 0)
                {
                    slot = list.arraySize;
                    list.arraySize++;
                }

                list.GetArrayElementAtIndex(slot).objectReferenceValue = candidate;
                added++;
            }

            // Serialized properties provide Undo and prefab-override tracking.
            if (componentObject.ApplyModifiedProperties())
            {
                EditorSceneManager.MarkSceneDirty(listener.gameObject.scene);
            }

            return added;
        }
    }
}
#endif