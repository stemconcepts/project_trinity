using System;
using UnityEngine;
using System.Collections;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine.UI;
using Spine;
using Assets.scripts.Models.statusModels;

namespace AssemblyCSharp
{
    public class Task_Manager : MainGameTaskManager
    {
        public BattleDetailsManager battleDetailsManager;

        /// <summary>
        /// Used for queue automated actions
        /// </summary>
        /// <param name="task"></param>
        /// <param name="delay"></param>
        public void ScheduleAction(Action task, float delay)
        {
            //Debug.Log("Task Scheduled " + task.Method);
            MainGameManager.instance.actionQueue.Enqueue(DoTask(task, delay));
        }

        IEnumerator DoTask(Action task, float delay)
        {
            //Debug.Log("Inside DoTask Coroutine");
            yield return new WaitForSeconds(delay);
            task();
        }

        public bool RemoveLabelTask(float waitTime, StatusLabelModel statusLabel, System.Action action = null)
        {
            var myTask = new Task(RemoveLabel(waitTime, statusLabel, action));

            if (!taskList.ContainsKey("removeLabel"))
            {
                taskList.Add("removeLabel", myTask);
            }

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

        public void CallChangePointsTask(StatusModel statusModel, System.Action action = null)
        {
            if (statusModel.singleStatus.statusTypeEnum != StatusTypeEnum.ValueOverTime)
            {
                return;
            }

            var currentStat = statusModel.baseManager.characterManager.GetAttributeValue(statusModel.singleStatus.attributeName, statusModel.baseManager.characterManager.characterModel);
            var maxStat = statusModel.baseManager.characterManager.GetAttributeValue("max" + statusModel.singleStatus.attributeName, statusModel.baseManager.characterManager.characterModel);
            var damageManager = statusModel.baseManager.damageManager;
            if (currentStat <= maxStat && currentStat > 0)
            {
                if (statusModel.singleStatus.name == StatusNameEnum.Regen.ToString())
                {
                    var damageModel = new BaseDamageModel()
                    {
                        baseManager = statusModel.baseManager,
                        skillSource = statusModel.singleStatus.name,
                        showExtraInfo = true,
                        incomingHeal = statusModel.power * statusModel.stacks,
                        damageImmidiately = true,
                        fontSize = 150
                    };
                    damageManager.calculateHdamage(damageModel);
                }
                else
                {
                    var damageModel = new BaseDamageModel()
                    {
                        baseManager = statusModel.baseManager,
                        skillSource = statusModel.singleStatus.name,
                        showExtraInfo = true,
                        incomingDmg = statusModel.power * statusModel.stacks,
                        damageImmidiately = true,
                        element = statusModel.singleStatus.element,
                        textColor = statusModel.dmgTextColor,
                        fontSize = 100
                    };
                    damageManager.calculatedamage(damageModel);
                }
                if (action != null)
                {
                    action();
                }
            }

            //var myTask = new Task(ChangePoints(statusModel, action));
            //return myTask;
        }
        IEnumerator ChangePoints(StatusModel statusModel, System.Action action = null)
        {
            var currentStat = statusModel.baseManager.characterManager.GetAttributeValue(statusModel.singleStatus.attributeName, statusModel.baseManager.characterManager.characterModel);
            var maxStat = statusModel.baseManager.characterManager.GetAttributeValue("max" + statusModel.singleStatus.attributeName, statusModel.baseManager.characterManager.characterModel);
            var damageManager = statusModel.baseManager.damageManager;
            while (currentStat <= maxStat && currentStat > 0)
            {
                if (statusModel.singleStatus.name == "regen")
                {
                    var damageModel = new PlayerDamageModel() {
                        baseManager = statusModel.baseManager,
                        skillSource = statusModel.singleStatus.name,
                        incomingHeal = statusModel.power,
                        damageImmidiately = true,
                        fontSize = 100
                    };
                    damageManager.calculateHdamage(damageModel);
                }
                else
                {
                    var damageModel = new PlayerDamageModel(){
                        baseManager = statusModel.baseManager,
                        skillSource = statusModel.singleStatus.name,
                        incomingDmg = statusModel.power,
                        damageImmidiately = true,
                        element = statusModel.singleStatus.element,
                        fontSize = 100
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

        public void CallFadeOutMeshRendererTask(MeshRenderer renderer, System.Action action = null)
        {
            var myTask = new Task(FadeOutRendererFade(renderer));
        }
        IEnumerator FadeOutRendererFade(MeshRenderer renderer)
        {
            var alpha = renderer.material.color.a;
            while (alpha > 0f)
            {
                var newColor = new Color(0, 0, 0, alpha - 0.1f);
                alpha = newColor.a;
                yield return null;
            }
        }

        public void CallFadeOutTextTask(Text text, System.Action action = null)
        {
            var myTask = new Task(FadeOutTextFade(text));
        }
        IEnumerator FadeOutTextFade(Text text)
        {
            var alpha = text.color.a;
            while (alpha > 0f)
            {
                var newColor = new Color(0, 0, 0, alpha - 0.1f);
                alpha = newColor.a;
                yield return null;
            }
        }

        public void CallFadeOutSpineTask(Slot slot, System.Action action = null)
        {
            var myTask = new Task(FadeOutSpineFade(slot));
        }
        IEnumerator FadeOutSpineFade(Slot slot)
        {
            while (slot.A > 0f)
            {
                slot.A -= 0.01f;
                yield return null;
            }
        }

        public void MoveForwardTask(BaseCharacterManagerGroup movingObjectScripts, float movementSpeed, Vector2 targetpos, GameObject effect)
        {
            var myTask = new Task(moveForward( movingObjectScripts, targetpos, effect, movementSpeed));
        }
        IEnumerator moveForward( BaseCharacterManagerGroup movingObjectScripts, Vector2 targetPosVar, GameObject effect, float movementSpeedVar ){
            if (effect)
            {
                movingObjectScripts.effectsManager.CallEffect(effect, "bottom");
            }
            while( (Vector2)movingObjectScripts.gameObject.transform.position != targetPosVar && (movingObjectScripts.autoAttackManager.isAttacking /*|| movingObjectScripts.skillManager.isSkillactive*/) )
            {
                float step = movementSpeedVar * Time.deltaTime;
                movingObjectScripts.gameObject.transform.position = Vector2.MoveTowards(movingObjectScripts.gameObject.transform.position, targetPosVar, step);
                yield return null;
            }
        }

        public void StartMoveTask( BaseCharacterManagerGroup movingObjectScripts, float movementSpeed, Vector2 targetpos, GameObject dashEffect = null, System.Action action = null)
        {
            var myTask = new Task(StartMove( movingObjectScripts, 0.009f, targetpos, dashEffect, movementSpeed));
        }
        public IEnumerator StartMove( BaseCharacterManagerGroup movingObjectScripts, float waitTime, Vector2 panelPosVar, GameObject dashEffect, float movementSpeedVar ){
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

        public void moveBackTask(BaseCharacterManagerGroup movingObjectScripts, float movementSpeed, Vector2 origPosVar, Vector2 currentPosition )
        {
            var myTask = new Task(moveBackward( movingObjectScripts, movementSpeed, origPosVar, currentPosition));
        }
        IEnumerator moveBackward( BaseCharacterManagerGroup movingObjectScripts, float movementSpeed, Vector2 origPosVar, Vector2 currentPosition ){
            while( (Vector2)movingObjectScripts.gameObject.transform.position != origPosVar  ){
                float step = movementSpeed * Time.deltaTime;
                movingObjectScripts.gameObject.transform.position = Vector2.MoveTowards(movingObjectScripts.gameObject.transform.position, origPosVar, step);     
                yield return null;
            }
        }

        public bool waitForTargetTask( GameObject character, SkillModel classSkill = null, bool weaponSkill = true, System.Action skillAction = null)
        {
            BattleManager.waitingForSkillTarget = true;
            BattleManager.offensiveSkill = classSkill.enemy;
            var myTask = new Task(waitForTarget( classSkill, character, weaponSkill, skillAction));
            if (!taskList.ContainsKey("waitForTarget"))
            {
                taskList.Add("waitForTarget", myTask);
            }
            return true;
        }
        public IEnumerator waitForTarget( SkillModel classSkill, GameObject player, bool weaponSkill = true, System.Action skillAction = null)
        {
            BattleManager.gameEffectManager.SlowMo(0.05f);
            var bm = player.GetComponent<CharacterManagerGroup>();
            var target = bm.skillManager.currenttarget;
            while( target == null ) {
                //target = player.GetComponent<PlayerSkillManager>().currenttarget;
                target = bm.skillManager.currenttarget;
                yield return 0;
            }
            BattleManager.waitingForSkillTarget = false;
            bm.skillManager.finalTargets.Add( target );
            skillAction?.Invoke();
            Time.timeScale = 1f;
        }

        public void TimerDisplayTask(float time, Image image, string taskName = null)
        {
            var myTask = new Task(TimerDisplay(time, image));
            if(!string.IsNullOrEmpty(taskName) && !taskList.ContainsKey(taskName))
            {
                taskList.Add(taskName, myTask);
            }
        }
        IEnumerator TimerDisplay(float time, Image image)
        {
            var timeRemaining = 0f;
            image.fillAmount = 1f;
            while (time > timeRemaining && image)
            { 
                yield return new WaitForSeconds(1f);
                timeRemaining += 1f;
                float timeSpent = 1f / time;
                image.fillAmount -= timeSpent;
            }
        }

        public IEnumerator CompareTurnsAndAction(int runAtTurn, BattleManager.TurnEnum turn, Action actionAtTheEnd)
        {
            while (BattleManager.turnCount <= runAtTurn)
            {
                if (BattleManager.turn == turn && BattleManager.turnCount >= runAtTurn)
                {
                    actionAtTheEnd();
                    yield break;
                }
                yield return null;
            }
        }

        public IEnumerator CompareTurnsAndActionDuring(int runTillTurn, BattleManager.TurnEnum turn, Action actionDuring)
        {
            var actioned = true;
            while (BattleManager.turnCount <= runTillTurn)
            {
                if (BattleManager.turn == turn && !actioned)
                {
                    actionDuring();
                    actioned = true;
                }
                else if(BattleManager.turn != turn)
                {
                    actioned = false;
                }
                yield return null;
            }
        }
    }
}



