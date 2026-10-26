#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;

namespace Erallie.StageAmplifierInspectors
{
    [CustomEditor(typeof(global::ProTVWhitelistBridge))]
    [CanEditMultipleObjects]
    public class ProTVWhitelistBridgeEditor : ErallieInspector
    {
        protected override string Description =>
#if ERALLIE_PROTV_PACKAGE
            "Adds ProTV authorized users and superusers to your whitelist, and refreshes controls when ProTV permissions change.";
#else
            "Does nothing without ProTV installed. Install ProTV to enable this whitelist bridge.";
#endif
    }

    [CustomEditor(typeof(global::VolumeSyncToggle))]
    [CanEditMultipleObjects]
    public class VolumeSyncToggleEditor : ErallieInspector
    {
        protected override string Description =>
#if ERALLIE_PROTV_PACKAGE
            "Lets whitelisted users enable or disable ProTV's shared volume control.";
#else
            "Does nothing without ProTV installed. Install ProTV to enable shared volume control.";
#endif
    }
}
#endif
