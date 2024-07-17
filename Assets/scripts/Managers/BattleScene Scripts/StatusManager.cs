using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.scripts.Models.statusModels;
using static UnityEngine.Rendering.DebugUI;
//using UnityEditor.PackageManager;

namespace AssemblyCSharp
{
    public class StatusManager : MonoBehaviour
    {
        public BaseCharacterManagerGroup baseManager;
        public BattleDetailsManager battleDetailsManager;
        public GameObject buffHolder;
        public GameObject debuffHolder;
        [Header("Immunity List:")]
        public List<SingleStatusModel> immunityList = new List<SingleStatusModel>();
        [Header("Status List:")]
        public List<SingleStatusModel> singleStatusList = new List<SingleStatusModel>();

        //Status on the character to run at the start of turn
        private List<StatusModel> activeStatuses = new List<StatusModel>();

        void Awake(){
            baseManager = this.gameObject.GetComponent<BaseCharacterManagerGroup>();
        }

        void Start()
        {
            singleStatusList = MainGameManager.instance.StatusFinder.AllStatuses;
            battleDetailsManager = BattleManager.battleDetailsManager;
        }

        void Update()
        {
            
        }

        public void RunStatusActions(BattleManager.TurnEnum relevantTurn)
        {
            if (BattleManager.turn == relevantTurn)
            {
                foreach (var status in activeStatuses.ToList())
                {
                    CheckAndRunDot(status, relevantTurn);
                }
            }
        }

        public Task ResetOnValidTurn(StatusModel statusModel, Action action = null)
        {
            var turn = this.gameObject.tag.ToLower() == "enemy" ? BattleManager.TurnEnum.EnemyTurn : BattleManager.TurnEnum.PlayerTurn;
            var myTask = new Task(BattleManager.taskManager.CompareTurnsAndAction(statusModel.turnToReset, turn, () =>
            {                
                if (baseManager.characterManager.characterModel.isAlive)
                {
                    if (this != null)
                    {
                        if (action != null)
                        {
                            action();
                        }
                    }
                }
            }));

            if (!BattleManager.taskManager.taskList.ContainsKey("reset"))
            {
                BattleManager.taskManager.taskList.Add("reset", myTask);
            }

            return myTask;
        }

        //For temporary buffs to stats
        public void AddTemporaryStats( StatusModel statusModel ){
            var currentStat = this.gameObject.tag.ToLower() == "enemy" ? GetComponent<EnemyCharacterManager>().GetAttributeValue(statusModel.singleStatus.attributeName, this.baseManager.characterManager.characterModel) : 
                GetComponent<CharacterManager>().GetAttributeValue(statusModel.singleStatus.attributeName, this.baseManager.characterManager.characterModel);
            var originalStat = this.gameObject.tag.ToLower() == "enemy" ? GetComponent<EnemyCharacterManager>().GetAttributeValue("original" + statusModel.singleStatus.attributeName, this.baseManager.characterManager.characterModel) : 
                GetComponent<CharacterManager>().GetAttributeValue("original" + statusModel.singleStatus.attributeName, this.baseManager.characterManager.characterModel);

            float statValue = 0.0f;
            if( statusModel.singleStatus.name == StatusNameEnum.Thorns.ToString() ){
                statValue = (float)Math.Ceiling((statusModel.power / BattleManager.ThornsWeight) + originalStat);
            }else if(statusModel.isFlat)
            {
                statValue = statusModel.power;
            }else{
                statValue = (float)Math.Ceiling((statusModel.power / 100) * originalStat);
            }
            float newStat;
            if( !statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName))
            {
                battleDetailsManager.ShowLabel(statusModel, statusModel.singleStatus.buff ? buffHolder : debuffHolder);
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                if ( statusModel.singleStatus.buff ){
                    newStat = currentStat + statValue;
                } else {
                    newStat = currentStat - statValue;
                }
                //set New stat to character
                this.baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, newStat, this.baseManager.characterManager.characterModel);
                statusModel.SaveTurnToReset();
                statusModel.effectOnEnd = () => 
                    this.baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, originalStat, this.baseManager.characterManager.characterModel);

                //Add status to active list
                activeStatuses.Add(statusModel);

                /*statusLabel.durationTimer = ResetOnValidTurn(statusModel);
                statusLabel.durationTimer.Finished += (t) => EndStatusAndTriggerEffectsOnEvent(t, statusModel, () =>
                {
                    this.baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, originalStat, this.baseManager.characterManager.characterModel);
                });*/
            } else 
            if(statusModel.turnOff){
                EndStatusAndTriggerEffectsOnEvent(true, statusModel, () =>
                 baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, originalStat, baseManager.characterManager.characterModel));
            }
        }

        //Add to stat permanently
        public void AddPermanentStats(StatusModel statusModel)
        {
            var attributeName = statusModel.singleStatus.attributeName;
            var originalStat = baseManager.characterManager.GetAttributeValue("original" + statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel);
            if (!statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName))
            {
                battleDetailsManager.ShowLabel(statusModel, statusModel.singleStatus.buff ? buffHolder : debuffHolder);
                if (statusModel.singleStatus.attributeName != null)
                {
                    baseManager.characterManager.SetAttribute(attributeName, statusModel.power, baseManager.characterManager.characterModel);
                }
            }
            else
            if (statusModel.turnOff)
            {
                EndStatusAndTriggerEffectsOnEvent(true, statusModel, () =>
                 baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, originalStat, baseManager.characterManager.characterModel));
            }
        }

        private void CheckAndRunDot(StatusModel statusModel, BattleManager.TurnEnum relevantTurn)
        {
            //stop status effect if turn to reset is greater than turn count
            if (statusModel.turnToReset <= BattleManager.turnCount && statusModel.turnToReset != 0)
            {
                EndStatusAndTriggerEffectsOnEvent(false, statusModel);
                return;
            }

            if(BattleManager.turnCount <= (BattleManager.turnCount + statusModel.turnDuration * 2) && BattleManager.turn == relevantTurn)
            {
                 StatusLabelModel currentStatusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                 if (baseManager.characterManager.characterModel.isAlive && statusModel != null && currentStatusLabel)
                 {
                     statusModel.stacks = currentStatusLabel.stacks;
                     BattleManager.taskManager.CallChangePointsTask(statusModel);
                 }
            };
        }

        public void EndStatusAndTriggerEffectsOnEvent(bool stoppedManually, StatusModel statusModel, Action actionOnStop = null)
        {
            if (stoppedManually)
            {
                ForceStatusOff(statusModel.singleStatus);
                Debug.Log($"{statusModel.singleStatus.name} timer stopped");
            }
            else
            {
                ForceStatusOff(statusModel.singleStatus);
                Debug.Log($"{statusModel.singleStatus.name} timer ended");

                //Run an effect at the end if there is one
                statusModel.effectOnEnd?.Invoke();
            }
            activeStatuses.Remove(statusModel);
            actionOnStop?.Invoke();
        }


        //Stat changes - use for Ticking Stat changes 
        public void AddTickingStatChanges(StatusModel statusModel)
        {
            if (!statusModel.turnOff)
            {
                if (!DoesStatusExist(statusModel.singleStatus.statusName))
                {
                    battleDetailsManager.ShowLabel(statusModel, statusModel.singleStatus.buff ? buffHolder : debuffHolder);
                    statusModel.SaveTurnToReset();

                    //Add status to active list
                    activeStatuses.Add(statusModel);
                }
                else
                {
                    StatusLabelModel currentStatusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                    if (currentStatusLabel != null && statusModel.singleStatus.canStack)
                    {
                        currentStatusLabel.stacks = currentStatusLabel.stacks < statusModel.singleStatus.maxStacks ? currentStatusLabel.stacks + 1 : currentStatusLabel.stacks;
                        battleDetailsManager.AddStacks(currentStatusLabel);
                        statusModel.power += currentStatusLabel.buffPower;
                        currentStatusLabel.buffPower = statusModel.power;
                    }
                    statusModel.SaveTurnToReset();
                }
            }
            else
            {
                EndStatusAndTriggerEffectsOnEvent(true, statusModel);
            }
        }

        public bool DoesStatusExist(StatusNameEnum statusName){
            Transform statusPanel;
            SingleStatusModel status;
            for (int i = 0; i < singleStatusList.Count; i++)
            {
                if (singleStatusList[i].statusName == statusName)
                {
                    status = singleStatusList[i];
                    if (status.buff)
                    {
                        statusPanel = buffHolder.transform;
                    }
                    else
                    {
                        statusPanel = debuffHolder.transform;
                    }
                    int statusCount = statusPanel.childCount;
                    for (int x = 0; x < statusCount; x++)
                    {
                        var statusLabelScript = statusPanel.GetChild(x).GetComponent<StatusLabelModel>();
                        if (statusLabelScript.statusName == statusName)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets possible statuses that can exist on the character
        /// </summary>
        /// <param name="statusName"></param>
        /// <returns></returns>
        public SingleStatusModel GetStatus(StatusNameEnum statusName)
        {
            for( int i = 0; i < singleStatusList.Count; i++ ){            
                if( singleStatusList[i].statusName == statusName  ){
                    return singleStatusList[i];
                }   
            }
            return null;
        }

        /// <summary>
        /// Gets a status if it is currently Active on the character
        /// </summary>
        /// <param name="statusName"></param>
        /// <returns></returns>
        public StatusLabelModel GetStatusIfExist(StatusNameEnum statusName)
        {
            Transform statusPanel;
            SingleStatusModel status;
            for( int i = 0; i < singleStatusList.Count; i++ ){            
                if(singleStatusList[i].statusName == statusName)
                {
                    status = singleStatusList[i];
                    if (status.buff)
                    {
                        statusPanel = buffHolder != null ? buffHolder.transform : null;
                    }
                    else
                    {
                        statusPanel = debuffHolder != null ? debuffHolder.transform : null;
                    }
                    if (statusPanel != null)
                    {
                        int statusCount = statusPanel.childCount;
                        for (int x = 0; x < statusCount; x++)
                        {
                            var statusLabelScript = statusPanel.GetChild(x).GetComponent<StatusLabelModel>();
                            if (statusLabelScript.statusName == statusName)
                            {
                                return statusLabelScript;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public List<StatusLabelModel> GetAllStatusIfExist( bool buff ){
            List<StatusLabelModel> currentStatusList = new List<StatusLabelModel>();
            Transform statusPanel;
            if( buff ){
                statusPanel = buffHolder.transform;
            } else {
                statusPanel = debuffHolder.transform;
            }
            int statusCount = statusPanel.childCount;
            for( int x = 0; x < statusCount; x++ ){
                currentStatusList.Add( statusPanel.GetChild(x).GetComponent<StatusLabelModel>() );
            }
            return currentStatusList;
        }   

        //check immunity
        public bool CheckImmunity( string statusName ){
            if (String.IsNullOrEmpty(statusName))
            {
                return false;
            }
            for (int i = 0; i < immunityList.Count; i++) {
                if( immunityList[i].name == statusName ){
                    return true;
                }
            }
            return false;
        }

        public void ForceStatusOff(SingleStatusModel singleStatus, System.Action statusAction = null)
        {
            if (statusAction != null)
            {
                statusAction();
            }
            battleDetailsManager.RemoveLabel(GetStatusIfExist(singleStatus.statusName));
            if (singleStatus.hitAnim != animationOptionsEnum.none)
            {
                var animationManager = GetComponent<Animation_Manager>();
                animationManager.AddStatusAnimation(false, singleStatus.hitAnim, singleStatus.holdAnim);
            }
        }

        public void StatusOn( StatusModel statusModel ){
            if(!statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName) && !CheckImmunity(statusModel.singleStatus.attributeName))
            {
                battleDetailsManager.ShowLabel(statusModel, statusModel.singleStatus.buff ? buffHolder : debuffHolder);
                if (statusModel.singleStatus.hitAnim != animationOptionsEnum.none && statusModel.singleStatus.holdAnim != animationOptionsEnum.none)
                {
                    baseManager.animationManager.AddStatusAnimation(true, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                }
                statusModel.SaveTurnToReset();

                //Add status to active list
                activeStatuses.Add(statusModel);
            } 
            else if( statusModel.turnOff ){
                EndStatusAndTriggerEffectsOnEvent(true, statusModel);
            }
        }

        //Tumor set Timer then do damage
        public void Tumor( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName) ){ 
                battleDetailsManager.ShowLabel( statusModel, statusModel.singleStatus.buff ? buffHolder : debuffHolder);
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                if ( statusModel.singleStatus.hitAnim != animationOptionsEnum.none)
                {
                    baseManager.animationManager.AddStatusAnimation( true, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                }
                statusModel.SaveTurnToReset();

                statusModel.effectOnEnd = () => 
                    baseManager.damageManager.DoDamage((int)statusLabel.buffPower, baseManager.characterManager, isMagic: true);

                //Add status to active list
                activeStatuses.Add(statusModel);
            } else 
            if(statusModel.turnOff)
            {
                EndStatusAndTriggerEffectsOnEvent(true, statusModel);
            }
        }

        public void OnTakeDmg( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName)){ 
                battleDetailsManager.ShowLabel(statusModel, statusModel.singleStatus.buff ? buffHolder : debuffHolder);
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                var attrValue = baseManager.characterManager.GetAttributeValue(statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel);

                /*var effect = ScriptableObject.CreateInstance<EffectOnEventModel>();
                //effect.power = statusModel.power;
                //effect.turnDuration = statusModel.turnDuration;
                effect.trigger = EventTriggerEnum.OnTakingDmg.ToString();
                effect.triggerChance = 1; //comes from status
                //effect.ready = true; //statusModel.singleStatus.active
                //effect.effect = EffectOnEventModel.effectGrp.Status;
                effect.eventAction = () => RunStatusMethodFromType(statusModel, statusModel.singleStatus.selectedStatusFunction);
                //effect.affectSelf = true; //comes from status
                effect.owner = this.gameObject;
                //effect.coolDown = 0;

                BattleManager.eventManager.EventAction += effect.RunEffectAction;*/

                baseManager.EventManagerV2.AddDelegateToEvent(EventTriggerEnum.OnTakingDmg, () => RunStatusMethodFromType(statusModel, statusModel.singleStatus.selectedStatusFunction));

                statusModel.effectOnEnd = () => baseManager.EventManagerV2.RemoveDelegateFromEvent(
                        EventTriggerEnum.OnTakingDmg,
                        () => RunStatusMethodFromType(statusModel, statusModel.singleStatus.selectedStatusFunction)
                    );

                statusModel.SaveTurnToReset();

                //Add status to active list
                activeStatuses.Add(statusModel);
            } else 
            if( statusModel.turnOff ){
                EndStatusAndTriggerEffectsOnEvent(
                    true, 
                    statusModel,
                    () => baseManager.EventManagerV2.RemoveDelegateFromEvent(EventTriggerEnum.OnTakingDmg, () => RunStatusMethodFromType(statusModel, statusModel.singleStatus.selectedStatusFunction))
                    );
            }
        }

        public void OnEvent(StatusModel statusModel, EventTriggerEnum trigger, EffectGrpEnum effectGrp)
        {
            if (!statusModel.turnOff)
            {
                if (!DoesStatusExist(statusModel.singleStatus.statusName))
                {
                    battleDetailsManager.ShowLabel(statusModel, statusModel.singleStatus.buff ? buffHolder : debuffHolder);
                    //var statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                    var effect = new EffectOnEventModel();
                    effect.effectPower = statusModel.power;
                    effect.triggerChance = statusModel.triggerChance;
                    effect.FocusAttribute = (CharacterStats)Enum.Parse(typeof(CharacterStats), statusModel.singleStatus.attributeName);
                    effect.owner = gameObject;
                    effect.EffectGrp = effectGrp;
                    effect.target = this.baseManager.characterManager;

                    baseManager.EventManagerV2.AddDelegateToEvent(
                        trigger,
                        effect.RunEffectFromSkill,
                        true
                    );

                    statusModel.effectOnEnd = () => baseManager.EventManagerV2.RemoveDelegateFromEvent(
                        trigger,
                        effect.RunEffectFromSkill
                    );

                    statusModel.SaveTurnToReset();

                    activeStatuses.Add(statusModel);
                }
            } else if (statusModel.turnOff)
            {
                EndStatusAndTriggerEffectsOnEvent(true, statusModel);
            }
        }

        public void SetImmunity(StatusModel statusModel)
        {
            if (!statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName))
            {
                battleDetailsManager.ShowLabel(statusModel, statusModel.singleStatus.buff ? buffHolder : debuffHolder);
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                immunityList.AddRange(statusModel.singleStatus.immunityList);
                statusModel.SaveTurnToReset();

                statusModel.effectOnEnd = () => statusModel.singleStatus.immunityList.ForEach(o =>
                    {
                        if (immunityList.Contains(o))
                        {
                            immunityList.Remove(o);
                        }
                    });

                activeStatuses.Add(statusModel);
            }
            else
            if (statusModel.turnOff)
            {
                EndStatusAndTriggerEffectsOnEvent(true, statusModel);
            }
        }

        public void MakeStunned(int turnDuration)
        {
            var stunStatus = singleStatusList.Find(s => s.statusName == StatusNameEnum.Stun);
            var sm = new StatusModel
            {
                singleStatus = stunStatus,
                power = 2,
                turnDuration = turnDuration,
                baseManager = baseManager
            };
            RunStatusFunction(sm);
        }

        private void RunStatusMethodFromType(StatusModel statusModel, StatusFunctionEnum statusFunction)
        {
            switch (statusFunction)
            {
                case StatusFunctionEnum.AttributeChange:
                    AddTemporaryStats(statusModel);
                    break;
                case StatusFunctionEnum.AddToStat:
                    AddPermanentStats(statusModel);
                    break;
                case StatusFunctionEnum.StatChange:
                    AddTickingStatChanges(statusModel);
                    break;
                case StatusFunctionEnum.StatusOn:
                    StatusOn(statusModel);
                    break;
                case StatusFunctionEnum.Tumor:
                    Tumor(statusModel);
                    break;
                case StatusFunctionEnum.OnHit:
                    //OnHit( statusModel);
                    break;
                case StatusFunctionEnum.OnTakingDamage:
                    OnTakeDmg(statusModel);
                    break;
                case StatusFunctionEnum.Immune:
                    SetImmunity(statusModel);
                    break;
            }
        }

        public void RunStatusFunction( StatusModel statusModel ){

            if (statusModel.singleStatus.StatusTriggers.Count == 0)
            {
                RunStatusMethodFromType(statusModel, statusModel.singleStatus.selectedStatusFunction);
                return;
            }

            statusModel.singleStatus.StatusTriggers.ForEach(statusTrigger =>
            {
                statusTrigger.Triggers.ForEach(trigger =>
                {
                     switch (trigger)
                     {
                         case EventTriggerEnum.None:
                             break;
                         case EventTriggerEnum.Passive:
                             break;
                         case EventTriggerEnum.OnTakingDmg:
                             OnTakeDmg(statusModel);
                             break;
                         case EventTriggerEnum.OnDealingDmg:
                             break;
                         case EventTriggerEnum.OnHeal:
                             break;
                         case EventTriggerEnum.OnMove:
                             OnEvent(statusModel, EventTriggerEnum.OnMove, statusTrigger.EffectGrpEnum);
                             break;
                         case EventTriggerEnum.OnSkillCast:
                             OnEvent(statusModel, EventTriggerEnum.OnSkillCast, statusTrigger.EffectGrpEnum);
                             break;
                         case EventTriggerEnum.OnFirstRow:
                             break;
                         case EventTriggerEnum.OnMiddleRow:
                             break;
                         case EventTriggerEnum.OnLastRow:
                             break;
                        case EventTriggerEnum.OnAction:
                            OnEvent(statusModel, trigger, statusTrigger.EffectGrpEnum);
                            break;
                        default:
                             break;
                     }
                });
            });
        }

    }
}






