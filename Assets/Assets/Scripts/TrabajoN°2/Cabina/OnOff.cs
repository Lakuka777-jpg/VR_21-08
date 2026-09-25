using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class OnOff : MonoBehaviour
{
    public XRLever xRLever;

    public BatterySocket batterySocket;

    public Material SinTextura;
    public Material ConTextura;

    public Renderer objetoRenderer;

    void Update()
    {
        if (batterySocket.bateriaConectada && xRLever.value)
        {
            objetoRenderer.material = ConTextura;
        }
        else
        {
            objetoRenderer.material = SinTextura;
        }
    }
}