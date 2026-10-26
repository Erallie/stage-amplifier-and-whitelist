using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

#if ERALLIE_PROTV_PACKAGE
using ArchiTech.ProTV;
#endif

[RequireComponent(typeof(Collider))]
public class VolumeSyncToggle : UdonSharpBehaviour
{
    [Header("References")]
    [Tooltip("Set this to the game object that contains the Whitelist script.")]
    public Whitelist whitelist;

#if ERALLIE_PROTV_PACKAGE
    [Tooltip("Set this to the ProTV TVManager.")]
    public TVManager tvManager;
#endif

    [Header("Whitelist Settings")]
    [Tooltip("If enabled, this object's mesh will be hidden for people who are not whitelisted.")]
    public bool hideMeshIfNotWhitelisted;

    [UdonSynced, FieldChangeCallback(nameof(SyncVolume))]
    private bool _syncVolume;

    public bool SyncVolume
    {
        set
        {
            _syncVolume = value;

#if ERALLIE_PROTV_PACKAGE
            if (tvManager != null)
            {
                tvManager.syncVolumeControl = value;
            }
#endif

            switch (value)
            {
                case true:
                    InteractionText = "Disable Globally Synced Volume";
                    break;

                case false:
                    InteractionText = "Enable Globally Synced Volume";
                    break;
            }
        }

        get => _syncVolume;
    }

    void Start()
    {
#if ERALLIE_PROTV_PACKAGE
        SyncVolume = tvManager != null && tvManager.syncVolumeControl;
#else
        SyncVolume = false;
#endif
    }

    void OnEnable()
    {
        WhitelistUpdated();
    }

    public void WhitelistUpdated()
    {
        bool onWhitelist = false;
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (whitelist != null && Utilities.IsValid(localPlayer))
        {
            onWhitelist = whitelist.IsOnWhitelist(localPlayer);
        }

        Collider thisCollider = GetComponent<Collider>();
        if (thisCollider != null)
        {
            thisCollider.enabled = onWhitelist;
        }

        if (hideMeshIfNotWhitelisted)
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = onWhitelist;
            }
        }
    }

    public override void Interact()
    {
#if ERALLIE_PROTV_PACKAGE
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (whitelist != null && Utilities.IsValid(localPlayer))
        {
            if (whitelist.IsOnWhitelist(localPlayer))
            {
                Networking.SetOwner(localPlayer, gameObject);
                SyncVolume = !SyncVolume;
                RequestSerialization();
            }
        }
#endif
    }
}