using UnityEngine;

    public class FPSLimiter : MonoBehaviour
    {
        public int targetFPS = 60; // Set your desired target FPS here

        void Awake()
        {
            // Disable VSync to ensure Application.targetFrameRate takes effect
            QualitySettings.vSyncCount = 0; 
            Application.targetFrameRate = targetFPS;
        }

        // You can also add a method to change the target FPS during runtime
        public void SetTargetFPS(int newFPS)
        {
            targetFPS = newFPS;
            Application.targetFrameRate = targetFPS;
        }
    }