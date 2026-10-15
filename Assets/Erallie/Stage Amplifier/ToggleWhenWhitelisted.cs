
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
        foreach(GameObject objectToToggle in objectsToToggle) {
            if (whitelist.IsOnWhitelist(Networking.LocalPlayer)) {
                objectToToggle.SetActive(toggleOn);
            } else {
                objectToToggle.SetActive(!toggleOn);
            }
        }
    }
}
