using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BatterySocket : MonoBehaviour
{
    public bool bateriaConectada = false;

    public void BateriaConectada(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag("batery"))
        {
            bateriaConectada = true;
            Debug.Log("Batería conectada");
        }
    }

    public void BateriaDesconectada(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag("batery"))
        {
            bateriaConectada = false;
            Debug.Log("Batería desconectada");
        }
    }
}