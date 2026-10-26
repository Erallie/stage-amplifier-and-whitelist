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
    private int[] activePlayerIds = new int[0];

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
            int index = FindPlayerIndex(player.playerId);

            if (index >= 0)
            {
                RemovePlayerAt(index);
            }
            else
            {
                int[] updated = new int[activePlayerIds.Length + 1];

                for (int i = 0; i < activePlayerIds.Length; i++)
                {
                    updated[i] = activePlayerIds[i];
                }

                updated[updated.Length - 1] = player.playerId;
                activePlayerIds = updated;
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

    private int FindPlayerIndex(int playerId)
    {
        for (int i = 0; i < activePlayerIds.Length; i++)
        {
            if (activePlayerIds[i] == playerId)
            {
                return i;
            }
        }

        return -1;
    }

    private void RemovePlayerAt(int index)
    {
        int[] updated = new int[activePlayerIds.Length - 1];
        int destination = 0;

        for (int i = 0; i < activePlayerIds.Length; i++)
        {
            if (i != index)
            {
                updated[destination] = activePlayerIds[i];
                destination++;
            }
        }

        activePlayerIds = updated;
    }

    public override void OnDeserialization()
    {
        RefreshTags();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        // Resolve players whose synced IDs arrived before their join event.
        RefreshTags();
        WhitelistUpdated();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (player != null && Networking.IsOwner(gameObject))
        {
            int index = FindPlayerIndex(player.playerId);

            if (index >= 0)
            {
                RemovePlayerAt(index);
                RequestSerialization();
            }
        }

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
            hasTag = FindPlayerIndex(localPlayer.playerId) >= 0;
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
            spawnedTags = new GameObject[activePlayerIds.Length];

            for (int i = 0; i < activePlayerIds.Length; i++)
            {
                VRCPlayerApi player =
                    VRCPlayerApi.GetPlayerById(activePlayerIds[i]);

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