
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StageAmplifier : UdonSharpBehaviour
{
    // public string[] whitelist;
    
    [Header("References")]
    [Tooltip("Set this to the game object that contains the Whitelist script.")]
    public Whitelist whitelist;
    
    [Tooltip("Add a collider from another GameObject that will act as a barrier to prevent others from getting onstage. Whitelisted people will not be blocked by the barrier.")]
    public Collider barrierCollider;

    [Tooltip("Add a transform to teleport people to if they somehow manage to get past the barrier.")]
    public Transform teleportOnKickTransform;

    [Tooltip("The Mesh Renderer to hide for the person onstage.")]
    public MeshRenderer meshToHide;

    [Tooltip("The Canvas to hide for the person onstage.")]
    public Canvas canvasToHide;
    
    [Header("Voice Properties")]
    [Tooltip("This value will be multiplied by the distance between the local player and the player holding the mic to adjust the voice falloff range.")]
    public float voiceMultiplier = 17f;
    [Tooltip("The minimum volume voices will be. This only takes effect when the local player is closer to the player holding the mic.")]
    public float startingVolume = 25f;

    [UdonSynced, FieldChangeCallback(nameof(CurrentPerformer))]
    private string _currentPerformer;

    [UdonSynced, FieldChangeCallback(nameof(WhitelistOnly))]
    private bool _whitelistOnly;

    public bool WhitelistOnly
    {
        set
        {
            _whitelistOnly = value;
            WhitelistToggled();
        }
        get => _whitelistOnly;
    }

    void Start()
    {
        _currentPerformer = "";
    }

    public string CurrentPerformer
    {
        set
        {
            _currentPerformer = value;
            WhitelistToggled();
        }
        get => _currentPerformer;
    }

    public void WhitelistToggled()
    {
        if (!barrierCollider || barrierCollider == GetComponent<Collider>())
        {
            return;
        }

        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        if (!Utilities.IsValid(localPlayer))
        {
            return;
        }

        bool isWhitelisted = whitelist.IsOnWhitelist(localPlayer);
        bool isCurrentPerformer = localPlayer.displayName == CurrentPerformer;

        if (isWhitelisted)
        {
            // Whitelisted users are never blocked.
            barrierCollider.enabled = false;
            return;
        }

        if (WhitelistOnly)
        {
            // Restricted mode: block all non-whitelisted users.
            barrierCollider.enabled = true;
            return;
        }

        // Normal mode:
        // Block non-whitelisted users only when somebody else
        // is currently performing.
        barrierCollider.enabled =
            CurrentPerformer != "" &&
            !isCurrentPerformer;
    }

    void OnEnable()
    {
        WhitelistToggled();
    }

    void OnDisable() {
        var players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        VRCPlayerApi.GetPlayers(players);

        foreach (VRCPlayerApi player in players) {
            ResetVoiceVolume(player);
        }
    }

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        bool isWhitelisted = whitelist.IsOnWhitelist(player);

        if (WhitelistOnly && !isWhitelisted)
        {
            player.TeleportTo(
                teleportOnKickTransform.position,
                teleportOnKickTransform.rotation
            );
            
            return;
        }

        if (CurrentPerformer == "")
        {
            CurrentPerformer = player.displayName;
        }

        if (isWhitelisted)
        {
            SetVoiceVolume(player);
            return;
        }

        if (player.displayName == CurrentPerformer)
        {
            SetVoiceVolume(player);
            return;
        }

        player.TeleportTo(
            teleportOnKickTransform.position,
            teleportOnKickTransform.rotation
        );
    }

    void SetVoiceVolume(VRCPlayerApi player) {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        float distance = Math.Abs(Vector3.Distance(localPlayer.GetPosition(), player.GetPosition()));
        player.SetVoiceDistanceFar(Math.Max((voiceMultiplier * distance), startingVolume));
        player.SetVoiceLowpass(false);

        if (localPlayer == player)
        {
            if (meshToHide) {
                meshToHide.enabled = false;
            }
            if (canvasToHide) {
                canvasToHide.enabled = false;
            }
        }
    }

    #region reset voice volume
    public override void OnPlayerTriggerExit(VRCPlayerApi player) {
        PlayerLeave(player);
    }

    public override void OnPlayerRespawn(VRCPlayerApi player) {
        PlayerLeave(player);
    }

    public override void OnPlayerLeft(VRCPlayerApi player) {
        PlayerLeave(player);
    }

    void PlayerLeave(VRCPlayerApi player) {
        if (player.displayName == CurrentPerformer) {
            barrierCollider.enabled = false;
            CurrentPerformer = "";
        }
        ResetVoiceVolume(player);
    }

    void ResetVoiceVolume(VRCPlayerApi player) {
        player.SetVoiceDistanceFar(25f);
        player.SetVoiceLowpass(true);

        if (meshToHide) {
            meshToHide.enabled = true;
        }
        if (canvasToHide) {
            canvasToHide.enabled = true;
        }
    }
    #endregion
}
