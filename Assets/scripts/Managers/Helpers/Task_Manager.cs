using System;
using UnityEngine;
using System.Collections;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Task_Manager : MonoBehaviour
    {
        //public Base_Character_Manager baseManager;
        public Battle_Details_Manager battleDetailsManager;
        public Dictionary<string, Task> tasks = new Dictionary<string, Task>();
        void Start()
        {
            //baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
        }

        /*public Task_Manager( Battle_Details_Manager bdm ){
            battleDetailsManager = bdm;
        }*/

        public void CallTask(float waitTime, System.Action action = null)
        {
            var myTask = new Task(CountDown(waitTime, action));
        }
        IEnumerator CountDown(float waitTime, System.Action action = null)
        {
            yield return new WaitForSeconds(waitTime);
            if (action != null)
            {
                action();
            }
        }

        public bool RemoveLabelTask(float waitTime, StatusLabelModel statusLabel, System.Action action = null)
        {
            var myTask = new Task(RemoveLabel(waitTime, statusLabel, action));
            return true;
        }
        IEnumerator RemoveLabel(float waitTime, StatusLabelModel statusLabel, System.Action action = null)
        {
            yield return new WaitForSeconds(waitTime);            
            if( statusLabel != null ){
                battleDetailsManager.RemoveLabel(statusLabel);
                if (action != null)
                {
                    action();
                }
            }
        }

        public Task CallChangePointsTask(StatusModel statusModel, System.Action action = null)
        {
            if( statusModel.singleStatus.canStack ){
                statusModel.power = statusModel.power * statusModel.stacks;
            }
            var myTask = new Task(ChangePoints(statusModel, action));
            return myTask;
        }
        IEnumerator ChangePoints(StatusModel statusModel, System.Action action = null)
        {
            var currentStat = statusModel.baseManager.characterManager.GetAttributeValue(statusModel.stat);
            var maxStat = statusModel.baseManager.characterManager.GetAttributeValue("max" + statusModel.stat);
            var damageManager = statusModel.baseManager.characterManager.gameObject.GetComponent<Damage_Manager>();
            while (currentStat <= maxStat && currentStat > 0)
            {
                if (statusModel.regenOn)
                {
                    //statusModel.characterManager.characterModel.incomingHeal = statusModel.power;
                    var damageModel = new DamageModel(statusModel.baseManager){
                        skillSource = statusModel.singleStatus.statusName,
                        incomingHeal = statusModel.power
                    };
                    damageManager.calculateHdamage(damageModel);
                }
                else
                {
                    //damageManager.charDamageModel.incomingDmg = statusModel.power;
                    var damageModel = new DamageModel(statusModel.baseManager){
                        skillSource = statusModel.singleStatus.statusName,
                        incomingDmg = statusModel.power
                    };
                    damageManager.calculatedamage(damageModel);
                }
                yield return new WaitForSeconds(5f);
                if (action != null)
                {
                    action();
                }
            }
        }

        public bool CallDurationTask(float duration, StatusLabelModel statusLabel, System.Action action = null)
        {
            var myTask = new Task(DurationTimer(duration, statusLabel, action));
            return true;
        }
        IEnumerator DurationTimer(float duration, StatusLabelModel statusLabel, System.Action statusAction = null)
        {
            yield return new WaitForSeconds(duration);
            if( statusAction != null ){
                statusAction();
            }
            battleDetailsManager.RemoveLabel( statusLabel );
        }

        public bool CallSoloMoTask(float slowAmount, System.Action action = null)
        {
            var myTask = new Task(SlowMoFade(slowAmount));
            return true;
        }
        IEnumerator SlowMoFade(float waitTime ) {
            while( Time.timeScale < 1f ){
                Time.timeScale += 0.001f;
                yield return null;
            }
            Time.timeScale = 1f;
        }

        public void MoveForwardTask(Base_Character_Manager movingObjectScripts, float movementSpeed, Vector2 targetpos, GameObject dashEffect)
        {
            var myTask = new Task(moveForward( movingObjectScripts, targetpos, dashEffect, movementSpeed));
        }
        IEnumerator moveForward( Base_Character_Manager movingObjectScripts, Vector2 targetPosVar, GameObject dashEffect, float movementSpeedVar ){
            movingObjectScripts.effectsManager.CallEffect( dashEffect, "bottom" );
            while( (Vector2)movingObjectScripts.gameObject.transform.position != targetPosVar && movingObjectScripts.autoAttackManager.isAttacking ){
                float step = movementSpeedVar * Time.deltaTime;
                movingObjectScripts.gameObject.transform.position = Vector2.MoveTowards(movingObjectScripts.gameObject.transform.position, targetPosVar, step);
                yield return null;
            }
        }

        public bool StartMoveTask( Base_Character_Manager movingObjectScripts, float movementSpeed, Vector2 targetpos, Vector2 currentPosition, GameObject dashEffect = null, System.Action action = null)
        {
            var myTask = new Task(StartMove( movingObjectScripts, 0.009f, targetpos, dashEffect, movementSpeed, currentPosition));
            return true;
        }
        public IEnumerator StartMove( Base_Character_Manager movingObjectScripts, float waitTime, Vector2 panelPosVar, GameObject dashEffect, float movementSpeedVar, Vector2 currentPosition ){
            movingObjectScripts.effectsManager.CallEffect( dashEffect, "bottom" );
            while( currentPosition != panelPosVar ){
                float step = movementSpeedVar * Time.deltaTime;
                movingObjectScripts.transform.position = Vector2.MoveTowards(transform.position, panelPosVar, step);
                yield return null;
            }
        }

        public void moveBackTask(Base_Character_Manager movingObjectScripts, float movementSpeed, Vector2 origPosVar, Vector2 currentPosition )
        {
            var myTask = new Task(moveBackward( movingObjectScripts, movementSpeed, origPosVar, currentPosition));
        }
        IEnumerator moveBackward( Base_Character_Manager movingObjectScripts, float movementSpeed, Vector2 origPosVar, Vector2 currentPosition ){
            while( (Vector2)movingObjectScripts.gameObject.transform.position != origPosVar  ){
                float step = movementSpeed * Time.deltaTime;
                movingObjectScripts.gameObject.transform.position = Vector2.MoveTowards(movingObjectScripts.gameObject.transform.position, origPosVar, step);     
                yield return null;
            }
        }

        public bool waitForTargetTask( SkillModel classskill, GameObject character, bool weaponSkill = true ){
            var myTask = new Task(waitForTarget( classskill, character, weaponSkill ));
            tasks.Add("waitForTarget", myTask);
            return true;
        }
        public IEnumerator waitForTarget( SkillModel classSkill, GameObject player, bool weaponSkill = true )
        {
            //waitingForSelection = true;
            Battle_Manager.gameEffectManager.SlowMo(0.01f);
            var target = player.GetComponent<Skill_Manager>().currenttarget;
            var bm = player.GetComponent<Base_Character_Manager>();
            var skillManager = player.GetComponent<Skill_Manager>();
            while( target == null ) {
                if( bm.statusManager.DoesStatusExist("stun") ){
                    skillManager.SkillActiveSet( classSkill, false, skillCancel:true );
                    yield break;
                }
                yield return 0;
            } 
            skillManager.finalTargets.Add( target );
            bm.characterManager.characterModel.target = target.GetComponent<Base_Character_Manager>();
            skillManager.currenttarget = null;
            Time.timeScale = 1f;
            skillManager.SkillComplete( classSkill, skillManager.finalTargets, weaponSkill );
        }

        public bool skillcoolDownTask( SkillModel skill, Image image ){
            var myTask = new Task(skillcoolDownDisplay( skill, image ));
            /*var myTask2 = new Task(CallTask( skill.skillCooldown, () => {
                skill.skillActive = false;
            }));*/

            CallTask( skill.skillCooldown, () => {
                skill.skillActive = false;
            });

            tasks.Add("skillcoolDownDisplay", myTask);
            //tasks.Add("skillcoolDown", myTask2);
            return true;
        }
        IEnumerator skillcoolDownDisplay( SkillModel skill, Image image ){
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



