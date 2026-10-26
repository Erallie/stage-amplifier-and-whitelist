#if ERALLIE_TXL_PACKAGE
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using Texel;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class TXLWhitelistBridge : UdonSharpBehaviour
{
    [Tooltip("TXL Access Control whose full access rules grant whitelist access.")]
    public AccessControl txlAccessControl;

    [Tooltip("The Whitelist component to refresh when access permissions change.")]
    public Whitelist updateListener;

    [HideInInspector] public VRCPlayerApi accessPlayer;
    [HideInInspector] public bool hasAccess;

    private void Start()
    {
        if (txlAccessControl != null)
        {
            txlAccessControl._EnsureInit();
            txlAccessControl._Register(
                AccessControl.EVENT_VALIDATE, this, nameof(AccessUpdated));
        }
        AccessUpdated();
    }

    public void CheckAccess()
    {
        hasAccess = false;
        if (txlAccessControl != null && Utilities.IsValid(accessPlayer))
        {
            txlAccessControl._EnsureInit();
            hasAccess = txlAccessControl._HasAccess(accessPlayer);
        }
    }

    public void AccessUpdated()
    {
        if (updateListener != null)
        {
            updateListener.UpdateUI();
        }
    }
}
#endif
