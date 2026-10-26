#if UNITY_EDITOR && !COMPILER_UDONSHARP && ERALLIE_TXL_PACKAGE
using UnityEditor;

namespace Erallie.StageAmplifierInspectors
{
    [CustomEditor(typeof(global::TXLWhitelistBridge))]
    [CanEditMultipleObjects]
    public class TXLWhitelistBridgeEditor : ErallieInspector
    {
        protected override string Description =>
            "Adds TXL access permissions to your whitelist. Assign this bridge to Whitelist, and connect the existing Whitelist Update Listener below. Volume synchronization remains ProTV-only.";
    }
}
#endif
