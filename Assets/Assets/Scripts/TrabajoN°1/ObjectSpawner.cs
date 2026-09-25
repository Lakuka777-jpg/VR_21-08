using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObjectSpawner : MonoBehaviour
{
    public static ObjectSpawner Instance;

    [Header("UI")]
    public TMP_Text textoContador;
    public Button botonSpawn;

    [Header("Spawn")]
    public GameObject ObjectoASpawn;
    public Transform PuntoSpawn;

    [Header("Configuración")]
    public int destruccionNecesarias = 3;

    private int destrucciones = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        destrucciones = 0;

        // El botón empieza desactivado
        botonSpawn.gameObject.SetActive(false);

        ActualizarTexto();
    }

    public void RegistrarDestruccion()
    {
        destrucciones++;

        // Evita superar el máximo
        if (destrucciones > destruccionNecesarias)
        {
            destrucciones = destruccionNecesarias;
        }

        ActualizarTexto();

        // Cuando llega a 3, aparece el botón
        if (destrucciones >= destruccionNecesarias)
        {
            botonSpawn.gameObject.SetActive(true);
        }
    }

    private void ActualizarTexto()
    {
        textoContador.text = destrucciones + " / " + destruccionNecesarias;
    }

    public void SpawnPrefab()
    {
        if (ObjectoASpawn != null && PuntoSpawn != null)
        {
            Instantiate(ObjectoASpawn, PuntoSpawn.position, PuntoSpawn.rotation);

            // Reiniciar contador después de spawnear

            ActualizarTexto();

            // Ocultar nuevamente el botón
            botonSpawn.gameObject.SetActive(false);
        }
    }
}