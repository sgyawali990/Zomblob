using UnityEngine;

public class FireInput : MonoBehaviour
{
    public bool IsFiring { get; private set; }

    void Update()
    {
        IsFiring = Input.GetMouseButton(0);
    }
}