using UnityEngine;

public class TurretAxisConstraint : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    public Axis freeAxis = Axis.Y;

    private Quaternion initialLocalRotation;

    void Start()
    {
        initialLocalRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        Vector3 euler = transform.localEulerAngles;

        switch (freeAxis)
        {
            case Axis.X:
                euler.y = initialLocalRotation.eulerAngles.y;
                euler.z = initialLocalRotation.eulerAngles.z;
                break;
            case Axis.Y:
                euler.x = initialLocalRotation.eulerAngles.x;
                euler.z = initialLocalRotation.eulerAngles.z;
                break;
            case Axis.Z:
                euler.x = initialLocalRotation.eulerAngles.x;
                euler.y = initialLocalRotation.eulerAngles.y;
                break;
        }

        transform.localRotation = Quaternion.Euler(euler);
    }
}