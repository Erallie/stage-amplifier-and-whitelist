#if ERALLIE_PROTV_PACKAGE
using ArchiTech.ProTV;
#endif

using UdonSharp;
using UnityEngine;

#if ERALLIE_PROTV_PACKAGE
public class WhitelistUpdateListener : TVPluginUI
#else
public class WhitelistUpdateListener : UdonSharpBehaviour
#endif
{
#if ERALLIE_PROTV_PACKAGE
    [Header("Architect TV Whitelist")]
    [Tooltip("The TVManagedWhitelist component whose whitelist changes we want to listen for.")]
    public TVManagedWhitelist tvWhitelist;
#endif

    [Header("Listeners")]
    [Tooltip("UdonSharp behaviours that should receive WhitelistUpdated whenever the whitelist changes.")]
    public UdonSharpBehaviour[] listeners;

#if ERALLIE_PROTV_PACKAGE
    public override void Start()
#else
    private void Start()
#endif
    {
#if ERALLIE_PROTV_PACKAGE
        if (init)
        {
            return;
        }

        base.Start();

        if (tvWhitelist != null)
        {
            tvWhitelist._RegisterListener(this);
        }
#endif

        RefreshListeners();
    }

#if ERALLIE_PROTV_PACKAGE
    public override void UpdateUI()
#else
    public void UpdateUI()
#endif
    {
        RefreshListeners();
    }

    private void RefreshListeners()
    {
        if (listeners != null)
        {
            foreach (UdonSharpBehaviour listener in listeners)
            {
                if (listener != null)
                {
                    listener.SendCustomEvent("WhitelistUpdated");
                }
            }
        }
    }
}