#if UNITY_EDITOR && !COMPILER_UDONSHARP
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Erallie.StageAmplifierInspectors
{
    public static class StageAmplifierAutoAssign
    {
        private const string MenuPath =
            "Tools/Erallie/Stage Amplifier and Whitelist/Assign All Scene References";

        [MenuItem(MenuPath, false, 100)]
        private static void AssignActiveScene()
        {
            Assign(SceneManager.GetActiveScene());
        }

        [MenuItem(MenuPath, true)]
        private static bool CanAssignActiveScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            return !EditorApplication.isPlayingOrWillChangePlaymode &&
                scene.IsValid() && scene.isLoaded;
        }

        public static void Assign(Scene scene)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode ||
                !scene.IsValid() || !scene.isLoaded)
            {
                return;
            }

            var components = new List<MonoBehaviour>();
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (MonoBehaviour component in root.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (component != null)
                    {
                        components.Add(component);
                    }
                }
            }

            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Assign All Stage Amplifier References");
            int assigned = 0;
            int listeners = 0;
            var unresolved = new List<string>();

            try
            {
                foreach (MonoBehaviour component in components)
                {
                    // Only components using our inspectors participate. TXL/ProTV
                    // types remain optional and use the same resolver as Find.
                    Editor editor = Editor.CreateEditor(component);
                    try
                    {
                        if (!(component is UdonSharp.UdonSharpBehaviour) ||
                            !component.GetType().Assembly.GetName().Name.StartsWith(
                                "Erallie.StageAmplifier", StringComparison.Ordinal))
                        {
                            continue;
                        }

                        var data = new SerializedObject(component);
                        data.Update();
                        SerializedProperty property = data.GetIterator();
                        bool enterChildren = true;
                        while (property.NextVisible(enterChildren))
                        {
                            enterChildren = false;
                            if (property.propertyType != SerializedPropertyType.ObjectReference ||
                                property.objectReferenceValue != null)
                            {
                                continue;
                            }

                            Type referenceType = ErallieInspector.ResolveReferenceSearchType(component.GetType(), property);
                            if (referenceType == null)
                            {
                                continue;
                            }

                            MonoBehaviour match = null;
                            int matches = 0;
                            foreach (MonoBehaviour candidate in components)
                            {
                                if (referenceType.IsInstanceOfType(candidate))
                                {
                                    match = candidate;
                                    matches++;
                                }
                            }

                            if (matches == 1)
                            {
                                property.objectReferenceValue = match;
                                assigned++;
                            }
                            else
                            {
                                unresolved.Add(component.name + " (" + component.GetType().Name +
                                    ")." + property.propertyPath + ": " + matches + " matches");
                            }
                        }
                        data.ApplyModifiedProperties();
                    }
                    finally
                    {
                        if (editor != null)
                        {
                            UnityEngine.Object.DestroyImmediate(editor);
                        }
                    }
                }

                // References have all been filled before collecting listeners.
                foreach (MonoBehaviour component in components)
                {
                    var whitelist = component as global::Whitelist;
                    if (whitelist != null)
                    {
                        listeners += WhitelistListenersEditor.AddSceneListeners(whitelist, true);
                    }
                }

                if (assigned > 0 || listeners > 0)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                }
            }
            finally
            {
                Undo.CollapseUndoOperations(group);
            }

            string summary = "Assigned " + assigned + " empty reference(s).\n" +
                "Added " + listeners + " whitelist listener(s).\n" +
                "Existing assignments were preserved.";
            if (unresolved.Count > 0)
            {
                summary += "\n\n" + unresolved.Count +
                    " reference(s) had zero or multiple matches. See Console for details.";
                Debug.Log("Stage Amplifier automatic assignment:\n" +
                    string.Join("\n", unresolved));
            }
            EditorUtility.DisplayDialog("Assign Scene References", summary, "OK");
        }
    }
}
#endif
