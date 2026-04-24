using UnityEngine;

public class Sit : MonoBehaviour
{
    public Animator characterAnimator;
    void Start()
    {
        characterAnimator = GetComponent<Animator>();
        characterAnimator.SetTrigger("Sit");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
