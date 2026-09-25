using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SpotLightColor : MonoBehaviour
{
    public XRLever xRLever;

    public BatterySocket batterySocket;

    [Header("SPOT LIGHT")]
    public Light spotLight;

    [Header("COLORES")]
    public Color colorApagado = Color.red;
    public Color colorEncendido = Color.white;

    void Update()
    {
        if (batterySocket.bateriaConectada && xRLever.value)
        {
            // Batería conectada + palanca encendida
            spotLight.color = colorEncendido;
        }
        else
        {
            // Batería desconectada o palanca apagada
            spotLight.color = colorApagado;
        }
    }
}
