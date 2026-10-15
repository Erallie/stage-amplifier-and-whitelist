using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using ArchiTech.ProTV;

public class Whitelist : UdonSharpBehaviour
{
    [Header("Architect TV Whitelist")]
    [Tooltip("The TVManagedWhitelist component used by Architect's TV.")]
    public TVManagedWhitelist tvWhitelist;

    [Header("Additional Whitelist")]
    [Tooltip("Additional VRChat display names to whitelist on top of Architect's TV whitelist.")]
    public string[] whitelist;

    [Tooltip("When enabled, the first instance master will also be included in the whitelist.")]
    public bool includeFirstInstanceMaster;

    [UdonSynced]
    private string owner;


    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (string.IsNullOrEmpty(owner) && Networking.IsOwner(gameObject))
        {
            owner = player.displayName;
        }
    }

    public bool IsOwner(VRCPlayerApi player) {
        return player.displayName == owner;
    }

    public bool IsOnWhitelist(VRCPlayerApi player)
    {
        if (!Utilities.IsValid(player))
        {
            return false;
        }

        // Architect's authorized users and explicit super users.
        if (Utilities.IsValid(tvWhitelist))
        {
            if (tvWhitelist._IsAuthorizedUser(player) ||
                tvWhitelist._IsSuperUser(player))
            {
                return true;
            }
        }

        // Our additional whitelist.
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

        // Optionally authorize the first instance master.
        if (includeFirstInstanceMaster)
        {
            return IsOwner(player);
        }

        return false;
    }
}