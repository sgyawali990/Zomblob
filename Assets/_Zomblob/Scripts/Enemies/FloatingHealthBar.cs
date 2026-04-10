using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private Transform _CameraTransform;
    private bool _isVisible = false;

    private void Awake()
    {
        _CameraTransform = Camera.main.transform;
        gameObject.SetActive(false); // Initialize health bar to inactive
    }
    public void ShowAndUpdate(float currentValue, float maxValue)
    {
        if (!_isVisible)
        {
            gameObject.SetActive(true);
            _isVisible = true;
        }

        slider.value = currentValue / maxValue;
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + _CameraTransform.rotation * Vector3.forward, _CameraTransform.rotation * Vector3.up);
    }
}
