using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Oleadas")]
    [Tooltip("Arrastra aquí los 4 prefabs de las oleadas en orden")]
    public GameObject[] oleadasPrefabs = new GameObject[4];

    [Header("Configuración")]
    public float tiempoEntreOleadas = 30f;   // Cada 30 segundos
    public Transform puntoSpawn;            // Dónde aparecen las oleadas (opcional)

    [Header("Debug")]
    public bool mostrarDebug = true;

    private int oleadaActual = 0;
    private bool sistemaActivo = true;

    void Start()
    {
        // Validación rápida
        if (oleadasPrefabs == null || oleadasPrefabs.Length == 0)
        {
            Debug.LogError("WaveManager: No hay prefabs asignados!");
            return;
        }

        // Activar la primera oleada inmediatamente
        ActivarOleada(0);

        // Empezar el ciclo de las siguientes oleadas
        StartCoroutine(CicloOleadas());
    }

    IEnumerator CicloOleadas()
    {
        // Esperamos 30 segundos antes de la 2ª, 3ª y 4ª
        while (oleadaActual < oleadasPrefabs.Length - 1 && sistemaActivo)
        {
            yield return new WaitForSeconds(tiempoEntreOleadas);

            oleadaActual++;
            ActivarOleada(oleadaActual);
        }

        if (mostrarDebug)
            Debug.Log("Todas las oleadas han sido activadas.");
    }

    void ActivarOleada(int indice)
    {
        if (indice < 0 || indice >= oleadasPrefabs.Length)
            return;

        GameObject prefab = oleadasPrefabs[indice];

        if (prefab == null)
        {
            Debug.LogWarning($"Oleada {indice + 1} tiene prefab nulo.");
            return;
        }

        // Instanciar el prefab
        Vector3 posicion = puntoSpawn != null ? puntoSpawn.position : Vector3.zero;
        Quaternion rotacion = puntoSpawn != null ? puntoSpawn.rotation : Quaternion.identity;

        GameObject instancia = Instantiate(prefab, posicion, rotacion);

        if (mostrarDebug)
            Debug.Log($"Oleada {indice + 1} activada: {prefab.name}");
    }

    // Por si quieres detener el sistema (por ejemplo cuando termine el juego a los 2 minutos)
    public void DetenerOleadas()
    {
        sistemaActivo = false;
        StopAllCoroutines();
    }
}