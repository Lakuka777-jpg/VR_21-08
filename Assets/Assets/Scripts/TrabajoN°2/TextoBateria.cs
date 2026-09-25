using UnityEngine;
using TMPro;

public class TextoBateria : MonoBehaviour
{
    public BatterySocket batterySocket;

    [Header("TEXTO")]
    public TMP_Text texto;

    [Header("MENSAJES")]
    [TextArea]
    public string textoAntes = "Batería requerida";

    [TextArea]
    public string textoDespues = "Batería conectada";

    void Start()
    {
        // Texto inicial
        if (texto != null)
        {
            texto.text = textoAntes;
        }
    }

    void Update()
    {
        if (batterySocket == null || texto == null)
            return;

        if (batterySocket.bateriaConectada)
        {
            // Batería puesta
            texto.text = textoDespues;
        }
        else
        {
            // Batería no puesta
            texto.text = textoAntes;
        }
    }
}
