using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Components;

[RequireComponent(typeof(Collider))]
public class WhitelistedToggle : UdonSharpBehaviour
{
    [Header("Access")]
    [Tooltip("Set this to the game object that contains the Whitelist script. Without a whitelist, interaction is disabled.")]
    public Whitelist whitelist;

    [Tooltip("Hide this object's Mesh Renderer from players who are not whitelisted.")]
    public bool hideMeshIfNotWhitelisted;

    [Header("Objects")]
    [Tooltip("Objects to toggle globally.")]
    public GameObject[] objectsToToggle;

    [Tooltip("Transfer ownership of the objects and their children to the player who interacts with this toggle. This is essential if there are Udon behaviours on the objects or their children.")]
    public bool transferOwnership = true;

    [Tooltip("Optional additional objects whose ownership should be transferred, such as children containing Udon behaviours.")]
    public GameObject[] childObjects;

    [InspectorName("Respawn On Disable")]
    [Tooltip("Respawn toggled objects when toggling them off.")]
    public bool respawnOnDisable;

    [Header("Optional UI")]
    [Tooltip("Optional UI toggle that displays whether the objects are toggled on or off. Useful if you have a menu that also toggles these objects locally.")]
    public Toggle menuToggle;

    [Header("Interaction Text")]
    [Tooltip("Override the interaction text according to the toggle state. When unchecked, leave the existing interaction text unchanged.")]
    public bool customizeInteractionText;

    [Tooltip("Interaction text displayed while the objects are on.")]
    public string interactTextWhenOn = "Turn Off Globally";

    [Tooltip("Interaction text displayed while the objects are off.")]
    public string interactTextWhenOff = "Turn On Globally";

    [UdonSynced, FieldChangeCallback(nameof(ToggledOn))]
    private bool _toggledOn;

    private bool initialized;

    public bool ToggledOn
    {
        get => _toggledOn;

        set
        {
            bool changed = _toggledOn != value;

            _toggledOn = value;
            initialized = true;

            ApplyState();

            if (changed &&
                !value &&
                respawnOnDisable &&
                Networking.IsOwner(gameObject))
            {
                RespawnObjects();
            }
        }
    }

    public bool GlobalOn
    {
        get => ToggledOn;
        set => ToggledOn = value;
    }

    private void OnEnable()
    {
        if (!initialized)
        {
            if (objectsToToggle != null)
            {
                foreach (GameObject toggleObj in objectsToToggle)
                {
                    if (toggleObj != null)
                    {
                        _toggledOn = toggleObj.activeSelf;
                        break;
                    }
                }
            }

            initialized = true;
        }

        WhitelistUpdated();
        ApplyState();
    }

    public void WhitelistUpdated()
    {
        bool onWhitelist = CanUseControl();

        Collider controlCollider = GetComponent<Collider>();

        if (controlCollider != null)
        {
            controlCollider.enabled = onWhitelist;
        }

        if (hideMeshIfNotWhitelisted)
        {
            MeshRenderer controlMesh = GetComponent<MeshRenderer>();

            if (controlMesh != null)
            {
                controlMesh.enabled = onWhitelist;
            }
        }

        if (menuToggle != null)
        {
            menuToggle.interactable = onWhitelist;
        }
    }

    public override void Interact()
    {
        SetSharedState(!ToggledOn);
    }

    public void OnMenuToggleChanged()
    {
        if (menuToggle != null)
        {
            SetSharedState(menuToggle.isOn);
        }
    }

    private bool CanUseControl()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        return Utilities.IsValid(localPlayer) &&
            whitelist != null &&
            whitelist.IsOnWhitelist(localPlayer);
    }

    private void SetSharedState(bool value)
    {
        if (!CanUseControl())
        {
            UpdateUI();
            return;
        }

        if (value == ToggledOn)
        {
            UpdateUI();
            return;
        }

        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        Networking.SetOwner(localPlayer, gameObject);

        if (!Networking.IsOwner(gameObject))
        {
            UpdateUI();
            return;
        }

        if (transferOwnership || (respawnOnDisable && !value))
        {
            TakeObjectOwnership(objectsToToggle, localPlayer);
        }

        if (transferOwnership)
        {
            TakeObjectOwnership(childObjects, localPlayer);
        }

        ToggledOn = value;
    }

    private void TakeObjectOwnership(
        GameObject[] objects,
        VRCPlayerApi player)
    {
        if (objects != null)
        {
            foreach (GameObject obj in objects)
            {
                if (obj != null)
                {
                    Networking.SetOwner(player, obj);
                }
            }
        }
    }

    private void ApplyState()
    {
        if (objectsToToggle != null)
        {
            foreach (GameObject toggleObj in objectsToToggle)
            {
                if (toggleObj != null)
                {
                    toggleObj.SetActive(_toggledOn);
                }
            }
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (customizeInteractionText)
        {
            InteractionText = _toggledOn
                ? interactTextWhenOn
                : interactTextWhenOff;
        }

        if (menuToggle != null)
        {
            menuToggle.SetIsOnWithoutNotify(_toggledOn);
        }
    }

    private void RespawnObjects()
    {
        if (objectsToToggle != null)
        {
            foreach (GameObject toggleObj in objectsToToggle)
            {
                if (toggleObj != null && Networking.IsOwner(toggleObj))
                {
                    VRCObjectSync sync =
                        toggleObj.GetComponent<VRCObjectSync>();

                    if (sync != null)
                    {
                        sync.Respawn();
                    }
                }
            }
        }
    }
}