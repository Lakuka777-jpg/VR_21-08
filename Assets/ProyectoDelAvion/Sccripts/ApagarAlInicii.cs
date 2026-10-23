using UnityEngine;

public class ApagarAlInicii : MonoBehaviour
{
    [SerializeField] private GameObject[] objetosAApagar;

    private void Awake()
    {
        Apagar();
    }

    private void Start()
    {
        Apagar();
    }

    private void Apagar()
    {
        foreach (GameObject obj in objetosAApagar)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}