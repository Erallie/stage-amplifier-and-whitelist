
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleOthersOnActiveSelf : UdonSharpBehaviour
{
    
    [Tooltip("Set this to the other GameObject you want to toggle when this object is enabled/disabled.")]
    public GameObject[] toToggle;

    void OnEnable() {
        if (toToggle != null && toToggle.Length != 0) {
            foreach (GameObject thisObj in toToggle) {
                if (thisObj != null)
                {
                    thisObj.SetActive(true);
                }
            }
        }
    }

    void OnDisable() {
        if (toToggle != null && toToggle.Length != 0) {
            foreach (GameObject thisObj in toToggle) {
                if (thisObj != null)
                {
                    thisObj.SetActive(false);
                }
            }
        }
    }
}
