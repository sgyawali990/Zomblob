using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform targetTransform;

    public void updateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }
    void Update()
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position + Vector3.up * 2f; // Adjust the height as needed
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180f, 0); // Flip the health bar to face the camera
        }
    }
}
