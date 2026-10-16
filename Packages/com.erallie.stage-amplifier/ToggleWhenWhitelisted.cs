
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleWhenWhitelisted : UdonSharpBehaviour
{
    [Tooltip("Set this to the game object that contains the Whitelist script.")]
    public Whitelist whitelist;
    // private VRCPlayerApi localPlayer;


    [Tooltip("Add the objects that you want to toggle on and off.")]
    public GameObject[] objectsToToggle;

    [Tooltip("Leave this unchecked to toggle the objects off when the user is whitelisted. Check this to toggle the objects on when whitelisted.")]
    public bool toggleOn;
    
    
    void OnEnable()
    {
        WhitelistUpdated();
    }

    public void WhitelistUpdated() {
        bool onWhitelist = false;
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (whitelist != null && Utilities.IsValid(localPlayer))
        {
            onWhitelist = whitelist.IsOnWhitelist(localPlayer);
        }

        if (objectsToToggle != null)
        {
            foreach (GameObject objectToToggle in objectsToToggle)
            {
                if (objectToToggle != null)
                {
                    if (onWhitelist)
                    {
                        objectToToggle.SetActive(toggleOn);
                    }
                    else
                    {
                        objectToToggle.SetActive(!toggleOn);
                    }
                }
            }
        }
    }
}
