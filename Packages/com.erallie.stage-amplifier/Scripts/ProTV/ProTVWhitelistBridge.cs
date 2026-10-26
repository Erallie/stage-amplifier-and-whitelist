#if ERALLIE_PROTV_PACKAGE
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using ArchiTech.ProTV;

public class ProTVWhitelistBridge : TVPluginUI
{
    [Tooltip("ProTV whitelist whose authorized users and superusers should be whitelisted.")]
    public TVManagedWhitelist tvWhitelist;
    [Tooltip("The Whitelist component to refresh when access permissions change.")]
    public Whitelist updateListener;
    [HideInInspector] public VRCPlayerApi accessPlayer;
    [HideInInspector] public bool hasAccess;

    public override void Start()
    {
        if (!init)
        {
            base.Start();
            if (tvWhitelist != null)
            {
                tvWhitelist._RegisterListener(this);
            }
            UpdateUI();
        }
    }

    public void CheckAccess()
    {
        hasAccess = false;
        if (tvWhitelist != null && Utilities.IsValid(accessPlayer))
        {
            hasAccess = tvWhitelist._IsAuthorizedUser(accessPlayer) ||
                tvWhitelist._IsSuperUser(accessPlayer);
        }
    }

    public override void UpdateUI()
    {
        if (updateListener != null)
        {
            updateListener.UpdateUI();
        }
    }
}
#else
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ProTVWhitelistBridge : UdonSharpBehaviour
{
    [Tooltip("The Whitelist component to refresh when access permissions change.")]
    public Whitelist updateListener;
    [HideInInspector] public VRCPlayerApi accessPlayer;
    [HideInInspector] public bool hasAccess;

    public void CheckAccess()
    {
        hasAccess = false;
    }

    public void AccessUpdated()
    {
        if (updateListener != null)
        {
            updateListener.UpdateUI();
        }
    }

    public void UpdateUI()
    {
        AccessUpdated();
    }
}
#endif
