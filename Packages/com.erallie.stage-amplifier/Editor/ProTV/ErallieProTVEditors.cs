#if UNITY_EDITOR && !COMPILER_UDONSHARP && ERALLIE_PROTV_PACKAGE
using UnityEditor;

namespace Erallie.StageAmplifierInspectors
{
    [CustomEditor(typeof(global::ProTVWhitelistBridge))]
    [CanEditMultipleObjects]
    public class ProTVWhitelistBridgeEditor : ErallieInspector
    {
        protected override string Description =>
            "Adds ProTV authorized users and superusers to your whitelist, and refreshes controls when ProTV permissions change.";
    }

    [CustomEditor(typeof(global::VolumeSyncToggle))]
    [CanEditMultipleObjects]
    public class VolumeSyncToggleEditor : ErallieInspector
    {
        protected override string Description =>
            "Lets whitelisted users enable or disable ProTV's shared volume control.";
    }
}
#endif
