﻿using System;
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
        public Dictionary<string, Task> taskList = new Dictionary<string, Task>();
        void Start()
        {
            //baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
        }

        /*public Task_Manager( Battle_Details_Manager bdm ){
            battleDetailsManager = bdm;
        }*/

        public void CallTask(float waitTime, System.Action action = null, string taskName = null)
        {
            var myTask = new Task(CountDown(waitTime, action));
            if (!string.IsNullOrEmpty(taskName) && !taskList.ContainsKey(taskName))
            {
                taskList.Add(taskName, myTask);
            }
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
                statusModel.power = statusModel.stacks > 0 ? statusModel.power * statusModel.stacks : statusModel.power;
            }
            var myTask = new Task(ChangePoints(statusModel, action));
            return myTask;
        }
        IEnumerator ChangePoints(StatusModel statusModel, System.Action action = null)
        {
            var currentStat = statusModel.baseManager.characterManager.GetAttributeValue(statusModel.singleStatus.attributeName);
            var maxStat = statusModel.baseManager.characterManager.GetAttributeValue("max" + statusModel.singleStatus.attributeName);
            var damageManager = statusModel.baseManager.damageManager;
            while (currentStat <= maxStat && currentStat > 0)
            {
                if (statusModel.singleStatus.statusName == "regen")
                {
                    //statusModel.characterManager.characterModel.incomingHeal = statusModel.power;
                    var damageModel = new DamageModel(){
                        baseManager = statusModel.baseManager,
                        skillSource = statusModel.singleStatus.statusName,
                        incomingHeal = statusModel.power,
                        damageImmidiately = true
                    };
                    damageManager.calculateHdamage(damageModel);
                }
                else
                {
                    //damageManager.charDamageModel.incomingDmg = statusModel.power;
                    var damageModel = new DamageModel(){
                        baseManager = statusModel.baseManager,
                        skillSource = statusModel.singleStatus.statusName,
                        incomingDmg = statusModel.power,
                        damageImmidiately = true,
                        //useResistances = statusModel.singleStatus.element != elementType.none,
                        element = statusModel.singleStatus.element
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

        public Task CallDurationTask(float duration, StatusLabelModel statusLabel, string taskName = null, System.Action action = null)
        {
            var myTask = new Task(DurationTimer(duration, statusLabel, action));
            if (!string.IsNullOrEmpty(taskName) && !taskList.ContainsKey(taskName))
            {
                taskList.Add(taskName, myTask);
            }
            return myTask;
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

        public void MoveForwardTask(Base_Character_Manager movingObjectScripts, float movementSpeed, Vector2 targetpos, GameObject effect)
        {
            var myTask = new Task(moveForward( movingObjectScripts, targetpos, effect, movementSpeed));
        }
        IEnumerator moveForward( Base_Character_Manager movingObjectScripts, Vector2 targetPosVar, GameObject effect, float movementSpeedVar ){
            if (effect)
            {
                movingObjectScripts.effectsManager.CallEffect(effect, "bottom");
            }
            while( (Vector2)movingObjectScripts.gameObject.transform.position != targetPosVar && (movingObjectScripts.autoAttackManager.isAttacking || movingObjectScripts.skillManager.isSkillactive) )
            {
                float step = movementSpeedVar * Time.deltaTime;
                movingObjectScripts.gameObject.transform.position = Vector2.MoveTowards(movingObjectScripts.gameObject.transform.position, targetPosVar, step);
                yield return null;
            }
        }

        public void StartMoveTask( Base_Character_Manager movingObjectScripts, float movementSpeed, Vector2 targetpos, GameObject dashEffect = null, System.Action action = null)
        {
            var myTask = new Task(StartMove( movingObjectScripts, 0.009f, targetpos, dashEffect, movementSpeed));
        }
        public IEnumerator StartMove( Base_Character_Manager movingObjectScripts, float waitTime, Vector2 panelPosVar, GameObject dashEffect, float movementSpeedVar ){
            if (dashEffect != null)
            {
                movingObjectScripts.effectsManager.CallEffect(dashEffect, "bottom");
            }
            while((Vector2)movingObjectScripts.gameObject.transform.position != panelPosVar && movingObjectScripts.autoAttackManager.isAttacking)
            {
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

        public bool waitForTargetTask( GameObject character, SkillModel classSkill = null, bool weaponSkill = true, System.Action skillAction = null)
        {
            Battle_Manager.waitingForSkillTarget = true;
            Battle_Manager.offensiveSkill = classSkill.enemy;
            var myTask = new Task(waitForTarget( classSkill, character, weaponSkill, skillAction));
            if (!taskList.ContainsKey("waitForTarget"))
            {
                taskList.Add("waitForTarget", myTask);
            }
            return true;
        }
        public IEnumerator waitForTarget( SkillModel classSkill, GameObject player, bool weaponSkill = true, System.Action skillAction = null)
        {
            Battle_Manager.gameEffectManager.SlowMo(0.01f);
            var bm = player.GetComponent<Base_Character_Manager>();
            var target = bm.skillManager.currenttarget;
            while( target == null ) {
                if( bm.statusManager.DoesStatusExist("stun") ){
                    bm.skillManager.SkillActiveSet( classSkill, false, skillCancel:true );
                    yield break;
                }
                target = player.GetComponent<Skill_Manager>().currenttarget;
                yield return 0;
            }
            Battle_Manager.waitingForSkillTarget = false;
            bm.skillManager.finalTargets.Add( target );
            //bm.characterManager.characterModel.target = target.GetComponent<Base_Character_Manager>();
            //bm.skillManager.currenttarget = null;
            if (skillAction != null)
            {
                skillAction();
            }
            Time.timeScale = 1f;
            //bm.skillManager.SkillComplete(bm.skillManager.finalTargets, skillModel : classSkill, weaponSkill : weaponSkill);
        }

        public bool skillcoolDownTask( SkillModel skill, Image image ){
            var myTask = new Task(skillcoolDownDisplay( skill, image ));
            /*var myTask2 = new Task(CallTask( skill.skillCooldown, () => {
                skill.skillActive = false;
            }));*/

            CallTask( skill.skillCooldown, () => {
                skill.skillActive = false;
            });
            if (!taskList.ContainsKey("skillcoolDownDisplay_" + skill.skillName))
            {
                taskList.Add("skillcoolDownDisplay_" + skill.skillName, myTask);
            }
            return true;
        }
        IEnumerator skillcoolDownDisplay( SkillModel skill, Image image ){
            image.fillAmount = 1f;
            while( skill.skillActive ){
                yield return new WaitForSeconds(1f);
                skill.currentCDAmount += 1f;
                float timeSpent;
                timeSpent = 1f/skill.skillCooldown;
                Battle_Manager.battleInterfaceManager.ForEach((o =>
                    {
                        if (o.skill == skill)
                        {
                            image.fillAmount -= timeSpent;
                        }
                    }
                ));
            }
            skill.currentCDAmount = 0f;
        }

        public bool skillcoolDownTask(enemySkill skill, Image image)
        {
            var myTask = new Task(skillcoolDownDisplay(skill, image));
            /*var myTask2 = new Task(CallTask( skill.skillCooldown, () => {
                skill.skillActive = false;
            }));*/

            CallTask(skill.skillCooldown, () => {
                skill.skillActive = false;
            });

            if (!taskList.ContainsKey("skillcoolDownDisplay_" + skill.skillName))
            {
                taskList.Add("skillcoolDownDisplay_" + skill.skillName, myTask);
            }
            return true;
        }
        IEnumerator skillcoolDownDisplay(enemySkill skill, Image image)
        {
            image.fillAmount = 1f;
            while (skill.skillActive)
            {
                yield return new WaitForSeconds(1f);
                skill.currentCDAmount += 1f;
                //float timeSpent;
                //timeSpent = 1f / skill.skillCooldown;
                //image.fillAmount -= timeSpent;
            }
            skill.currentCDAmount = 0f;
        }
    }
}


