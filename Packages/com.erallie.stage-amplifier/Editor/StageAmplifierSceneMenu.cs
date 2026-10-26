#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Erallie.StageAmplifierInspectors
{
    public static class StageAmplifierSceneMenu
    {
        private const string MenuRoot =
            "Tools/Erallie/Stage Amplifier and Whitelist/";
            
        private const string AddToSceneRoot =
            MenuRoot + "Add to Scene/";

        private const string OpenSceneRoot = 
            MenuRoot + "Open Example Scene/";

        private const string PrefabRoot =
            "Packages/com.erallie.stage-amplifier/Examples/Example Prefabs/";

        [MenuItem(AddToSceneRoot + "Add Whitelist Script to Scene", false, 0)]
        private static void AddWhitelist()
        {
            AddPrefab(PrefabRoot + "Whitelist.prefab");
        }

#if ERALLIE_ACCESSTXL_PACKAGE
        [MenuItem(AddToSceneRoot + "Add AccessTXL Whitelist Bridge to Scene", false, 1)]
        private static void AddTXLBridge()
        {
            AddPrefab(PrefabRoot + "Integrations/AccessTXL Whitelist Bridge.prefab");
        }
#endif

#if ERALLIE_PROTV_PACKAGE
        [MenuItem(AddToSceneRoot + "Add ProTV Whitelist Bridge to Scene", false, 2)]
        private static void AddProTVBridge()
        {
            AddPrefab(PrefabRoot + "Integrations/ProTV Whitelist Bridge.prefab");
        }
#endif

        [MenuItem(AddToSceneRoot + "Add Toggle When Whitelisted Script to Scene", false, 20)]
        private static void AddToggleWhenWhitelisted()
        {
            AddPrefab(PrefabRoot + "Toggle When Whitelisted.prefab");
        }

        [MenuItem(AddToSceneRoot + "Add Global Amplifier Toggle to Scene", false, 21)]
        private static void AddGlobalAmplifier()
        {
            AddPrefab(PrefabRoot + "Toggles/Global Amplifier.prefab");
        }

#if ERALLIE_PROTV_PACKAGE
        [MenuItem(AddToSceneRoot + "Add Volume Sync Toggle to Scene", false, 22)]
        private static void AddVolumeSync()
        {
            AddPrefab(PrefabRoot + "Toggles/Volume Sync Toggle.prefab");
        }
#endif

        [MenuItem(AddToSceneRoot + "Add Whitelist Toggle to Scene", false, 23)]
        private static void AddWhitelistToggle()
        {
            AddPrefab(PrefabRoot + "Toggles/Whitelist Toggle.prefab");
        }

        [MenuItem(AddToSceneRoot + "/Examples/Add Microphone Example Prefab to Scene", false, 24)]
        private static void AddMicrophone()
        {
            AddPrefab(PrefabRoot + "Microphone.prefab");
        }

        [MenuItem(AddToSceneRoot + "/Examples/Add Stage Amplifier Example Prefab to Scene", false, 25)]
        private static void AddStageAmplifier()
        {
            AddPrefab(PrefabRoot + "Stage Amplifier.prefab");
        }

        [MenuItem(AddToSceneRoot + "/Examples/Toggles/Add Microphone Toggle Example to Scene", false, 26)]
        private static void AddMicrophoneToggle()
        {
            AddPrefab(PrefabRoot + "/Toggles/Whitelisted Toggle Examples/Microphone Toggle.prefab");
        }

        [MenuItem(AddToSceneRoot + "/Examples/Toggles/Add Stage Toggle Example to Scene", false, 27)]
        private static void AddStageToggle()
        {
            AddPrefab(PrefabRoot + "/Toggles/Whitelisted Toggle Examples/Stage Toggle.prefab");
        }

        private static void AddPrefab(string prefabPath)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog(
                    "Add Prefab",
                    "Exit Play Mode before adding a prefab.",
                    "OK");
                return;
            }

            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                EditorUtility.DisplayDialog(
                    "Prefab Not Found at",
                    "The following prefab was not found:\n" + prefabPath + "\n\nPlease reinstall the Stage Amplifier and Whitelist package to restore missing files.",
                    "OK");
                return;
            }

            Scene scene = SceneManager.GetActiveScene();

            if (!scene.IsValid() || !scene.isLoaded)
            {
                EditorUtility.DisplayDialog(
                    "Add Prefab",
                    "Open a scene before adding a prefab.",
                    "OK");
                return;
            }

            GameObject instance =
                PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;

            if (instance != null)
            {
                Undo.RegisterCreatedObjectUndo(
                    instance,
                    "Add " + prefab.name);

                Selection.activeGameObject = instance;
                EditorGUIUtility.PingObject(instance);
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }
    }
}
#endif