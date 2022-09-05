using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class StatusManager : MonoBehaviour
    {
        public BaseCharacterManagerGroup baseManager;
        public BattleDetailsManager battleDetailsManager;
        public Dictionary<string,Task> taskList = new Dictionary<string, Task>();
        public GameObject statusHolderObject;
        [Header("Immunity List:")]
        public List<SingleStatusModel> immunityList = new List<SingleStatusModel>();
        [Header("Status List:")]
        public List<SingleStatusModel> singleStatusList = new List<SingleStatusModel>();

        void Awake(){
            //battleDetailsManager = Battle_Manager.battleDetailsManager;
        }

        void Start()
        {
            singleStatusList = BattleManager.assetFinder.GetAllStatuses();
            baseManager = this.gameObject.GetComponent<BaseCharacterManagerGroup>();
            battleDetailsManager = BattleManager.battleDetailsManager;
            if (statusHolderObject == null)
            {
                statusHolderObject = GameObject.Find(baseManager.characterManager.characterModel.role.ToString() + "status");
            }
        }

        void Update()
        {

        }

        public Task ResetOnValidTurn(StatusModel statusModel, Action action = null)
        {
            var turn = this.gameObject.tag.ToLower() == "enemy" ? BattleManager.TurnEnum.EnemyTurn : BattleManager.TurnEnum.PlayerTurn;
            var myTask = new Task(BattleManager.taskManager.CompareTurnsAndAction(statusModel.turnToReset, turn, () =>
            {                
                if (baseManager.characterManager.characterModel.isAlive)
                {
                    if (gameObject != null)
                    {
                        if (action != null)
                        {
                            action();
                        }
                    }
                }
            }));

            return myTask;
        }

        //For temporary buffs to stats
        public void AttributeChange( StatusModel statusModel ){
            var currentStat = this.gameObject.tag.ToLower() == "enemy" ? GetComponent<EnemyCharacterManager>().GetAttributeValue(statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel) : 
                GetComponent<CharacterManager>().GetAttributeValue(statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel);
            var originalStat = this.gameObject.tag.ToLower() == "enemy" ? GetComponent<EnemyCharacterManager>().GetAttributeValue("original" + statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel) : 
                GetComponent<CharacterManager>().GetAttributeValue("original" + statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel);
            float statValue;
            if( statusModel.singleStatus.name == "thorns" ){
                statValue = (statusModel.power / 20) + originalStat;
            } else if (statusModel.isFlat )
            {
                statValue = statusModel.power;
            } else {
                statValue = (statusModel.power / 100) * originalStat;
            }
            float newStat;
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName ))
            {
                battleDetailsManager.ShowLabel(statusModel, statusHolderObject);
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                if ( statusModel.singleStatus.buff ){ //Attack speed not relevant in game anymore
                    newStat = statusModel.singleStatus.attributeName != "ATKspd" ? currentStat + statValue : currentStat - statValue;
                } else {
                    newStat = statusModel.singleStatus.attributeName != "ATKspd" ? currentStat - statValue : currentStat + statValue;
                } 
                //set New stat to character
                baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, newStat, baseManager.characterManager.characterModel);

                statusModel.SaveTurnToReset();
                statusLabel.durationTimer = ResetOnValidTurn(statusModel);
                statusLabel.durationTimer.Finished += (t) => EndStatusAndTriggerEffectsOnEvent(t, statusModel, () =>
                {
                    baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, originalStat, baseManager.characterManager.characterModel);
                });
            } else 
            if( statusModel.turnOff ){
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                statusLabel.durationTimer.Stop();
            }
        }

        //Add to stat permanently
        public void AddToStatus(StatusModel statusModel)
        {
            var attributeName = statusModel.singleStatus.attributeName;
            var originalStat = baseManager.characterManager.GetAttributeValue("original" + statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel);
            if (!statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName))
            {
                battleDetailsManager.ShowLabel(statusModel, statusHolderObject);
                if (statusModel.singleStatus.attributeName != null)
                {
                    baseManager.characterManager.SetAttribute(attributeName, statusModel.power, baseManager.characterManager.characterModel);
                }
            }
            else
            if (statusModel.turnOff)
            {
                ForceStatusOff(statusModel.singleStatus);
                baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, originalStat, baseManager.characterManager.characterModel);
            }
        }

        private Task RefreshDotTimer(StatusModel statusModel, BattleManager.TurnEnum relevantTurn)
        {
            Task myTask = new Task(BattleManager.taskManager.CompareTurnsAndActionDuring(BattleManager.turnCount + statusModel.turnDuration * 2, relevantTurn, () =>
            {
                if (baseManager.characterManager.characterModel.isAlive && statusModel != null)
                {
                    StatusLabelModel currentStatusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                    statusModel.stacks = currentStatusLabel.stacks;
                    BattleManager.taskManager.CallChangePointsTask(statusModel);
                }
            }));
            return myTask;
        }

        void EndStatusAndTriggerEffectsOnEvent(bool stoppedManually, StatusModel statusModel, Action actionOnStop = null)
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
            }
            actionOnStop?.Invoke();
        }


        //Stat changes - use for Ticking Stat changes 
        public void StatChanges(StatusModel statusModel)
        {
            var relevantTurn = this.gameObject.tag == "Enemy" ? BattleManager.TurnEnum.EnemyTurn : BattleManager.TurnEnum.PlayerTurn;
            if (!statusModel.turnOff)
            {
                if (!DoesStatusExist(statusModel.singleStatus.statusName))
                {
                    battleDetailsManager.ShowLabel(statusModel, statusHolderObject);
                    StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                    statusModel.SaveTurnToReset();
                    statusLabel.tickTimer = RefreshDotTimer(statusModel, relevantTurn);
                    statusLabel.tickTimer.Finished += (t) => EndStatusAndTriggerEffectsOnEvent(t, statusModel);
                }
                else
                {
                    StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                    if (statusModel.singleStatus.canStack)
                    {
                        statusLabel.stacks = statusLabel.stacks < statusModel.singleStatus.maxStacks ? statusLabel.stacks + 1 : statusLabel.stacks;
                        battleDetailsManager.AddStacks(statusLabel);
                        statusLabel.tickTimer.Stop();
                        statusModel.power += statusLabel.buffPower;
                        statusLabel.buffPower = statusModel.power;
                    }
                    statusModel.SaveTurnToReset();
                    statusLabel.tickTimer = RefreshDotTimer(statusModel, relevantTurn);
                }
            }
            else
            {
                StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                statusLabel.tickTimer.Stop();
            }
        }

        public bool DoesStatusExist( string statusName ){
            if (statusHolderObject)
            {
                Transform statusPanel;
                SingleStatusModel status;
                for (int i = 0; i < singleStatusList.Count; i++)
                {
                    if (singleStatusList[i].statusName == statusName)
                    {
                        status = singleStatusList[i];
                        if (status.buff)
                        {
                            statusPanel = statusHolderObject.transform.Find("Panel buffs");
                        }
                        else
                        {
                            statusPanel = statusHolderObject.transform.Find("Panel debuffs");
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
                if( singleStatusList[i].statusName == statusName  ){
                    status = singleStatusList[i];
                    if (status.buff)
                    {
                        statusPanel = statusHolderObject ? statusHolderObject.transform.Find("Panel buffs") : null; 
                    }
                    else
                    {
                        statusPanel = statusHolderObject ? statusHolderObject.transform.Find("Panel debuffs") : null;
                    }
                    if (statusPanel)
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
                //this need to change to a variable stored in the Manager for the GameObject that holds the statuses 
            var statusHolderObject = GameObject.Find( baseManager.characterManager.characterModel.role.ToString() + "status" );
            Transform statusPanel;
                    if( buff ){
                        statusPanel = statusHolderObject.transform.Find( "Panel buffs" );
                    } else {
                        statusPanel = statusHolderObject.transform.Find( "Panel debuffs" ); 
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
                battleDetailsManager.ShowLabel(statusModel, statusHolderObject);
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                if (statusModel.singleStatus.hitAnim != animationOptionsEnum.none && statusModel.singleStatus.holdAnim != animationOptionsEnum.none)
                {
                    baseManager.animationManager.AddStatusAnimation(true, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                }
                statusModel.SaveTurnToReset();
                statusLabel.durationTimer = ResetOnValidTurn(statusModel);
                statusLabel.durationTimer.Finished += (t) => EndStatusAndTriggerEffectsOnEvent(t, statusModel);
            } 
            else if( statusModel.turnOff ){
                StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                statusLabel.durationTimer.Stop();
            }
        }

        //Tumor set Timer then do damage
        public void Tumor( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName) ){ 
                battleDetailsManager.ShowLabel( statusModel, statusHolderObject );
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                if ( statusModel.singleStatus.hitAnim != animationOptionsEnum.none)
                {
                    baseManager.animationManager.AddStatusAnimation( true, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                }
                statusModel.SaveTurnToReset();
                statusLabel.durationTimer = ResetOnValidTurn(statusModel);
                statusLabel.durationTimer.Finished += (t) => EndStatusAndTriggerEffectsOnEvent(t, statusModel, () =>
                {
                    if (gameObject)
                    {
                        baseManager.damageManager.DoDamage((int)statusLabel.buffPower, baseManager.characterManager, isMagic: true);
                    }
                });
            } else 
            if( statusModel.turnOff)
            {
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                statusLabel.durationTimer.Stop();
            }
        }

        public void OnTakeDmg( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName)){ 
                battleDetailsManager.ShowLabel(statusModel, statusHolderObject);
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                var attrValue = baseManager.characterManager.GetAttributeValue(statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel);
                var effect = ScriptableObject.CreateInstance<EffectOnEventModel>();
                effect.power = attrValue * 0.25f;
                effect.turnDuration = statusModel.turnDuration;
                effect.trigger = "OnTakingDmg";
                effect.triggerChance = 1; //comes from status
                effect.ready = true; //statusModel.singleStatus.active
                effect.effect = EffectOnEventModel.effectGrp.Status;
                effect.affectSelf = true; //comes from status
                effect.owner = gameObject;
                effect.coolDown = 0;
                BattleManager.eventManager.EventAction += effect.RunEffect;

                statusModel.SaveTurnToReset();
                statusLabel.durationTimer = ResetOnValidTurn(statusModel);
                statusLabel.durationTimer.Finished += (t) => EndStatusAndTriggerEffectsOnEvent(t, statusModel);
            } else 
            if( statusModel.turnOff ){
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                statusLabel.durationTimer.Stop();
                statusLabel.durationTimer.Finished -= (t) => EndStatusAndTriggerEffectsOnEvent(t, statusModel);
            }
        }

        //set Immunity
        public void SetImmunity(StatusModel statusModel)
        {
            if (!statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName))
            {
                battleDetailsManager.ShowLabel(statusModel, statusHolderObject);
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                immunityList.AddRange(statusModel.singleStatus.immunityList);
                statusModel.SaveTurnToReset();
                statusLabel.durationTimer = ResetOnValidTurn(statusModel);
                statusLabel.durationTimer.Finished += (t) => EndStatusAndTriggerEffectsOnEvent(t, statusModel, () =>
                {
                    statusModel.singleStatus.immunityList.ForEach(o =>
                    {
                        if (immunityList.Contains(o))
                        {
                            immunityList.Remove(o);
                        }
                    });
                });
            }
            else
            if (statusModel.turnOff)
            {
                var statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                statusLabel.durationTimer.Stop();
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

        public void RunStatusFunction( StatusModel statusModel ){
                switch ( statusModel.singleStatus.selectedStatusFunction ) {
                case SingleStatusModel.statusFunction.AttributeChange:
                    AttributeChange( statusModel );
                    break;
                case SingleStatusModel.statusFunction.AddToStat: 
                    AddToStatus( statusModel );
                    break;
                case SingleStatusModel.statusFunction.StatChange:
                    StatChanges( statusModel );
                    break;
                case SingleStatusModel.statusFunction.StatusOn:
                    StatusOn( statusModel );
                    break;
                case SingleStatusModel.statusFunction.Tumor:
                    Tumor( statusModel );
                    break;
                case SingleStatusModel.statusFunction.OnHit:
                    //OnHit( statusModel);
                    break;
                case SingleStatusModel.statusFunction.OnHitEnemy:
                    OnTakeDmg( statusModel );
                    break;
                case SingleStatusModel.statusFunction.Immune:
                    SetImmunity( statusModel );
                    break;
                }
            if (false)
            {
                if (statusModel.singleStatus.buff)
                {
                    MainGameManager.instance.soundManager.playSoundUsingAudioSource("buff", baseManager.gameObject.GetComponent<AudioSource>());
                }
                else
                {
                    MainGameManager.instance.soundManager.playSoundUsingAudioSource("debuff", baseManager.gameObject.GetComponent<AudioSource>());
                }

            }
        }
    }
}






