using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static AssemblyCSharp.GenericSkillModel;

namespace AssemblyCSharp
{
    public class Effects_Manager : MonoBehaviour
    {
        public BaseCharacterManagerGroup baseManager;
        public GameObject fxBottom;
        public GameObject fxFront;
        public GameObject fxCenter;
        public GameObject stompEffect;
        void Start()
        {
            baseManager = this.gameObject.GetComponent<BaseCharacterManagerGroup>();
        }

        public void CallEffect(GameObject fxObject, string position = "center")
        {
            GameObject fxposition = fxCenter;
            if (position == "bottom")
            {
                fxposition = fxBottom;
            }
            else if (position == "front")
            {
                fxposition = fxFront;
            }
            if (gameObject.tag != "Player" && fxObject)
            {
                var fx = Instantiate(fxObject, fxposition.transform);
                var particles = fx.GetComponents<ParticleSystem>();
                if (particles != null)
                {
                    foreach (var particle in particles)
                    {
                        var main = particle.main;
                        main.flipRotation = 1.0f;
                    }
                }
            }
            else
            {
                if (fxObject)
                {
                    Instantiate(fxObject, fxposition.transform);
                }
            }
        }

        public Vector2 ConvertFxPositionToVector(fxPosEnum position)
        {
            switch (position)
            {
                case fxPosEnum.center:
                    return fxCenter.transform.position;
                case fxPosEnum.bottom:
                    return fxBottom.transform.position;
                case fxPosEnum.front:
                    return fxFront.transform.position;
                case fxPosEnum.top:
                    return fxCenter.transform.position;
                default:
                    return fxCenter.transform.position;
            }
        }

        public GameObject GetGameObjectByFXPos(fxPosEnum position)
        {
            switch (position)
            {
                case fxPosEnum.center:
                    return fxCenter;
                case fxPosEnum.bottom:
                    return fxBottom;
                case fxPosEnum.front:
                    return fxFront;
                case fxPosEnum.top:
                    return fxCenter;
                default:
                    return fxCenter;
            }
        }

        public void callEffectTarget(BaseCharacterManager target, GameObject fxObject, fxPosEnum position)
        {
            fxObject.transform.localScale = new Vector2(4, 4);
            var particleSystems = fxObject.GetComponentsInChildren<ParticleSystemRenderer>().ToList();
            particleSystems.ForEach(o =>
            {
                o.sortingLayerName = "SFX";
                o.sortingOrder = 5;
            });
            GameObject fxposition = target.gameObject;
            if (target.tag != "Player")
            {
                var fx = Instantiate(fxObject, target.GetComponent<Effects_Manager>().ConvertFxPositionToVector(position), new Quaternion(0, 180, 0, 0));
                var particles = fx.GetComponents<ParticleSystem>();
                if (particles != null)
                {
                    foreach (var particle in particles)
                    {
                        var main = particle.main;
                        main.flipRotation = 1.0f;
                        main.stopAction = ParticleSystemStopAction.Destroy;
                    }
                }
            }
            else
            {
                var fx = Instantiate(fxObject, target.GetComponent<Effects_Manager>().ConvertFxPositionToVector(position), fxposition.transform.rotation);
                var particles = fx.GetComponents<ParticleSystem>();
                if (particles != null)
                {
                    foreach (var particle in particles)
                    {
                        var main = particle.main;
                        main.stopAction = ParticleSystemStopAction.Destroy;
                    }
                }
            }
        }
    }
}

