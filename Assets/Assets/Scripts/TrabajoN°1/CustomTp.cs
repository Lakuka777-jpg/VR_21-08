using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class CustomTp : MonoBehaviour
{
    public Transform target;
    public TeleportationProvider teleportationProvider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        teleportationProvider = FindAnyObjectByType<TeleportationProvider>();    
    }

    public void TP()
    {
        TeleportRequest teleportRequest = new();
        teleportRequest.destinationPosition = target.position;
        teleportRequest.destinationRotation = target.rotation;
        teleportRequest.matchOrientation = MatchOrientation.TargetUpAndForward;

        teleportationProvider.QueueTeleportRequest(teleportRequest);
    }

    private void OnTriggerEnter(Collider other)
    {
        TP();
    }

    private void OnTriggerStay(Collider other)
    {
        print("que ondi");
    }
}
