using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
#if ERALLIE_PROTV
using ArchiTech.ProTV;
#endif

public class VolumeSyncToggle : UdonSharpBehaviour
{
    [Header("References")]
    [Tooltip("Set this to the game object that contains the Whitelist script.")]
    public Whitelist whitelist;

#if ERALLIE_PROTV
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

#if ERALLIE_PROTV
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
        // Start with whatever value is currently configured on the TVManager.
#if ERALLIE_PROTV
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
        bool onWhitelist = whitelist.IsOnWhitelist(Networking.LocalPlayer);

        Collider thisCollider = GetComponent<Collider>();
        thisCollider.enabled = onWhitelist;

        if (hideMeshIfNotWhitelisted)
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = onWhitelist;
        }
    }


    public override void Interact()
    {
#if !ERALLIE_PROTV
        return;
#else
        if (!whitelist.IsOnWhitelist(Networking.LocalPlayer))
        {
            return;
        }

        Networking.SetOwner(Networking.LocalPlayer, gameObject);

        SyncVolume = !SyncVolume;

        RequestSerialization();
#endif
    }
}