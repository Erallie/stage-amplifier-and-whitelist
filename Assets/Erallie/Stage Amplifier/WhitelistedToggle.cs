
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;
using VRC.Udon;

public class WhitelistedToggle : UdonSharpBehaviour
{
    [Tooltip("Set this to the game object that contains the Whitelist script.")]
    public Whitelist whitelist;
    // private VRCPlayerApi localPlayer;


    [Tooltip("Add the objects that you want to toggle on and off.")]
    public GameObject[] objectsToToggle;

    [Tooltip("Add any of the toggled object's children that have udon behaviors.")]
    public GameObject[] childObjects;

    [Tooltip("A list of canvases to hide for unwhitelisted people.")]
    public Canvas[] hiddenCanvases;

    [Tooltip("If this option is enabled, the objects will be respawned whenever they are toggled on.")]
    public bool respawnOnEnable;

    [Tooltip("If this option is enabled, this object will not be rendered for people who are not whitelisted.")]
    public bool hideMeshIfNotWhitelisted;

    [UdonSynced, FieldChangeCallback(nameof(ToggledOn))]
    private bool _toggledOn;

    private VRCObjectSync[] objectSyncs;


    void Start()
    {
        if (!respawnOnEnable)
        {
            return;
        }

        int count = 0;

        foreach (GameObject toggleObj in objectsToToggle)
        {
            if (toggleObj.GetComponent<VRCObjectSync>() != null)
            {
                count++;
            }
        }

        objectSyncs = new VRCObjectSync[count];

        int index = 0;
        foreach (GameObject toggleObj in objectsToToggle)
        {
            VRCObjectSync sync = toggleObj.GetComponent<VRCObjectSync>();
            if (sync != null)
            {
                objectSyncs[index++] = sync;
            }
        }
    }


    void OnEnable() {
        WhitelistUpdated();
        _toggledOn = objectsToToggle[0].activeInHierarchy;
    }

    public void WhitelistUpdated()
    {
        bool onWhitelist = whitelist.IsOnWhitelist(Networking.LocalPlayer);
        this.GetComponent<Collider>().enabled = onWhitelist;
        if (hideMeshIfNotWhitelisted) {
            this.GetComponent<MeshRenderer>().enabled = onWhitelist;
        }
        
        if (hiddenCanvases.Length != 0) {
            foreach (Canvas canvas in hiddenCanvases) {
                canvas.enabled = onWhitelist;
            }
        }
    }

    public override void Interact() {
        // localPlayer = Networking.LocalPlayer;
        bool isOn = objectsToToggle[0].activeSelf;
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        ToggledOn = !isOn;
    }

    public bool ToggledOn
    {
        set
        {
            _toggledOn = value;
            
            VRCPlayerApi owner = Networking.GetOwner(this.gameObject);
            
            if (objectsToToggle.Length != 0) {
                foreach (GameObject toggleObj in objectsToToggle) {
                    toggleObj.SetActive(value);
                    Networking.SetOwner(owner, toggleObj);
                }
            }

            if (childObjects.Length != 0) {
                // Debug.Log(childObjects.Length);
                foreach (GameObject thisObj in childObjects) {
                    Networking.SetOwner(owner, thisObj);
                }
            }
            
            if (respawnOnEnable && value == false && objectsToToggle.Length != 0 && Networking.IsOwner(this.gameObject)) {
                foreach (GameObject toggleObj2 in objectsToToggle) {
                    VRCObjectSync sync = toggleObj2.GetComponent<VRCObjectSync>();
                    if (sync == null) {
                        continue;
                    }
                    sync.Respawn();
                }
            }

        }
        get => _toggledOn;
    }
}
