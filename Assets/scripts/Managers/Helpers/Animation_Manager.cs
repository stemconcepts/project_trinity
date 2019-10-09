using System;
using Spine;
using UnityEngine;
using Spine.Unity;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Animation_Manager : MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        public SkeletonAnimation skeletonAnimation;
        public MeshRenderer meshRenderer;
        public bool inAnimation;
        public string idleAnim;

        void Awake()
        {
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
            meshRenderer = transform.Find("Animations").GetComponent<MeshRenderer>();
        }

        public void SetSortingLayer(int sortingLayer ){
            meshRenderer.sortingOrder = sortingLayer;
        }

        public void AddStatusAnimation( bool addStatus, string animName, string holdAnim, bool animHold ){
            if( addStatus ){
                if( animName == "toDeath" ){
                    skeletonAnimation.state.SetAnimation( 0, "toDeath", false);
                } else {
                    baseManager.damageManager.charDamageModel.hitAnimation = animName;
                    if( animHold ){
                        baseManager.damageManager.charDamageModel.animationHold = animHold;
                    } else {
                        baseManager.damageManager.charDamageModel.holdAnimation = holdAnim;
                    }
                }
            } else {
                skeletonAnimation.state.SetAnimation(0, "stunToIdle", true );
                baseManager.damageManager.charDamageModel.hitAnimation = "";       
                baseManager.damageManager.charDamageModel.animationHold = addStatus;
                var animationDuration = skeletonAnimation.state.SetAnimation(0, "stunToIdle", false).Animation.duration;
                Battle_Manager.taskManager.CallTaskBusyAnimation( animationDuration, this );
            }
        }
    }
}

