using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class VolumenMusica : MonoBehaviour
{
    public XRSlider slider;
    public AudioSource musica;
    public BatterySocket batterySocket;

    void Update()
    {
        if (batterySocket.bateriaConectada)
        {
            musica.volume = slider.value;
        }
        else
        {
            musica.volume = 0f;
        }
    }
}