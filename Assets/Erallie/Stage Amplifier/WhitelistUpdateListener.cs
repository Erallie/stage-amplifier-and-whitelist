using ArchiTech.ProTV;
using ArchiTech.SDK;
using UdonSharp;
using UnityEngine;

public class WhitelistUpdateListener : TVPluginUI
{
    [Header("Architect TV Whitelist")]
    [Tooltip("The TVManagedWhitelist component whose whitelist changes we want to listen for.")]
    public TVManagedWhitelist tvWhitelist;

    [Header("Listeners")]
    [Tooltip("UdonSharp behaviours that should receive WhitelistUpdated whenever the whitelist changes.")]
    public UdonSharpBehaviour[] listeners;


    public override void Start()
    {
        if (init)
        {
            return;
        }

        base.Start();

        if (tvWhitelist != null)
        {
            tvWhitelist._RegisterListener(this);
        }

        RefreshListeners();
    }


    public override void UpdateUI()
    {
        RefreshListeners();
    }


    private void RefreshListeners()
    {
        if (listeners == null)
        {
            return;
        }

        foreach (UdonSharpBehaviour listener in listeners)
        {
            if (listener != null)
            {
                listener.SendCustomEvent("WhitelistUpdated");
            }
        }
    }
}