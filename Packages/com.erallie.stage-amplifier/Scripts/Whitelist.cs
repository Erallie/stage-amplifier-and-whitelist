using UdonSharp;
using UnityEngine;
using VRC.SDKBase;


public class Whitelist : UdonSharpBehaviour
{
    [Header("TXL Access Control")]
    [Tooltip("Optional Erallie TXL Whitelist Bridge. TXL permissions are added to the existing whitelist. Use Find to locate the bridge in this scene.")]
    public UdonSharpBehaviour txlAccessBridge;

    [Header("ProTV Whitelist")]
    [Tooltip("Optional Erallie ProTV Whitelist Bridge. Its permissions are added to this whitelist.")]
    public UdonSharpBehaviour proTVAccessBridge;

    [Header("Additional Whitelist")]
    [Tooltip("Additional VRChat display names to whitelist alongside optional ProTV and TXL permissions.")]
    public string[] whitelist;

    [Tooltip("When enabled, the first instance master will also be included in the whitelist.")]
    public bool includeFirstInstanceMaster;

    [UdonSynced]
    private string owner;

    [Header("Listeners")]
    [Tooltip("Components to refresh whenever whitelist permissions change. Use the button below to collect supported components in this scene.")]
    public UdonSharpBehaviour[] listeners;

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
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
    
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (string.IsNullOrEmpty(owner) && Networking.IsOwner(gameObject))
        {
            owner = player.displayName;
            UpdateUI();
        }
    }

    public bool IsOwner(VRCPlayerApi player)
    {
        return player.displayName == owner;
    }

    public bool IsOnWhitelist(VRCPlayerApi player)
    {
        if (!Utilities.IsValid(player))
        {
            return false;
        }

        if (proTVAccessBridge != null)
        {
            proTVAccessBridge.SetProgramVariable("accessPlayer", player);
            proTVAccessBridge.SendCustomEvent("CheckAccess");
            object proTVResult = proTVAccessBridge.GetProgramVariable("hasAccess");
            if (proTVResult != null && (bool)proTVResult)
            {
                return true;
            }
        }

        if (txlAccessBridge != null)
        {
            txlAccessBridge.SetProgramVariable("accessPlayer", player);
            txlAccessBridge.SendCustomEvent("CheckAccess");
            object accessResult = txlAccessBridge.GetProgramVariable("hasAccess");
            if (accessResult != null && (bool)accessResult)
            {
                return true;
            }
        }

        if (whitelist != null)
        {
            string playerName = player.displayName;

            foreach (string listedName in whitelist)
            {
                if (playerName == listedName)
                {
                    return true;
                }
            }
        }

        if (includeFirstInstanceMaster)
        {
            return IsOwner(player);
        }

        return false;
    }
}