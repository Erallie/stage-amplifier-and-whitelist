
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class WhitelistedGlobalToggle : UdonSharpBehaviour
{
    [UdonSynced, FieldChangeCallback(nameof(GlobalOn))]
    private bool _globalOn;

    
    [Tooltip("Set this to the game object that contains the Whitelist script.")]
    public Whitelist whitelist;
    
    [Tooltip("If this option is enabled, this object will not be rendered for people who are not whitelisted.")]
    public bool hideMeshIfNotWhitelisted;

    public GameObject[] objectsToToggle;

    public Toggle menuToggle;

    public string interactTextWhenOn = "Turn Off Globally";
    public string interactTextWhenOff = "Turn On Globally";

    
    public bool GlobalOn
    {
        set
        {
            _globalOn = value;

            if (menuToggle != null) {
                menuToggle.isOn = value;
            }
            foreach (GameObject objectToToggle in objectsToToggle) {
                objectToToggle.SetActive(value);
            }
            SetInteractionText(value);
        }
        get => _globalOn;
    }

    
    void OnEnable()
    {
        WhitelistUpdated();
        _globalOn = objectsToToggle[0].activeSelf;
        SetInteractionText(_globalOn);
    }

    void SetInteractionText(bool value) {
        switch (value)
        {
            case true:
                InteractionText = interactTextWhenOn;
                break;
            case false:
                InteractionText = interactTextWhenOff;
                break;
        }
    }

    public void WhitelistUpdated()
    {
        bool onWhitelist = whitelist.IsOnWhitelist(Networking.LocalPlayer);
        this.GetComponent<Collider>().enabled = onWhitelist;
        if (hideMeshIfNotWhitelisted) {
            this.GetComponent<MeshRenderer>().enabled = onWhitelist;
        }
    }
    public override void Interact() {
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        GlobalOn = !GlobalOn;
    }

}
