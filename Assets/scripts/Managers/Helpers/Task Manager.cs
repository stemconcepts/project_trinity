using System;
using UnityEngine;
using System.Collections;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Task_Manager : Base_Character_Manager
    {
        public Battle_Details_Manager battleDetailsManager { get; set; }
        public Dictionary<string, Task> tasks = new Dictionary<string, Task>();
        public Task_Manager()
        {
            battleDetailsManager = Battle_Manager.battleDetailsManager;
        }

        public bool CallTask(float waitTime, System.Action action = null)
        {
            var myTask = new Task(CountDown(waitTime));
            if (action != null)
            {
                action();
            }
            return true;
        }
        IEnumerator CountDown(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
        }

        public bool RemoveLabelTask(float waitTime, singleStatus singleStatus, System.Action action = null)
        {
            var myTask = new Task(RemoveLabel(waitTime, singleStatus));
            if (action != null)
            {
                action();
            }
            return true;
        }
        IEnumerator RemoveLabel(float waitTime, singleStatus singleStatus)
        {
            yield return new WaitForSeconds(waitTime);            
            if( singleStatus != null ){
                battleDetailsManager.RemoveLabel(singleStatus.statusName, singleStatus.buff);
            }
        }

        public bool CallTaskBusyAnimation(float waitTime, Animation_Manager animationManager, System.Action action = null)
        {
            var myTask = new Task(busyAnimation(waitTime, animationManager));
            if (action != null)
            {
                action();
            }
            return true;
        }

        IEnumerator busyAnimation(float waitTime, Animation_Manager animationManager )
        {
            yield return new WaitForSeconds(waitTime);
            if( animationManager != null && animationManager.skeletonAnimation != null ){
                animationManager.skeletonAnimation.state.AddAnimation(0, "idle", true, 0 );
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

        public bool MoveForwardTask(float movementSpeed, Vector2 targetpos, GameObject dashEffect, System.Action action = null)
        {
            var myTask = new Task(moveForward( 0.009f, targetpos, dashEffect, movementSpeed));
            return true;
        }
        IEnumerator moveForward( float waitTime, Vector2 targetPosVar, GameObject dashEffect, float movementSpeedVar ){
            Vector3 currentPosition = transform.position;
            effectsManager.CallEffect( dashEffect, "bottom" );
            while( characterManager.characterModel.isAttacking ){
                float step = movementSpeedVar * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, targetPosVar, step);
                yield return null;
            }
        }

        public bool StartMoveTask(float movementSpeed, Vector2 targetpos, GameObject dashEffect, Vector2 currentPosition, System.Action action = null)
        {
            var myTask = new Task(StartMove( 0.009f, targetpos, dashEffect, movementSpeed, currentPosition));
            return true;
        }
        public IEnumerator StartMove( float waitTime, Vector2 panelPosVar, float movementSpeedVar, Vector2 currentPosition ){
            while( currentPosition != panelPosVar ){
                float step = movementSpeedVar * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, panelPosVar, step);
                yield return null;
            }
        }

        public bool moveBackTask(float waitTime, Vector2 origPosVar, Vector2 currentPosition )
        {
            var myTask = new Task(moveBackward( 0.009f, origPosVar, currentPosition));
            return true;
        }
        IEnumerator moveBackward( float waitTime, Vector2 origPosVar, Vector2 currentPosition ){
            while( currentPosition != origPosVar  ){
                float step = 70f * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, origPosVar, step);     
                yield return null;
            }
        }

        public bool waitForTargetTask( classSkills classskill, GameObject character ){
            var myTask = new Task(waitForTarget( classskill, character));
            tasks.Add("waitForTarget", myTask);
            return true;
        }
        public IEnumerator waitForTarget( classSkills classSkill, GameObject player, bool weaponSkill = true )
        {
            //waitingForSelection = true;
            Game_Effects_Manager.SlowMo(0.01f);
            var target = player.GetComponent<Skill_Manager>().currenttarget;
            var characterManager = player.GetComponent<Character_Manager>();
            var skillManager = player.GetComponent<Skill_Manager>();
            while( target == null ) {
                if( characterManager.statusManager.DoesStatusExist("stun") ){
                    skillManager.SkillActiveSet( classSkill, false, true );
                    yield break;
                }
                yield return 0;
            } 
            skillManager.finalTargets.Add( target );
            characterManager.characterModel.target = target.GetComponent<Character_Manager>();
            skillManager.currenttarget = null;
            Time.timeScale = 1f;
            skillManager.SkillComplete( classSkill, skillManager.finalTargets, weaponSkill, player: player );
        }

        public bool skillcoolDownTask( classSkills skill, Image image ){
            var myTask = new Task(skillcoolDownDisplay( skill, image ));
            var myTask2 = new Task(CallTask( skill.skillCooldown, () => {
                skill.skillActive = false;
            }));
            tasks.Add("skillcoolDownDisplay", myTask);
            tasks.Add("skillcoolDown", myTask2);
            return true;
        }
        IEnumerator skillcoolDownDisplay( classSkills skill, Image image ){
            image.fillAmount = 1f;
            while( skill.skillActive ){
                yield return new WaitForSeconds(1f);
                skill.currentCDAmount += 1f;
                float timeSpent;
                timeSpent = 1f/skill.skillCooldown;
                image.fillAmount -= timeSpent;
            }
            skill.currentCDAmount = 0f;
        }
    }
}



