using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Components;

[RequireComponent(typeof(Collider))]
public class WhitelistedToggle : UdonSharpBehaviour
{
    [Header("Access")]
    [Tooltip("The whitelist that authorizes this control. Without a whitelist, interaction is disabled.")]
    public Whitelist whitelist;

    [Tooltip("Hide this object's Mesh Renderer from players who are not whitelisted.")]
    public bool hideMeshIfNotWhitelisted;

    [Tooltip("Optional canvases to hide from players who are not whitelisted.")]
    public Canvas[] hiddenCanvases;

    [Header("Objects")]
    [Tooltip("Objects to enable or disable for everyone. The first assigned object supplies the initial state. Keep this controller outside these objects so it remains active.")]
    public GameObject[] objectsToToggle;

    [Tooltip("Transfer ownership of the listed objects and additional child objects to the player using the control. Disable this if ownership should remain unchanged.")]
    public bool transferOwnership = true;

    [Tooltip("Optional additional objects whose ownership should be transferred, such as children containing Udon behaviours.")]
    public GameObject[] childObjects;

    [InspectorName("Respawn On Disable")]
    [Tooltip("Respawn toggled objects with VRC Object Sync when switching them off. The field's original name is preserved to retain existing Inspector assignments.")]
    public bool respawnOnEnable;

    [Header("Optional UI")]
    [Tooltip("Optional UI toggle that displays the shared state. Its value is updated without firing On Value Changed. To control this script from the toggle, connect that event to OnMenuToggleChanged.")]
    public Toggle menuToggle;

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
                respawnOnEnable &&
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

        if (hiddenCanvases != null)
        {
            foreach (Canvas canvas in hiddenCanvases)
            {
                if (canvas != null)
                {
                    canvas.enabled = onWhitelist;
                }
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

        if (transferOwnership || (respawnOnEnable && !value))
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
        InteractionText = _toggledOn
            ? interactTextWhenOn
            : interactTextWhenOff;

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