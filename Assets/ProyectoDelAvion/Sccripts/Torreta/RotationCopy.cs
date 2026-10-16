using UnityEngine;

public class RotationCopy : MonoBehaviour
{
    public Transform objetoPrincipal;

    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, objetoPrincipal.eulerAngles.y,0f);
    }
}