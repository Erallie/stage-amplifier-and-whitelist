using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.UdonNetworkCalling;
using VRC.Udon.Common.Interfaces;

[RequireComponent(typeof(Collider))]
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class StaffTagToggle : UdonSharpBehaviour
{
    [Header("References")]
    [Tooltip("The Whitelist component that determines who can use this toggle.")]
    public Whitelist whitelist;

    [Tooltip("The prefab to spawn. Its root GameObject must have StaffTag.")]
    public GameObject staffTagPrefab;

    [Header("Whitelist Settings")]
    [Tooltip("Hide this object's mesh for players who are not whitelisted.")]
    public bool hideMeshIfNotWhitelisted;

    [UdonSynced]
    private string[] activePlayerNames = new string[0];

    private GameObject[] spawnedTags = new GameObject[0];

    private void Start()
    {
        WhitelistUpdated();
        RefreshTags();
    }

    public override void Interact()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        if (CanUse(localPlayer) && staffTagPrefab != null)
        {
            if (Networking.IsOwner(gameObject))
            {
                ToggleForPlayer(localPlayer);
            }
            else
            {
                SendCustomNetworkEvent(
                    NetworkEventTarget.Owner,
                    nameof(RequestToggle));
            }
        }
    }

    [NetworkCallable]
    public void RequestToggle()
    {
        if (Networking.IsOwner(gameObject))
        {
            VRCPlayerApi sender = NetworkCalling.CallingPlayer;

            if (CanUse(sender))
            {
                ToggleForPlayer(sender);
            }
        }
    }

    private bool CanUse(VRCPlayerApi player)
    {
        return Utilities.IsValid(player) &&
            whitelist != null &&
            whitelist.IsOnWhitelist(player);
    }

    private void ToggleForPlayer(VRCPlayerApi player)
    {
        if (staffTagPrefab != null)
        {
            int index = FindPlayerIndex(player.displayName);

            if (index >= 0)
            {
                RemovePlayerAt(index);
            }
            else
            {
                string[] updated =
                    new string[activePlayerNames.Length + 1];

                for (int i = 0; i < activePlayerNames.Length; i++)
                {
                    updated[i] = activePlayerNames[i];
                }

                updated[updated.Length - 1] = player.displayName;
                activePlayerNames = updated;
            }

            RefreshTags();
            RequestSerialization();
        }
        else
        {
            Debug.LogError(
                "[StaffTagToggle] Assign a StaffTag prefab.");
        }
    }

    private int FindPlayerIndex(string playerName)
    {
        for (int i = 0; i < activePlayerNames.Length; i++)
        {
            if (activePlayerNames[i] == playerName)
            {
                return i;
            }
        }

        return -1;
    }

    private void RemovePlayerAt(int index)
    {
        string[] updated =
            new string[activePlayerNames.Length - 1];

        int destination = 0;

        for (int i = 0; i < activePlayerNames.Length; i++)
        {
            if (i != index)
            {
                updated[destination] = activePlayerNames[i];
                destination++;
            }
        }

        activePlayerNames = updated;
    }

    private VRCPlayerApi FindPlayerByName(string playerName)
    {
        VRCPlayerApi[] players =
            new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];

        VRCPlayerApi.GetPlayers(players);

        foreach (VRCPlayerApi player in players)
        {
            if (Utilities.IsValid(player) &&
                player.displayName == playerName)
            {
                return player;
            }
        }

        return null;
    }

    public override void OnDeserialization()
    {
        RefreshTags();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        // Recreate enabled tags when players join or return.
        RefreshTags();
        WhitelistUpdated();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        // Keep their saved enabled state, but remove their visual tag.
        RefreshTags();

        // Refresh again after the departing player has been removed.
        SendCustomEventDelayedFrames(nameof(RefreshAfterPlayerLeft), 1);
    }

    public void RefreshAfterPlayerLeft()
    {
        RefreshTags();
    }

    public void WhitelistUpdated()
    {
        bool allowed = CanUse(Networking.LocalPlayer);

        Collider controlCollider = GetComponent<Collider>();

        if (controlCollider != null)
        {
            controlCollider.enabled = allowed;
        }

        MeshRenderer controlMesh = GetComponent<MeshRenderer>();

        if (controlMesh != null)
        {
            controlMesh.enabled = !hideMeshIfNotWhitelisted || allowed;
        }

        UpdateInteractionText();
    }

    private void UpdateInteractionText()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        bool hasTag = false;

        if (Utilities.IsValid(localPlayer))
        {
            hasTag = FindPlayerIndex(localPlayer.displayName) >= 0;
        }

        InteractionText = hasTag
            ? "Hide Your Staff Tag"
            : "Show Your Staff Tag";
    }

    private void ClearTags()
    {
        foreach (GameObject tag in spawnedTags)
        {
            if (tag != null)
            {
                tag.SetActive(false);
                Destroy(tag);
            }
        }

        spawnedTags = new GameObject[0];
    }

    private void RefreshTags()
    {
        ClearTags();

        if (staffTagPrefab != null)
        {
            spawnedTags = new GameObject[activePlayerNames.Length];

            for (int i = 0; i < activePlayerNames.Length; i++)
            {
                VRCPlayerApi player =
                    FindPlayerByName(activePlayerNames[i]);

                if (Utilities.IsValid(player))
                {
                    GameObject instance = VRCInstantiate(staffTagPrefab);

                    if (instance != null)
                    {
                        instance.SetActive(false);

                        StaffTag tag = instance.GetComponent<StaffTag>();

                        if (tag != null)
                        {
                            tag.SetTargetPlayer(player);
                            spawnedTags[i] = instance;
                            instance.SetActive(true);
                        }
                        else
                        {
                            Destroy(instance);
                        }
                    }
                }
            }
        }

        UpdateInteractionText();
    }

    private void OnDisable()
    {
        ClearTags();
    }

    private void OnEnable()
    {
        WhitelistUpdated();
        RefreshTags();
    }
}