using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.PackageManager;

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
            singleStatusList = BattleManager.assetFinder.GetAllStatuses();
            battleDetailsManager = BattleManager.battleDetailsManager;
        }

        void Update()
        {
            
        }

        public void RunStatusActions(BattleManager.TurnEnum relevantTurn)
        {
            if (BattleManager.turn == relevantTurn)
            {
                foreach (var status in activeStatuses)
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
        public void AttributeChange( StatusModel statusModel ){
            var currentStat = this.gameObject.tag.ToLower() == "enemy" ? GetComponent<EnemyCharacterManager>().GetAttributeValue(statusModel.singleStatus.attributeName, this.baseManager.characterManager.characterModel) : 
                GetComponent<CharacterManager>().GetAttributeValue(statusModel.singleStatus.attributeName, this.baseManager.characterManager.characterModel);
            var originalStat = this.gameObject.tag.ToLower() == "enemy" ? GetComponent<EnemyCharacterManager>().GetAttributeValue("original" + statusModel.singleStatus.attributeName, this.baseManager.characterManager.characterModel) : 
                GetComponent<CharacterManager>().GetAttributeValue("original" + statusModel.singleStatus.attributeName, this.baseManager.characterManager.characterModel);
            float statValue;
            if( statusModel.singleStatus.name == "thorns" ){
                statValue = (statusModel.power / 20) + originalStat;
            }else if(statusModel.isFlat)
            {
                statValue = statusModel.power;
            }else{
                statValue = (statusModel.power / 100) * originalStat;
            }
            float newStat;
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName ))
            {
                battleDetailsManager.ShowLabel(statusModel, statusModel.singleStatus.buff ? buffHolder : debuffHolder);
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                if ( statusModel.singleStatus.buff ){ //Attack speed not relevant in game anymore
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
        public void AddToStatus(StatusModel statusModel)
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
                 StatusLabelModel currentStatusLabel = GetStatusIfExist(statusModel.singleStatus.name);
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
        public void StatChanges(StatusModel statusModel)
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
                    StatusLabelModel currentStatusLabel = GetStatusIfExist(statusModel.singleStatus.name);
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

        public bool DoesStatusExist( string statusName ){
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
                            if (statusLabelScript.statusname == statusName)
                            {
                                return true;
                            }
                        }
                    }
                }
            return false;
        }

        public SingleStatusModel GetStatus( string statusName ){
            for( int i = 0; i < singleStatusList.Count; i++ ){            
                if( singleStatusList[i].statusName == statusName  ){
                    return singleStatusList[i];
                }   
            }
            return null;
        }

        public StatusLabelModel GetStatusIfExist( string statusName ){
            Transform statusPanel;
            SingleStatusModel status;
            for( int i = 0; i < singleStatusList.Count; i++ ){            
                if(singleStatusList[i].statusName == statusName){
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
                            if (statusLabelScript.statusname == statusName)
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
            battleDetailsManager.RemoveLabel(GetStatusIfExist(singleStatus.name));
            if (singleStatus.hitAnim != animationOptionsEnum.none)
            {
                var animationManager = GetComponent<Animation_Manager>();
                animationManager.AddStatusAnimation(false, singleStatus.hitAnim, singleStatus.holdAnim);
            }
        }

        public void StatusOn( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName) && !CheckImmunity( statusModel.singleStatus.attributeName ))
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
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
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
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                var attrValue = baseManager.characterManager.GetAttributeValue(statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel);
                var effect = ScriptableObject.CreateInstance<EffectOnEventModel>();

                //effect.power = statusModel.power;
                //effect.turnDuration = statusModel.turnDuration;
                //effect.trigger = "OnTakingDmg";
                effect.triggerChance = 1; //comes from status
                //effect.ready = true; //statusModel.singleStatus.active
                //effect.effect = EffectOnEventModel.effectGrp.Status;
                effect.eventAction = () => RunStatusMethodFromType(statusModel, statusModel.singleStatus.selectedStatusFunction);
                //effect.affectSelf = true; //comes from status
                effect.owner = gameObject;
                //effect.coolDown = 0;

                BattleManager.eventManager.EventAction += effect.RunEffectAction;

                statusModel.SaveTurnToReset();

                //Add status to active list
                activeStatuses.Add(statusModel);
            } else 
            if( statusModel.turnOff ){
                EndStatusAndTriggerEffectsOnEvent(true, statusModel);
            }
        }

        //set Immunity
        public void SetImmunity(StatusModel statusModel)
        {
            if (!statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName))
            {
                battleDetailsManager.ShowLabel(statusModel, statusModel.singleStatus.buff ? buffHolder : debuffHolder);
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
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
            var stunStatus = singleStatusList.Find(s => s.statusName.ToLower() == "stun");
            var sm = new StatusModel
            {
                singleStatus = stunStatus,
                power = 2,
                turnDuration = turnDuration,
                baseManager = baseManager
            };
            RunStatusFunction(sm);
        }

        private void RunStatusMethodFromType(StatusModel statusModel, SingleStatusModel.statusFunction statusFunction)
        {
            switch (statusFunction)
            {
                case SingleStatusModel.statusFunction.AttributeChange:
                    AttributeChange(statusModel);
                    break;
                case SingleStatusModel.statusFunction.AddToStat:
                    AddToStatus(statusModel);
                    break;
                case SingleStatusModel.statusFunction.StatChange:
                    StatChanges(statusModel);
                    break;
                case SingleStatusModel.statusFunction.StatusOn:
                    StatusOn(statusModel);
                    break;
                case SingleStatusModel.statusFunction.Tumor:
                    Tumor(statusModel);
                    break;
                case SingleStatusModel.statusFunction.OnHit:
                    //OnHit( statusModel);
                    break;
                case SingleStatusModel.statusFunction.OnTakingDamage:
                    OnTakeDmg(statusModel);
                    break;
                case SingleStatusModel.statusFunction.Immune:
                    SetImmunity(statusModel);
                    break;
            }
        }

        public void RunStatusFunction( StatusModel statusModel ){
            if (statusModel.singleStatus.trigger == triggerGrp.None)
            {
                RunStatusMethodFromType(statusModel, statusModel.singleStatus.selectedStatusFunction);
            } else
            {
                switch (statusModel.singleStatus.trigger)
                {
                    case triggerGrp.None:
                        break;
                    case triggerGrp.Passive:
                        break;
                    case triggerGrp.OnTakingDmg:
                        OnTakeDmg(statusModel);
                        break;
                    case triggerGrp.OnDealingDmg:
                        break;
                    case triggerGrp.OnHeal:
                        break;
                    case triggerGrp.OnMove:
                        break;
                    case triggerGrp.OnSkillCast:
                        break;
                    case triggerGrp.OnFirstRow:
                        break;
                    case triggerGrp.OnMiddleRow:
                        break;
                    case triggerGrp.OnLastRow:
                        break;
                    default:
                        break;
                }
            }
        }

        public void RunStatusFunctions(List<StatusModel> statusModels)
        {
            statusModels.ForEach(statusModel =>
            {
                RunStatusFunction(statusModel);
            });
            
        }
    }
}






