using UnityEngine;

public class DestruirPorContacto : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ObjetoPeligrosos"))
        {
            DestruirObjeto();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ObjetoPeligrosos"))
        {
            DestruirObjeto();
        }
    }

    private void DestruirObjeto()
    {
        // Avisar al ObjectSpawner antes de destruir el objeto
        if (ObjectSpawner.Instance != null)
        {
            ObjectSpawner.Instance.RegistrarDestruccion();
        }

        Destroy(gameObject);
    }
}