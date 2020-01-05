using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.UI;

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

        public void FadeOutSpine(SkeletonAnimation skeletonAnimation)
        {
            skeletonAnimation.skeleton.Slots.ForEach(o =>
            {
                Battle_Manager.taskManager.CallFadeOutSpineTask(o);
            });
        }

        public void FadeOut(MeshRenderer renderer)
        {
            Battle_Manager.taskManager.CallFadeOutTask(renderer);
        }

        public void ScreenShake( float shakeAmt, int frequency = 0 ){
            for (var x = 0; x <= frequency; x++)
            {
                float quakeAmt = UnityEngine.Random.value * shakeAmt * 2 - shakeAmt;
                Vector3 pp = mainCamera.transform.position;
                pp.y = originalCameraPosition.y + quakeAmt;
                pp.x = originalCameraPosition.x + quakeAmt;
                mainCamera.transform.position = pp;
                Battle_Manager.taskManager.CallTask(0.1f, () =>
                    {
                        mainCamera.transform.position = originalCameraPosition;
                    }
                );
            }
        }
    }
}

