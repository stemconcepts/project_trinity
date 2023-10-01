using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static AssemblyCSharp.GenericSkillModel;

namespace AssemblyCSharp
{
    public class Game_Effects_Manager : MonoBehaviour
    {
        //private Task SloMoTask;
        //Vector3 originalCameraPosition;
        //float shakeAmt = 0;
        private Camera mainCamera;
        private Task CancelShake;
        private Vector2 originalForPos;
        private Vector2 originalMidPos;
        private Vector2 originalBackPos;
        private Vector2 originalCamGuidePos;
        public GameObject camGuide;
        public GameObject backGround;
        public GameObject midGround;
        public GameObject foreGround;
        public GameObject lineObject;
        public GameObject SFXCanvas;
        void Awake()
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            SFXCanvas = GameObject.Find("Canvas - SFX");
            if (camGuide && foreGround && midGround && backGround)
            {
                originalCamGuidePos = camGuide.transform.position;
                originalForPos = foreGround.transform.position;
                originalMidPos = midGround.transform.position;
                originalBackPos = backGround.transform.position;
            }
        }

        public void SlowMo(float slowAmount)
        {
            Time.timeScale = slowAmount;
            BattleManager.taskManager.CallSoloMoTask(1f);
        }

        public void PanCamera(bool friendly)
        {
            if (!friendly)
            {
                /*var newBackPos = new Vector2(originalBackPos.x - 1.0f, originalBackPos.y);
                var newMidPos = new Vector2(originalMidPos.x - 0.5f, originalMidPos.y);
                var newforePos = new Vector2(originalForPos.x - 0.2f, originalForPos.y);
                var newCamPos = new Vector2(originalCamGuidePos.x - 3f, originalCamGuidePos.y);*/

                camGuide.transform.DOMoveX(originalCamGuidePos.x - 1f, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    originalCamGuidePos = new Vector2(originalCamGuidePos.x - 1f, originalCamGuidePos.y);
                });
                //MoveGameObject(camGuide, newCamPos, 0.01f, "cameranPan");
                //MoveGameObject(backGround, newBackPos, 0.001f, "backGroundPan");
                //MoveGameObject(midGround, newMidPos, 0.001f, "midGroundPan");
                //MoveGameObject(foreGround, newforePos, 0.001f, "foreGroundPan");
            }
            else
            {
                /*var newBackPos = new Vector2(originalBackPos.x + 1.0f, originalBackPos.y);
                var newMidPos = new Vector2(originalMidPos.x + 0.5f, originalMidPos.y);
                var newforePos = new Vector2(originalForPos.x + 0.2f, originalForPos.y);
                var newCamPos = new Vector2(originalCamGuidePos.x + 3f, originalCamGuidePos.y);*/

                camGuide.transform.DOMoveX(originalCamGuidePos.x + 1f, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    originalCamGuidePos = new Vector2(originalCamGuidePos.x + 1f, originalCamGuidePos.y);
                });
                //MoveGameObject(camGuide, newCamPos, 0.01f, "cameranPan");
                //MoveGameObject(backGround, newBackPos, 0.001f, "backGroundPan");
                // MoveGameObject(midGround, newMidPos, 0.001f, "midGroundPan");
                // MoveGameObject(foreGround, newforePos, 0.001f, "foreGroundPan");
            }
        }

        /*public void MoveGameObject(GameObject gameObject, Vector2 newPositon, float speed, string taskName)
        {
            if (BattleManager.taskManager.taskList.ContainsKey(taskName))
            {
                BattleManager.taskManager.taskList[taskName].Stop();
                BattleManager.taskManager.taskList.Remove(taskName);
            }
            var myTask = new Task(MoveUntil(gameObject, newPositon, speed));
            BattleManager.taskManager.taskList.Add(taskName, myTask);
        }
        IEnumerator MoveUntil(GameObject gameObject, Vector2 newPosition, float speed)
        {
            var time = 0f;
            while ((Vector2)gameObject.transform.position != newPosition)
            {
                float xPos;
                float yPos;
                if (gameObject.transform.position.x > newPosition.x)
                {
                    xPos = gameObject.transform.position.x - (speed - time);
                }
                else if (gameObject.transform.position.x < newPosition.x)
                {
                    xPos = gameObject.transform.position.x + (speed - time);
                }
                else
                {
                    xPos = gameObject.transform.position.x;
                }

                if (gameObject.transform.position.y > newPosition.y)
                {
                    yPos = gameObject.transform.position.y - (speed - time);
                }
                else if (gameObject.transform.position.y < newPosition.y)
                {
                    yPos = gameObject.transform.position.y + (speed - time);
                }
                else
                {
                    yPos = gameObject.transform.position.y;
                }

                gameObject.transform.position = new Vector2(xPos, yPos);
                yield return null;
            }
        }*/

        public void DrawLineFromMouseToPoint(Vector2 position)
        {
            var line = Instantiate(lineObject);
            line.GetComponent<LineRendererController>().SetUpLineFromMouse(position);
        }

        public void FadeOutSpine(SkeletonAnimation skeletonAnimation)
        {
            skeletonAnimation.skeleton.Slots.ForEach(o =>
            {
                BattleManager.taskManager.CallFadeOutSpineTask(o);
            });
        }

        public void FadeOut(MeshRenderer renderer)
        {
            BattleManager.taskManager.CallFadeOutMeshRendererTask(renderer);
        }

        public void ScreenShake(float shakeAmt, int frequency = 0)
        {
            for (var x = 0; x <= frequency; x++)
            {
                float quakeAmt = UnityEngine.Random.value * shakeAmt * 2 - shakeAmt;
                Vector3 pp = camGuide.transform.position;
                pp.y = originalCamGuidePos.y + quakeAmt;
                pp.x = originalCamGuidePos.x + quakeAmt;
                camGuide.transform.position = pp;
                BattleManager.taskManager.CallTask(0.1f, () =>
                    {
                        camGuide.transform.position = originalCamGuidePos;
                    }
                );
            }
        }

        public void TransitionToScene(Animator transition, float transitionTime, Action action)
        {
            transition.SetTrigger("Start");
            MainGameManager.instance.taskManager.CallTask(transitionTime, () =>
            {
                action.Invoke();
                transition.SetTrigger("End");
            });
        }

        public void CallEffectOnTarget(GameObject target, SkillEffect skillEffect, bool flip = false)
        {
            var fxObject = skillEffect.fxObject;
            fxObject.transform.localScale = new Vector2(skillEffect.size, skillEffect.size);
            var transformsInChildren = fxObject.GetComponentsInChildren<Transform>();
            if (transformsInChildren != null)
            {
                transformsInChildren.ToList().ForEach(transform =>
                {
                    transform.localScale = new Vector2(skillEffect.size, skillEffect.size);
                });
            }
            var particleSystems = fxObject.GetComponentsInChildren<ParticleSystemRenderer>().ToList();
            particleSystems.ForEach(o =>
            {
                o.sortingLayerName = "SFX";
                o.sortingOrder = 5;
            });
            if (SFXCanvas == null)
            {
                SFXCanvas = GameObject.Find("Canvas - SFX");
            }
            var fx = Instantiate(fxObject, target.transform.position, new Quaternion(0, flip ? 180 : 0, 0, 0), SFXCanvas.transform /*target.transform*/);
            var particles = fx.GetComponents<ParticleSystem>();
            if (particles != null)
            {
                foreach (var particle in particles)
                {
                    var main = particle.main;
                    main.flipRotation = flip ? 1.0f : 0.0f;
                    main.stopAction = ParticleSystemStopAction.Destroy;
                }
            }
        }

    }
}

