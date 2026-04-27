using System.Collections;
using UnityEngine;

public class CameraSweep : MonoBehaviour
{
    public Transform camA;
    public Transform camB;
    public Transform camC;
    public float duration = 2f;
    private Camera mainCam;
    private Coroutine currentSweep;

    void Start() => mainCam = Camera.main;

    public void SweepToA()=> StartSweep(camA);
    public void SweepToB()=> StartSweep(camB);
    public void SweepToC()=> StartSweep(camC);


    void StartSweep(Transform target)
    {
        if (currentSweep != null) StopCoroutine(currentSweep);
        currentSweep = StartCoroutine(Sweep(target));
    }


    IEnumerator Sweep(Transform to)
    {
        float elapsed = 0f;
        
        Vector3 startPos = mainCam.transform.position;
        Quaternion startRot = mainCam.transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            mainCam.transform.position = Vector3.Lerp(startPos, to.position, t);
            mainCam.transform.rotation = Quaternion.Slerp(startRot, to.rotation, t);

            yield return null;
        }
        mainCam.transform.SetPositionAndRotation(to.position, to.rotation);
        currentSweep = null;
    }
}