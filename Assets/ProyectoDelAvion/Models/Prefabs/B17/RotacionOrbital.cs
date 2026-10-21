
using UnityEngine;

public class RotacionOrbital : MonoBehaviour
{
    [System.Serializable]
    public class ParRotacion
    {
        [Header("Objetos")]
        public Transform centroVacio;
        public Transform objeto;

        [Header("Configuracion")]
        public float velocidad = 45f;
        public Vector3 ejeRotacion = Vector3.up;
    }

    public ParRotacion[] objetos;

    void Update()
    {
        foreach (ParRotacion par in objetos)
        {
            if (par.centroVacio == null || par.objeto == null)
                continue;

            // El vacio rota indefinidamente sobre su propio eje
            par.centroVacio.Rotate(
                par.ejeRotacion.normalized * par.velocidad * Time.deltaTime,
                Space.Self
            );

            // El objeto orbita alrededor del centro del vacio
            par.objeto.RotateAround(
                par.centroVacio.position,
                par.centroVacio.TransformDirection(
                    par.ejeRotacion.normalized
                ),
                par.velocidad * Time.deltaTime
            );
        }
    }
}
