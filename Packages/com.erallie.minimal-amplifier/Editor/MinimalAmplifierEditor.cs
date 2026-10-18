#if UNITY_EDITOR && !COMPILER_UDONSHARP
using System;
using System.Reflection;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;

namespace Erallie.MinimalAmplifierInspectors
{
    [CustomEditor(typeof(global::MinimalAmplifier))]
    [CanEditMultipleObjects]
    public class MinimalAmplifierEditor : Editor
    {
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
            EditorGUILayout.LabelField("Minimal Amplifier", titleStyle);
            EditorGUILayout.Space(4);

            EditorGUILayout.LabelField(
                "Amplifies players while they are inside this object's trigger.",
                descriptionStyle);

            EditorGUILayout.Space(4);
            
            var packageInfo =
                UnityEditor.PackageManager.PackageInfo.FindForAssembly(
                    target.GetType().Assembly);

            if (packageInfo != null)
            {
                EditorGUILayout.LabelField(
                    "Version " + packageInfo.version,
                    EditorStyles.miniLabel);
            }

            if (EditorGUILayout.LinkButton(
                "View on GitHub",
                GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL(
                    "https://github.com/Erallie/stage-amplifier-and-whitelist");
            }

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

                if (property.propertyPath == "limitDistance")
                {
                    SerializedProperty limitRange =
                        serializedObject.FindProperty("limitAmplificationRange");

                    if (limitRange != null &&
                        !limitRange.hasMultipleDifferentValues &&
                        !limitRange.boolValue)
                    {
                        continue;
                    }
                }

                EditorGUILayout.PropertyField(property, true);

                string tooltip = GetTooltip(target.GetType(), property.name);

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
                    BindingFlags.DeclaredOnly);

                if (field != null)
                {
                    var tooltip = Attribute.GetCustomAttribute(
                        field,
                        typeof(TooltipAttribute)) as TooltipAttribute;

                    return tooltip != null ? tooltip.tooltip : null;
                }

                type = type.BaseType;
            }

            return null;
        }
    }
}
#endif