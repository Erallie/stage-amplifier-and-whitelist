
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;
using VRC.Udon;

[RequireComponent(typeof(VRCPickup))]
public class Microphone : UdonSharpBehaviour
{
    
    [UdonSynced, FieldChangeCallback(nameof(MicHeld))]
    private bool _micHeld;

    [Header("Voice Properties")]
    [Tooltip("This value will be multiplied by the distance between the local player and the player holding the mic to adjust the voice falloff range.")]
    public float voiceMultiplier = 17f;
    [Tooltip("The minimum volume voices will be. This only takes effect when the local player is closer to the player holding the mic.")]
    public float startingVolume = 25f;

    private VRC_Pickup vrcPickup;
    void Start()
    {
        
    }

    void OnEnable() {
        vrcPickup = this.GetComponent<VRC_Pickup>();
    }

    void OnDisable() {
        MicHeld = false;
    }

    public bool MicHeld
    {
        set
        {
            _micHeld = value;

            if (value == false) {
                if (vrcPickup != null)
                {
                    vrcPickup.pickupable = false;
                }
            }
        }
        get => _micHeld;
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

    public override void OnPickup() {
        if (Utilities.IsValid(Networking.LocalPlayer))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            MicHeld = true;
        }
    }

    public override void OnDrop() {
        MicHeld = false;
    }

    void Update() {
        switch (_micHeld) {
            case true:
                // Debug.Log(Networking.localPlayer.isOwner(this.gameObject));
                if (Utilities.IsValid(Networking.LocalPlayer) && !Networking.LocalPlayer.IsOwner(this.gameObject)) {
                    SetVoiceVolume(Networking.GetOwner(this.gameObject));
                }
                break;
            case false:
                if (vrcPickup != null && vrcPickup.pickupable == false) {
                    ResetVoiceVolume(Networking.GetOwner(this.gameObject));
                    vrcPickup.pickupable = true;
                }
                break;
        }
    }
}
