using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        public void CallEffect( GameObject fxObject, string position = "center" ){
            GameObject fxposition = fxCenter;
            if( position == "bottom" ){
                fxposition = fxBottom;
            } else if ( position == "front" ){
                fxposition = fxFront;
            }
            if( gameObject.tag != "Player" && fxObject ){
                var fx = Instantiate( fxObject, fxposition.transform);
                var particles = fx.GetComponents<ParticleSystem>();
                if( particles != null ){
                    foreach (var particle in particles)
                    {
                        var main = particle.main;
                        main.flipRotation = 1.0f;
                    }
                }
            } else {
                if (fxObject)
                {
                    Instantiate(fxObject, fxposition.transform);
                }
            }   
        }
    
        public void callEffectTarget(BaseCharacterManager target, GameObject fxObject, string position = "center"){
            //fxObject.GetComponent<ParticleSystemRenderer>().sortingLayerName = "SFX";
            var x = fxObject.GetComponentsInChildren<ParticleSystemRenderer>().ToList();
            x.ForEach(o =>
            {
                o.sortingLayerName = "SFX";
            });
            GameObject fxposition = target.gameObject;//.transform.Find("FXpositions").transform.Find("FXcenter").gameObject;
            /*if( position == "bottom" ){
                fxposition = target.gameObject.transform.Find("FXpositions").transform.Find("FXfloor").gameObject;
            } else if ( position == "front" ){
                fxposition = target.gameObject.transform.Find("FXpositions").transform.Find("FXfront").gameObject;
            }*/
            if( target.tag != "Player" ){
                var fx = Instantiate( fxObject, new Vector2 ( fxposition.transform.position.x , fxposition.transform.position.y ), new Quaternion ( 0, 180, 0, 0 ) );
                var particles = fx.GetComponents<ParticleSystem>();
                if( particles != null ){
                    foreach (var particle in particles)
                    {
                        var main = particle.main;
                        main.flipRotation = 1.0f;
                    }
                }
                //fx.transform.SetParent(position.transform);
            } else {
                var fx = Instantiate( fxObject, new Vector2 ( fxposition.transform.position.x , fxposition.transform.position.y ), fxposition.transform.rotation );
                //fx.transform.SetParent(position.transform);
            }   
        }
    }
}

