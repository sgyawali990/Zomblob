using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform targetTransform;


    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (slider != null)
        {
            slider.gameObject.SetActive(false);
        }
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        if (slider != null)
        {
            slider.gameObject.SetActive(true);
        }

        slider.value = currentValue / maxValue;
    }

    void Update()
    {
        if (targetTransform != null && mainCamera != null)
        {
            transform.position = targetTransform.position + Vector3.up * 2f;

            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180f, 0);
        }
        if (slider != null && slider.value <= 0)
        {
            slider.gameObject.SetActive(false);
        }
    }
}
