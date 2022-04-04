using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class EnemyMovementManager : BaseMovementManager
    {
        // Use this for initialization
        void Start()
        {
            baseManager = this.gameObject.GetComponent<EnemyCharacterManagerGroup>();
            if (baseManager.animationManager.skeletonAnimation)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventMove;
            }
            else if (baseManager.animationManager.skeletonAnimationMulti)
            {
                baseManager.animationManager.skeletonAnimationMulti.GetSkeletonAnimations().ForEach(o => {
                    o.state.Event += OnEventMove;
                });
            }
        }
    }
}