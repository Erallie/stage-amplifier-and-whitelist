#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;

namespace Erallie.StageAmplifierInspectors
{
    [CustomEditor(typeof(global::TXLWhitelistBridge))]
    [CanEditMultipleObjects]
    public class TXLWhitelistBridgeEditor : ErallieInspector
    {
        protected override string Description =>
#if ERALLIE_TXL_PACKAGE
            "Adds TXL access permissions to your whitelist. Connect the TXL Access Control and Whitelist components below.";
#else
            "Does nothing without AccessTXL / CommonTXL installed. Install AccessTXL to enable this whitelist bridge.";
#endif
    }
}
#endif
