using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class StageWhitelistToggle : UdonSharpBehaviour
{
    [UdonSynced, FieldChangeCallback(nameof(WhitelistOnly))]
    private bool _whitelistOnly;
    
    [Tooltip("Set this to the game object that contains the Whitelist script.")]
    public Whitelist whitelist;
    
    [Tooltip("If this option is enabled, this object will not be rendered for people who are not whitelisted.")]
    public bool hideMeshIfNotWhitelisted;

    public StageAmplifier stageAmplifier;

    public string interactTextWhenOn = "Allow Unwhitelisted People Onstage";
    public string interactTextWhenOff = "Disallow Unwhitelisted People Onstage";

    
    public bool WhitelistOnly
    {
        set
        {
            _whitelistOnly = value;
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
        get => _whitelistOnly;
    }

    void OnEnable() {
        WhitelistUpdated();

        if (stageAmplifier != null)
        {
            WhitelistOnly = stageAmplifier.WhitelistOnly;
        }
    }

    public void WhitelistUpdated()
    {
        bool onWhitelist = false;
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (whitelist != null && Utilities.IsValid(localPlayer))
        {
            onWhitelist = whitelist.IsOnWhitelist(localPlayer);
        }
        Collider controlCollider = GetComponent<Collider>();
        if (controlCollider != null)
        {
            controlCollider.enabled = onWhitelist;
        }
        if (hideMeshIfNotWhitelisted) {
            MeshRenderer controlMesh = GetComponent<MeshRenderer>();
            if (controlMesh != null)
            {
                controlMesh.enabled = onWhitelist;
            }
        }
    }

    public override void Interact() {
        if (stageAmplifier != null && Utilities.IsValid(Networking.LocalPlayer))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Networking.SetOwner(Networking.LocalPlayer, stageAmplifier.gameObject);

            bool value = !stageAmplifier.WhitelistOnly;

            stageAmplifier.WhitelistOnly = value;
            WhitelistOnly = value;

            stageAmplifier.RequestSerialization();
            RequestSerialization();
        }
    }
}