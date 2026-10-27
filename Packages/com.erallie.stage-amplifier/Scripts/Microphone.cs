using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;

[RequireComponent(typeof(VRCPickup))]
public class Microphone : UdonSharpBehaviour
{
    [UdonSynced, FieldChangeCallback(nameof(MicHeld))]
    private bool _micHeld;

    [Header("Voice Properties")]
    [Tooltip("This value will be multiplied by the distance between the local player and the player holding the mic to adjust the voice falloff range. The default value is generally good for both earmuff mode and non-earmuff mode.")]
    public float voiceMultiplier = 17f;

    [Tooltip("The minimum volume voices will be. This only takes effect when the local player is closer to the player holding the mic.")]
    public float startingVolume = 25f;

    private VRC_Pickup vrcPickup;

    // Each client remembers whose voice it actually changed.
    private VRCPlayerApi amplifiedPlayer;

    private void OnEnable()
    {
        vrcPickup = GetComponent<VRC_Pickup>();

        if (vrcPickup != null)
        {
            vrcPickup.pickupable = true;
        }
    }

    private void OnDisable()
    {
        MicHeld = false;
        ResetAmplifiedPlayer();
    }

    public bool MicHeld
    {
        set
        {
            _micHeld = value;

            if (!value)
            {
                ResetAmplifiedPlayer();
            }
        }

        get => _micHeld;
    }

    private void SetVoiceVolume(VRCPlayerApi player)
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        if (Utilities.IsValid(localPlayer) && Utilities.IsValid(player))
        {
            if (amplifiedPlayer != player)
            {
                ResetAmplifiedPlayer();
                amplifiedPlayer = player;
            }

            float distance = Vector3.Distance(
                localPlayer.GetPosition(),
                player.GetPosition());

            player.SetVoiceDistanceFar(
                Math.Max(voiceMultiplier * distance, startingVolume));

            player.SetVoiceLowpass(false);
        }
    }

    private void ResetVoiceVolume(VRCPlayerApi player)
    {
        if (Utilities.IsValid(player))
        {
            player.SetVoiceDistanceFar(25f);
            player.SetVoiceLowpass(true);
        }
    }

    private void ResetAmplifiedPlayer()
    {
        if (Utilities.IsValid(amplifiedPlayer))
        {
            ResetVoiceVolume(amplifiedPlayer);
        }

        amplifiedPlayer = null;
    }

    public override void OnPickup()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        if (Utilities.IsValid(localPlayer))
        {
            Networking.SetOwner(localPlayer, gameObject);
            MicHeld = true;
        }
    }

    public override void OnDrop()
    {
        MicHeld = false;
    }

    private void Update()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        if (_micHeld && Utilities.IsValid(localPlayer))
        {
            VRCPlayerApi holder = Networking.GetOwner(gameObject);

            if (Utilities.IsValid(holder) && holder != localPlayer)
            {
                SetVoiceVolume(holder);
            }
            else
            {
                ResetAmplifiedPlayer();
            }
        }
        else
        {
            ResetAmplifiedPlayer();
        }
    }
}