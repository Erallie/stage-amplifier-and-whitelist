#if UNITY_EDITOR && !COMPILER_UDONSHARP
using System;
using System.Reflection;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;

namespace Erallie.StageAmplifierInspectors
{
    public abstract class ErallieInspector : Editor
    {
        protected abstract string Description { get; }

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

                EditorGUILayout.PropertyField(property, true);

                string tooltip = GetTooltip(
                    target.GetType(),
                    property.name
                );

                if (!string.IsNullOrWhiteSpace(tooltip))
                {
                    EditorGUILayout.HelpBox(tooltip, MessageType.Info);
                    EditorGUILayout.Space(6);
                }
            }

            serializedObject.ApplyModifiedProperties();
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
            "Lets a whitelisted user interact to amplify their voice throughout the world. Interact again to stop. Can hide the control from users who are not whitelisted.";
    }

    [CustomEditor(typeof(global::Microphone))]
    [CanEditMultipleObjects]
    public class MicrophoneEditor : ErallieInspector
    {
        protected override string Description =>
            "Amplifies the voice of the player holding this pickup microphone and restores their normal voice settings when it is dropped. Requires a VRC Pickup component.";
    }

    [CustomEditor(typeof(global::MinimalAmplifier))]
    [CanEditMultipleObjects]
    public class MinimalAmplifierEditor : ErallieInspector
    {
        protected override string Description =>
            "Amplifies players while they are inside this object's trigger. Restores normal voice settings when they leave. Requires a trigger collider.";
    }

    [CustomEditor(typeof(global::StageAmplifier))]
    [CanEditMultipleObjects]
    public class StageAmplifierEditor : ErallieInspector
    {
        protected override string Description =>
            "Amplifies performers inside the stage trigger. Whitelisted users can share the stage; otherwise, the first performer reserves it. Optional barrier and teleport references control access. Requires a trigger collider.";
    }

    [CustomEditor(typeof(global::StageWhitelistToggle))]
    [CanEditMultipleObjects]
    public class StageWhitelistToggleEditor : ErallieInspector
    {
        protected override string Description =>
            "Lets whitelisted users switch a linked Stage Amplifier between normal access and whitelist-only access. Updates the interaction text to match the current mode.";
    }

    [CustomEditor(typeof(global::ToggleOthersOnActiveSelf))]
    [CanEditMultipleObjects]
    public class ToggleOthersOnActiveSelfEditor : ErallieInspector
    {
        protected override string Description =>
            "Enables the listed objects when this component is enabled and disables them when it is disabled. The changes are applied locally on each client.";
    }

    [CustomEditor(typeof(global::ToggleWhenWhitelisted))]
    [CanEditMultipleObjects]
    public class ToggleWhenWhitelistedEditor : ErallieInspector
    {
        protected override string Description =>
            "Shows or hides the listed objects locally according to whether the current player is whitelisted. Toggle On selects which state whitelisted players receive.";
    }

    [CustomEditor(typeof(global::VolumeSyncToggle))]
    [CanEditMultipleObjects]
    public class VolumeSyncToggleEditor : ErallieInspector
    {
        protected override string Description =>
            "Lets whitelisted users enable or disable ProTV's shared volume control. Requires ProTV and a linked TV Manager; interaction does nothing when ProTV is not installed.";
    }

    [CustomEditor(typeof(global::Whitelist))]
    [CanEditMultipleObjects]
    public class WhitelistEditor : ErallieInspector
    {
        protected override string Description =>
            "Defines authorized players by their VRChat display names. Can also include the first instance master and, when ProTV is installed and linked, ProTV's authorized users and super users.";
    }

    [CustomEditor(typeof(global::WhitelistedToggle))]
    [CanEditMultipleObjects]
    public class WhitelistedToggleEditor : ErallieInspector
    {
        protected override string Description =>
            "Switches objects on or off for everyone using a whitelist-controlled interaction or optional UI toggle. Supports custom interaction text, hidden canvases, optional ownership transfer, and respawning objects when switched off.";
    }

    [CustomEditor(typeof(global::WhitelistUpdateListener))]
    [CanEditMultipleObjects]
    public class WhitelistUpdateListenerEditor : ErallieInspector
    {
        protected override string Description =>
            "Sends WhitelistUpdated to the listed UdonSharp behaviours at startup. When ProTV is installed and linked, also forwards its whitelist updates so the linked controls can refresh.";
    }
}
#endif