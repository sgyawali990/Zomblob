using UnityEngine;
using UnityEngine.UI;

namespace DavidJalbert.LowPolyPeople
{
    public class AnimationController : MonoBehaviour
    {
        // We keep these variables so the Inspector doesn't show "Missing" references
        public Animator[] characters;
        public Text label;
        public Material[] palettes;
        public Camera[] cameras;

        private int currentCamera = 0;

        void Start()
        {
            
        }

        void Update()
        {
            
        }

        public void setAnimation(string tag)
        {
            
        }

        public void randomizePalette()
        {
            // Optional: I left this in case you still want to use the 'R' key to swap colors
            foreach (Animator animator in characters)
            {
                if (animator == null) continue;
                SkinnedMeshRenderer renderer = animator.GetComponentInChildren<SkinnedMeshRenderer>();
                if (renderer != null && palettes.Length > 0)
                {
                    renderer.sharedMaterial = palettes[Random.Range(0, palettes.Length)];
                }
            }
        }

        public void setCamera(int c) { }
        public void changeCamera() { }
    }
}