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

        
        private const string ExamplesRoot = "Packages/com.erallie.stage-amplifier/Examples/";

        private const string SceneRoot = ExamplesRoot + "Example Scenes/";

        private const string PrefabRoot =
            ExamplesRoot + "Example Prefabs/";

        [MenuItem(AddToSceneRoot + "Whitelist Script", false, 0)]
        private static void AddWhitelist()
        {
            AddPrefab(PrefabRoot + "Whitelist.prefab");
        }

#if ERALLIE_ACCESSTXL_PACKAGE
        [MenuItem(AddToSceneRoot + "AccessTXL Whitelist Bridge", false, 1)]
        private static void AddTXLBridge()
        {
            AddPrefab(PrefabRoot + "Integrations/AccessTXL Whitelist Bridge.prefab");
        }
#endif

#if ERALLIE_PROTV_PACKAGE
        [MenuItem(AddToSceneRoot + "ProTV Whitelist Bridge", false, 2)]
        private static void AddProTVBridge()
        {
            AddPrefab(PrefabRoot + "Integrations/ProTV Whitelist Bridge.prefab");
        }
#endif

        [MenuItem(AddToSceneRoot + "Toggle When Whitelisted Script", false, 20)]
        private static void AddToggleWhenWhitelisted()
        {
            AddPrefab(PrefabRoot + "Toggle When Whitelisted.prefab");
        }

        [MenuItem(AddToSceneRoot + "Global Amplifier Toggle", false, 21)]
        private static void AddGlobalAmplifier()
        {
            AddPrefab(PrefabRoot + "Toggles/Global Amplifier.prefab");
        }

#if ERALLIE_PROTV_PACKAGE
        [MenuItem(AddToSceneRoot + "Volume Sync Toggle", false, 22)]
        private static void AddVolumeSync()
        {
            AddPrefab(PrefabRoot + "Toggles/Volume Sync Toggle.prefab");
        }
#endif

        [MenuItem(AddToSceneRoot + "Whitelist Toggle", false, 23)]
        private static void AddWhitelistToggle()
        {
            AddPrefab(PrefabRoot + "Toggles/Whitelist Toggle.prefab");
        }

        [MenuItem(AddToSceneRoot + "Staff Tag Toggle", false, 24)]
        private static void AddStaffTagToggle()
        {
            AddPrefab(PrefabRoot + "Toggles/Staff Tag Toggle.prefab");
        }

        [MenuItem(AddToSceneRoot + "/Microphone Example Prefab", false, 50)]
        private static void AddMicrophone()
        {
            AddPrefab(PrefabRoot + "Microphone.prefab");
        }

        [MenuItem(AddToSceneRoot + "/Stage Amplifier Example Prefab", false, 51)]
        private static void AddStageAmplifier()
        {
            AddPrefab(PrefabRoot + "Stage Amplifier.prefab");
        }

        [MenuItem(AddToSceneRoot + "/Whitelisted Toggle Examples/Microphone Toggle Example", false, 52)]
        private static void AddMicrophoneToggle()
        {
            AddPrefab(PrefabRoot + "/Toggles/Whitelisted Toggle Examples/Microphone Toggle.prefab");
        }

        [MenuItem(AddToSceneRoot + "/Whitelisted Toggle Examples/Stage Toggle Example", false, 53)]
        private static void AddStageToggle()
        {
            AddPrefab(PrefabRoot + "/Toggles/Whitelisted Toggle Examples/Stage Toggle.prefab");
        }

#if !ERALLIE_PROTV_PACKAGE && !ERALLIE_ACCESSTXL_PACKAGE
        [MenuItem(OpenSceneRoot + "Example Scene", false, 50)]
#else
        [MenuItem(OpenSceneRoot + "Example Scene without Integrations", false, 50)]
#endif
        private static void OpenNormalScene()
        {
            OpenExampleScene(
                SceneRoot + "Example Scene.unity");
        }

#if ERALLIE_PROTV_PACKAGE
        [MenuItem(OpenSceneRoot + "Example Scene with ProTV", false, 51)]
        private static void OpenProTVScene()
        {
            OpenExampleScene(
                SceneRoot + "Example Scene With ProTV.unity");
        }
#endif

#if ERALLIE_ACCESSTXL_PACKAGE
        [MenuItem(OpenSceneRoot + "Example Scene with AccessTXL", false, 52)]
        private static void OpenTXLScene()
        {
            OpenExampleScene(
                SceneRoot + "Example Scene With AccessTXL.unity");
        }
#endif

        private static bool CanUseSceneMenu()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        [MenuItem(AddToSceneRoot + "Add Whitelist Script to Scene", true)]
        [MenuItem(AddToSceneRoot + "Add Toggle When Whitelisted Script to Scene", true)]
        [MenuItem(AddToSceneRoot + "Add Global Amplifier Toggle to Scene", true)]
        [MenuItem(AddToSceneRoot + "Add Whitelist Toggle to Scene", true)]
        [MenuItem(AddToSceneRoot + "/Microphone Example Prefab", true)]
        [MenuItem(AddToSceneRoot + "/Stage Amplifier Example Prefab", true)]
        [MenuItem(AddToSceneRoot + "/Whitelisted Toggle Examples/Add Microphone Toggle Example to Scene", true)]
        [MenuItem(AddToSceneRoot + "/Whitelisted Toggle Examples/Add Stage Toggle Example to Scene", true)]
        private static bool ValidateCommonMenuItems()
        {
            return CanUseSceneMenu();
        }

#if ERALLIE_PROTV_PACKAGE
        [MenuItem(AddToSceneRoot + "Add ProTV Whitelist Bridge to Scene", true)]
        [MenuItem(AddToSceneRoot + "Add Volume Sync Toggle to Scene", true)]
        [MenuItem(OpenSceneRoot + "Open Example Scene with ProTV", true)]
        private static bool ValidateProTVMenuItems()
        {
            return CanUseSceneMenu();
        }
#endif

#if ERALLIE_ACCESSTXL_PACKAGE
        [MenuItem(AddToSceneRoot + "Add AccessTXL Whitelist Bridge to Scene", true)]
        [MenuItem(OpenSceneRoot + "Open Example Scene with AccessTXL", true)]
        private static bool ValidateTXLMenuItems()
        {
            return CanUseSceneMenu();
        }
#endif

#if !ERALLIE_PROTV_PACKAGE && !ERALLIE_ACCESSTXL_PACKAGE
        [MenuItem(OpenSceneRoot + "Open Example Scene", true)]
#else
        [MenuItem(OpenSceneRoot + "Open Example Scene without Integrations", true)]
#endif
        private static bool ValidateNormalSceneMenuItem()
        {
            return CanUseSceneMenu();
        }

        private static void OpenExampleScene(string scenePath)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) == null)
            {
                EditorUtility.DisplayDialog(
                    "Scene Not Found",
                    "No scene was found at:\n" + scenePath,
                    "OK");
                return;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }
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