using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class StaffTag : UdonSharpBehaviour
{
    [Tooltip("Vertical distance above the target player's head, in meters.")]
    public float verticalOffset = 0.3f;

    private VRCPlayerApi targetPlayer;

    public void SetTargetPlayer(VRCPlayerApi player)
    {
        targetPlayer = player;
    }

    private void Update()
    {
        if (Utilities.IsValid(targetPlayer))
        {
            Vector3 headPosition = targetPlayer.GetTrackingData(
                VRCPlayerApi.TrackingDataType.Head).position;

            transform.position =
                headPosition + Vector3.up * verticalOffset;

            VRCPlayerApi localPlayer = Networking.LocalPlayer;

            if (Utilities.IsValid(localPlayer))
            {
                Vector3 viewerPosition = localPlayer.GetTrackingData(
                    VRCPlayerApi.TrackingDataType.Head).position;

                if ((viewerPosition - transform.position).sqrMagnitude > 0.0001f)
                {
                    transform.LookAt(viewerPosition);
                }
            }
        }
    }
}