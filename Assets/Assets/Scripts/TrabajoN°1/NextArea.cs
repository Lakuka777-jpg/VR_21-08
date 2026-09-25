using UnityEngine;

public class NextArea : MonoBehaviour
{
    public GameObject ObjectToActivate;

    public void SpawnObject()
    {
        if (ObjectToActivate != null)
        {
            ObjectToActivate.SetActive(true);
        }
        else
        {
            Debug.Log("NO");
        }
    }
}
