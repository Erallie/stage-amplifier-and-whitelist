

using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[RequireComponent(typeof(Collider))]
public class GlobalAmplifier : UdonSharpBehaviour
{
    
    [UdonSynced, FieldChangeCallback(nameof(Amplified))]
    private bool _amplified;

    [Header("Voice Properties")]
    [Tooltip("This value will be multiplied by the distance between the local player and the player holding the mic to adjust the voice falloff range.")]
    public float voiceMultiplier = 17f;
    [Tooltip("The minimum volume voices will be. This only takes effect when the local player is closer to the player holding the mic.")]
    public float startingVolume = 25f;

    
    [Tooltip("Set this to the game object that contains the Whitelist script.")]
    public Whitelist whitelist;
    
    [Tooltip("If this option is enabled, this object will not be rendered for people who are not whitelisted.")]
    public bool hideMeshIfNotWhitelisted;

    private bool hasUpdated = true;
    
    public bool Amplified
    {
        set
        {
            _amplified = value;

            if (value == false) {
                hasUpdated = false;
            }
        }
        get => _amplified;
    }

    
    
    void OnEnable() {
        WhitelistUpdated();
        hasUpdated = true;
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

        // If the person currently being amplified is no longer whitelisted,
        // automatically turn off the amplifier.
        VRCPlayerApi amplifiedPlayer = Networking.GetOwner(this.gameObject);
        if (_amplified && Networking.LocalPlayer == amplifiedPlayer)
        {
            if (whitelist != null && Utilities.IsValid(amplifiedPlayer) && !whitelist.IsOnWhitelist(amplifiedPlayer))
            {
                Amplified = false;
            }
        }
    }
    
    void SetVoiceVolume(VRCPlayerApi player) {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        if (Utilities.IsValid(localPlayer) && Utilities.IsValid(player))
        {
            float distance = Math.Abs(Vector3.Distance(localPlayer.GetPosition(), player.GetPosition()));
            player.SetVoiceDistanceFar(Math.Max((voiceMultiplier * distance), startingVolume));
            player.SetVoiceLowpass(false);
        }
    }

    void ResetVoiceVolume(VRCPlayerApi player) {
        if (Utilities.IsValid(player))
        {
            player.SetVoiceDistanceFar(25f);
            player.SetVoiceLowpass(true);
        }
    }
    public override void Interact()
    {
        if (Utilities.IsValid(Networking.LocalPlayer))
        {
            switch (Amplified)
            {
                case true:
                    if (Networking.LocalPlayer.IsOwner(this.gameObject))
                    {
                        Amplified = false;
                    }
                    break;
                case false:
                        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
                        Amplified = true;
                    break;
            }
        }
    }

    public override void OnOwnershipTransferred(VRCPlayerApi newOwner)
    {
        if (Amplified == true)
        {
            Amplified = false;
        }
    }

    
    void Update() {
        if (Utilities.IsValid(Networking.LocalPlayer))
        {
            switch (_amplified) {
                case true:
                    // Debug.Log(Networking.localPlayer.isOwner(this.gameObject));
                    if (!Networking.LocalPlayer.IsOwner(this.gameObject)) {
                        SetVoiceVolume(Networking.GetOwner(this.gameObject));
                        InteractionText = "Please Wait";
                    }
                    else
                    {
                        InteractionText = "Unamplify Yourself";
                    }
                    break;
                case false:
                    if (hasUpdated == false && !Networking.LocalPlayer.IsOwner(this.gameObject)) {
                        ResetVoiceVolume(Networking.GetOwner(this.gameObject));
                        hasUpdated = true;
                    }
                    InteractionText = "Global Amplifier";
                    break;
            }
        }
    }
}
