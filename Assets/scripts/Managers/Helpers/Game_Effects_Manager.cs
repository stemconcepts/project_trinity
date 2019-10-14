using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Game_Effects_Manager : MonoBehaviour
    {
        private Task SloMoTask;
        Vector3 originalCameraPosition;
        float shakeAmt = 0;
        private Camera mainCamera;
        private Task CancelShake;

        void Awake()
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            originalCameraPosition = mainCamera.GetComponent<Transform>().position;
        }

        public void SlowMo( float slowAmount ){
            Time.timeScale = slowAmount;
            Battle_Manager.taskManager.CallSoloMoTask(1f);
        }
    
        public void ScreenShake( float shakeAmt ){
            float quakeAmt = UnityEngine.Random.value*shakeAmt*2 - shakeAmt;
            Vector3 pp = mainCamera.transform.position;
            pp.y+= quakeAmt;
            mainCamera.transform.position = pp;
            Battle_Manager.taskManager.CallTask(1f, () =>
                {
                    mainCamera.transform.position = originalCameraPosition;
                }
            );
        }
    }
}

