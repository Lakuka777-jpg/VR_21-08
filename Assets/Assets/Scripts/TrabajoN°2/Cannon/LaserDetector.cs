using UnityEngine;
using TMPro;

public class LaserDetector : MonoBehaviour
{
    public float distanciaMaxima = 100f;

    public LineRenderer laser;

    public TextMeshPro texto;

    [Header("Tag que queremos detectar")]
    public string tagDetectar = "ObjetoPeligroso";

    void Update()
    {
        Vector3 origen = transform.position;
        Vector3 direccion = transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(origen, direccion, out hit, distanciaMaxima))
        {
            // Extender el láser hasta el objeto
            laser.SetPosition(0, origen);
            laser.SetPosition(1, hit.point);

            // Comprobar el Tag
            if (hit.collider.CompareTag(tagDetectar))
            {
                float distancia = hit.distance;

                texto.text = "Tag: " + hit.collider.tag +
                             "\nDistancia: " + distancia.ToString("F2") + " m";
            }
            else
            {
                texto.text = "Sin objetivo";
            }
        }
        else
        {
            // Si no golpea nada, extender el láser hasta la distancia máxima
            laser.SetPosition(0, origen);
            laser.SetPosition(1, origen + direccion * distanciaMaxima);

            texto.text = "Sin objetivo";
        }
    }
}