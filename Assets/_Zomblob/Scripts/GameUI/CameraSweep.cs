using System.Collections;
using UnityEngine;

public class CameraSweep : MonoBehaviour
{
    public Transform camA;
    public Transform camB;
    public float duration = 2f;
    private Camera mainCam;

    void Start() => mainCam = Camera.main;

    public void SweepToB()
    {
        StartCoroutine(Sweep(camA, camB));
    }

    IEnumerator Sweep(Transform from, Transform to)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration); // SmoothStep gives ease in/out

            mainCam.transform.position = Vector3.Lerp(from.position, to.position, t);
            mainCam.transform.rotation = Quaternion.Slerp(from.rotation, to.rotation, t);

            yield return null;
        }

        // Snap to exact final position
        mainCam.transform.SetPositionAndRotation(to.position, to.rotation);
    }
}