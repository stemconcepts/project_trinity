using AssemblyCSharp;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts.Helpers
{
    internal class SpineAnimationHelper : MonoBehaviour
    {
        public SkeletonAnimation skeletonAnimation;
        public SkeletonAnimationMultiOld skeletonAnimationMulti;

        private void Start()
        {
            IdleToggle();
        }

        void IdleToggle()
        {
            if (skeletonAnimation.state.Data.SkeletonData.FindAnimation("idle2") != null && MainGameManager.instance.GetChanceByPercentage(0.2f))
            {
                var duration = PlaySetAnimation("idle2", false);
                PlayAddAnimation("idle", true, duration);
            }
            MainGameManager.instance.taskManager.CallTask(5f, () =>
            {
                IdleToggle();
            });
        }

        float PlaySetAnimation(string animationName, bool loop = false)
        {
            if (skeletonAnimationMulti != null && skeletonAnimationMulti.FindAnimation(animationName) != null)
            {
                var trackEntry = skeletonAnimationMulti.SetAnimation(animationName, loop);
                return trackEntry.Animation.Duration;
            }
            else
            {
                if (skeletonAnimation.state.Data.SkeletonData.Animations.Items.ToList().Any(o => o.Name == animationName))
                {
                    var trackEntry = skeletonAnimation.state.SetAnimation(0, animationName, loop);
                    return trackEntry.Animation.Duration;
                }
                return 0;
            }
        }

        float PlayAddAnimation(string animationName, bool loop = false, float delay = 0)
        {
            if (skeletonAnimationMulti != null && skeletonAnimationMulti.FindAnimation(animationName) != null)
            {
                var trackEntry = skeletonAnimationMulti.AddAnimation(animationName, loop, delay);
                return trackEntry.Animation.Duration;
            }
            else
            {
                if (skeletonAnimation.state.Data.SkeletonData.Animations.Items.ToList().Any(o => o.Name == animationName))
                {
                    var trackEntry = skeletonAnimation.state.AddAnimation(0, animationName, loop, delay);
                    return trackEntry.Animation.Duration;
                }
                return 0;
            }
        }
    }
}
