
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[RequireComponent(typeof(Collider))]
public class MinimalAmplifier : UdonSharpBehaviour
{
    [Header("Voice Properties")]
    [Tooltip("This value will be multiplied by the distance between the local player and the person onstage to adjust the voice falloff range. The default value is generally good for both earmuff mode and non-earmuff mode.")]
    public float voiceMultiplier = 17f;

    [Tooltip("The minimum volume voices will be. This only takes effect when the local player is closer to the player(s) onstage.")]
    public float startingVolume = 25f;
    
    [Tooltip("Whether or not to stop amplifying the voice when the disatance between the local player and the person onstage is greater than a certain distance.")]
    public bool limitAmplificationRange = false;
    
    [Tooltip("The distance in meters at which voice amplification will stop if Limit Amplification Range is enabled.")]
    public float limitDistance = 100f;

    void OnDisable() {
        var players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        VRCPlayerApi.GetPlayers(players);

        foreach (VRCPlayerApi player in players) {
            ResetVoiceVolume(player);
        }
    }

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        if (Utilities.IsValid(localPlayer) && Utilities.IsValid(player))
        {
            float distance = Math.Abs(Vector3.Distance(localPlayer.GetPosition(), player.GetPosition()));
            if (limitAmplificationRange && distance > limitDistance)
            {
                ResetVoiceVolume(player);
                return;
            }
            player.SetVoiceDistanceFar(Math.Max((voiceMultiplier * distance), startingVolume));
            player.SetVoiceLowpass(false);
        }
    }

    #region reset voice volume
    public override void OnPlayerTriggerExit(VRCPlayerApi player) {
        ResetVoiceVolume(player);
    }

    public override void OnPlayerRespawn(VRCPlayerApi player) {
        ResetVoiceVolume(player);
    }

    public override void OnPlayerLeft(VRCPlayerApi player) {
        ResetVoiceVolume(player);
    }

    void ResetVoiceVolume(VRCPlayerApi player) {
        if (Utilities.IsValid(player))
        {
            player.SetVoiceDistanceFar(25f);
            player.SetVoiceLowpass(true);
        }
    }

    #endregion
}
