using System;
using UnityEngine;
using System.Collections;
using Spine.Unity;

namespace AssemblyCSharp
{
    public class Task_Manager : BasicManager
    {
        public Battle_Details_Manager battleDetailsManager { get; set; }
        Task task;
        public Task_Manager()
        {
            battleDetailsManager = Battle_Manager.battleDetailsManager;
        }

        public bool CallTask(float waitTime, singleStatus singleStatus = null, System.Action action = null)
        {
            var myTask = new Task(CountDown(waitTime, singleStatus));
            if (action != null)
            {
                action();
            }
            return true;
        }
        IEnumerator CountDown(float waitTime, singleStatus singleStatus)
        {
            yield return new WaitForSeconds(waitTime);            if( singleStatus != null ){
                battleDetailsManager.RemoveLabel(singleStatus.statusName, singleStatus.buff);
            }
        }

        public bool CallTaskBusyAnimation(float waitTime, Animation_Manager animationManager, SkeletonAnimation skeletonAnimation, System.Action action = null)
        {
            var myTask = new Task(busyAnimation(waitTime, animationManager, skeletonAnimation));
            if (action != null)
            {
                action();
            }
            return true;
        }

        IEnumerator busyAnimation(float waitTime, Animation_Manager animationManager, SkeletonAnimation skeletonAnimation )
        {
            yield return new WaitForSeconds(waitTime);
            if( animationManager != null && skeletonAnimation != null ){
                skeletonAnimation.state.AddAnimation(0, "idle", true, 0 );
                animationManager.inAnimation = false;
            }
        }

        public bool CallChangePointsTask(StatusModel statusModel, System.Action action = null)
        {
            if( statusModel.singleStatus.canStack ){
                statusModel.power = statusModel.power * statusModel.stacks;
            }
            var myTask = new Task(ChangePoints(statusModel));
            if (action != null)
            {
                action();
            }
            return true;
        }
        IEnumerator ChangePoints(StatusModel statusModel)
        {
            var currentStat = statusModel.characterManager.GetAttributeValue(statusModel.stat);
            var maxStat = statusModel.characterManager.GetAttributeValue("max" + statusModel.stat);
            var damageManager = statusModel.characterManager.gameObject.GetComponent<Damage_Manager>();
            while (currentStat <= maxStat && currentStat > 0)
            {
                if (statusModel.regenOn)
                {
                    //statusModel.characterManager.characterModel.incomingHeal = statusModel.power;
                    var damageModel = new DamageModel{
                        skillSource = singleStatus.statusName,
                        incomingHeal = statusModel.power
                    };
                    damageManager.calculateHdamage(damageModel);
                }
                else
                {
                    //damageManager.charDamageModel.incomingDmg = statusModel.power;
                    var damageModel = new DamageModel{
                        skillSource = singleStatus.statusName,
                        incomingDmg = statusModel.power
                    };
                    damageManager.calculatedamage(damageModel);
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public bool CallDurationTask(StatusModel statusModel, System.Action action = null)
        {
            var myTask = new Task(DurationTimer(statusModel, action));
            return true;
        }
        IEnumerator DurationTimer(StatusModel statusModel, System.Action statusAction = null)
        {
            yield return new WaitForSeconds(statusModel.duration);
            if( statusAction != null ){
                statusAction();
            }
            battleDetailsManager.RemoveLabel( singleStatus.statusName, singleStatus.buff );
        }

        public bool CallSoloMoTask(float slowAmount, System.Action action = null)
        {
            var myTask = new Task(SlowMoFade(slowAmount, action));
            return true;
        }
        IEnumerator SlowMoFade(float waitTime ) {
            while( Time.timeScale < 1f ){
                Time.timeScale += 0.001f;
                yield return null;
            }
            Time.timeScale = 1f;
        }
    }
}



