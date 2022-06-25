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
            var myTask = new Task(BattleManager.taskManager.CompareTurns(statusModel.turnToReset, () =>
            {                if (baseManager.characterManager.characterModel.isAlive)
                {
                    if (gameObject != null)
                    {
                        battleDetailsManager.RemoveLabel(GetStatusIfExist(statusModel.singleStatus.name));
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
            var currentStat = this.gameObject.tag == "Enemy" ? GetComponent<EnemyCharacterManager>().GetAttributeValue(statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel) : 
                GetComponent<CharacterManager>().GetAttributeValue(statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel);
            var originalStat = this.gameObject.tag == "Enemy" ? GetComponent<EnemyCharacterManager>().GetAttributeValue("original" + statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel) : 
                GetComponent<CharacterManager>().GetAttributeValue("original" + statusModel.singleStatus.attributeName, baseManager.characterManager.characterModel);
            float buffedStat;
            if( statusModel.singleStatus.name == "thorns" ){
                buffedStat = (statusModel.power / 20) + originalStat;
            } else if (/*statusModel.singleStatus.name == "vigor" ||*/ statusModel.isFlat )
            {
                buffedStat = statusModel.power;
            } else {
                buffedStat = (statusModel.power / 100) * originalStat;
            }
            float newStat;
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName ) ){ 
                battleDetailsManager.ShowLabel( statusModel, statusHolderObject );
                if ( statusModel.singleStatus.buff ){
                    newStat = statusModel.singleStatus.attributeName != "ATKspd" ? currentStat + buffedStat : currentStat - buffedStat;
                } else {
                    newStat = statusModel.singleStatus.attributeName != "ATKspd" ? currentStat - buffedStat : currentStat + buffedStat;
                } 
    
                //set New stat to character
                baseManager.characterManager.SetAttribute( statusModel.singleStatus.attributeName, newStat, baseManager.characterManager.characterModel);

                //Remove status 
                statusModel.SaveTurnToReset();
                ResetOnValidTurn(statusModel, () =>
                {
                    baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, originalStat, baseManager.characterManager.characterModel);
                });

                /*Battle_Manager.taskManager.RemoveLabelTask(statusModel.turnDuration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, originalStat);
                });*/  
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff(statusModel.singleStatus, ()=> {
                    baseManager.characterManager.SetAttribute( statusModel.singleStatus.attributeName, originalStat, baseManager.characterManager.characterModel);
                });
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
                statusModel.SaveTurnToReset();
                ResetOnValidTurn(statusModel, () =>
                {
                    baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, originalStat, baseManager.characterManager.characterModel);
                });

                /*Battle_Manager.taskManager.RemoveLabelTask(statusModel.turnDuration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, originalStat);
                });*/
            }
            else
            if (statusModel.turnOff)
            {
                ForceStatusOff(statusModel.singleStatus);
            }
        }

        public Task TriggerTillTurn(int turnToEnd, BattleManager.TurnEnum turn, Action action, string taskName = null)
        {
            Task myTask = new Task(BattleManager.taskManager.CompareTurns(BattleManager.turnCount + 1, () =>
            {
                if (BattleManager.turn == turn && baseManager.characterManager.characterModel.isAlive)
                {
                    action();
                }
                if (BattleManager.turnCount <= (turnToEnd * 2) && baseManager.characterManager.characterModel.isAlive)
                {
                    if (BattleManager.taskManager.taskList.ContainsKey(taskName))
                    {
                        BattleManager.taskManager.taskList.Remove(taskName);
                    }
                    Task innerTask = TriggerTillTurn(turnToEnd, turn, action, taskName);
                    BattleManager.taskManager.taskList.Add(taskName, innerTask);
                };
            }));
            return myTask;
        }

        private Task RefreshDotTimer(StatusModel statusModel, BattleManager.TurnEnum relevantTurn)
        {
            if (BattleManager.taskManager.taskList.ContainsKey($"{gameObject.name}_{statusModel.singleStatus.statusName}"))
            {
                BattleManager.taskManager.taskList[$"{gameObject.name}_{statusModel.singleStatus.statusName}"].Stop();
                BattleManager.taskManager.taskList.Remove($"{gameObject.name}_{statusModel.singleStatus.statusName}");
            }
            var Task = TriggerTillTurn(statusModel.turnDuration, relevantTurn, () =>
            {
                StatusLabelModel currentStatusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                statusModel.stacks = currentStatusLabel.stacks;
                BattleManager.taskManager.CallChangePointsTask(statusModel);
            }, $"{gameObject.name}_{statusModel.singleStatus.statusName}");

            return Task;
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

                    statusLabel.tickTimer = RefreshDotTimer(statusModel, relevantTurn);

                    /*statusLabel.tickTimer = TriggerTillTurn(statusModel.turnDuration, relevantTurn, () =>
                    {
                        StatusLabelModel currentStatusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                        statusModel.stacks = currentStatusLabel.stacks;
                        Battle_Manager.taskManager.CallChangePointsTask(statusModel);
                    }, statusModel.singleStatus.statusName);*/

                    //if (statusModel.turnDuration > 0)
                    //{
                        statusModel.SaveTurnToReset();
                        statusLabel.durationTimer = ResetOnValidTurn(statusModel, () =>
                        {
                            statusLabel.tickTimer.Stop();
                            Debug.Log($"{statusLabel.statusname} timer stopped line 203");
                        });
                    //}

                    /*statusLabel.durationTimer = Battle_Manager.taskManager.CallDurationTask(statusModel.turnDuration, statusLabel, "durationTimer", () =>
                    {
                        //StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                        statusLabel.tickTimer.Stop();
                    });*/
                }
                else
                {
                    StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                    if (statusModel.singleStatus.canStack)
                    {
                        statusLabel.stacks = statusLabel.stacks < statusModel.singleStatus.maxStacks ? statusLabel.stacks + 1 : statusLabel.stacks;
                        battleDetailsManager.AddStacks(statusLabel);
                        //statusModel.turnDuration = statusModel.turnDuration + BattleManager.turnCount;
                        statusLabel.durationTimer.Stop();
                        statusLabel.tickTimer.Stop();
                        Debug.Log($"{statusLabel.statusname} timer stopped line 223 reset for stack");
                        statusModel.power += statusLabel.buffPower;
                        statusLabel.buffPower = statusModel.power;
                    }

                    //fire every new turn and save Task to cancel later
                    statusLabel.tickTimer = RefreshDotTimer(statusModel, relevantTurn);

                    statusModel.SaveTurnToReset();
                    statusLabel.durationTimer = ResetOnValidTurn(statusModel, () =>
                    {
                        statusLabel.tickTimer.Stop();
                        Debug.Log($"{statusLabel.statusname} timer stopped line 233");
                        if (BattleManager.taskManager.taskList.ContainsKey($"{gameObject.name}_{statusModel.singleStatus.statusName}"))
                        {
                            BattleManager.taskManager.taskList[$"{gameObject.name}_{statusModel.singleStatus.statusName}"].Stop();
                            BattleManager.taskManager.taskList.Remove($"{gameObject.name}_{statusModel.singleStatus.statusName}");
                        }
                    });

                    /*statusLabel.durationTimer = Battle_Manager.taskManager.CallDurationTask(statusModel.turnDuration, statusLabel, "durationTimer", () =>
                    {
                        //StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                        statusLabel.tickTimer.Stop();
                    });*/
                }
            }
            else
            {
                StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.name);
                ForceStatusOff(statusModel.singleStatus, () => {
                    statusLabel.tickTimer.Stop();
                    statusLabel.durationTimer.Stop();
                });
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
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName) && !CheckImmunity( statusModel.singleStatus.attributeName ) ){ 
                battleDetailsManager.ShowLabel( statusModel, statusHolderObject );
                if (statusModel.singleStatus.hitAnim != animationOptionsEnum.none && statusModel.singleStatus.holdAnim != animationOptionsEnum.none)
                {
                    baseManager.animationManager.AddStatusAnimation(true, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                }
                statusModel.SaveTurnToReset();
                ResetOnValidTurn(statusModel, () =>
                {
                    ForceStatusOff(statusModel.singleStatus);
                    if (statusModel.singleStatus.hitAnim != animationOptionsEnum.none)
                    {
                        baseManager.animationManager.AddStatusAnimation(false, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                    }
                });

                /*Battle_Manager.taskManager.RemoveLabelTask(statusModel.turnDuration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    ForceStatusOff( statusModel.singleStatus );
                    if (!string.IsNullOrEmpty(statusModel.singleStatus.hitAnim))
                    {
                        baseManager.animationManager.AddStatusAnimation(false, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                    }
                });*/
                
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff( statusModel.singleStatus );
                if( statusModel.singleStatus.hitAnim != animationOptionsEnum.none){
                    baseManager.animationManager.AddStatusAnimation( false, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                }
            }
        }

        //Tumor set Timer then do damage
        public void Tumor( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName) ){ 
                battleDetailsManager.ShowLabel( statusModel, statusHolderObject );
                if( statusModel.singleStatus.hitAnim != animationOptionsEnum.none)
                {
                    baseManager.animationManager.AddStatusAnimation( true, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                }

                statusModel.SaveTurnToReset();
                ResetOnValidTurn(statusModel, () =>
                {
                    var dmgModel = new BaseDamageModel()
                    {
                        baseManager = statusModel.baseManager,
                        incomingDmg = GetStatusIfExist(statusModel.singleStatus.name).buffPower,
                        dueDmgTargets = new List<BaseCharacterManager> { baseManager.characterManager },
                        hitEffectPositionScript = baseManager.effectsManager.fxCenter.transform,
                        isMagicDmg = true,
                        damageImmidiately = true
                    };
                    baseManager.damageManager.calculatedamage(dmgModel);
                });

                /*Battle_Manager.taskManager.RemoveLabelTask(statusModel.turnDuration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    var dmgModel = new DamageModel(){
                        baseManager = statusModel.baseManager,
                        incomingDmg = GetStatusIfExist( statusModel.singleStatus.statusName ).buffPower,
                        dueDmgTargets = new List<Character_Manager> { baseManager.characterManager },
                        hitEffectPositionScript = baseManager.effectsManager.fxCenter.transform,
                        isMagicDmg = true,
                        damageImmidiately = true
                    };
                    baseManager.damageManager.calculatedamage( dmgModel );
                });*/
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff( statusModel.singleStatus, ()=> {
                    var dmgModel = new BaseDamageModel()
                    {
                        baseManager = statusModel.baseManager,
                        incomingDmg = GetStatusIfExist(statusModel.singleStatus.name).buffPower,
                        dueDmgTargets = new List<BaseCharacterManager> { baseManager.characterManager },
                        hitEffectPositionScript = baseManager.effectsManager.fxCenter.transform,
                        isMagicDmg = true,
                        damageImmidiately = true
                    };
                    baseManager.damageManager.calculatedamage( dmgModel );
                });
            }
        }

        public void OnTakeDmg( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName)){ 
                battleDetailsManager.ShowLabel(statusModel, statusHolderObject);
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
                ResetOnValidTurn(statusModel, () =>
                {
                    ForceStatusOff(statusModel.singleStatus);
                });

                /*Battle_Manager.taskManager.RemoveLabelTask(statusModel.turnDuration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    ForceStatusOff( statusModel.singleStatus );
                });*/
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff( statusModel.singleStatus );
            }
        }

        //set Immunity
        public void SetImmunity(StatusModel statusModel)
        {
            if (!statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName))
            {
                battleDetailsManager.ShowLabel(statusModel, statusHolderObject);
                immunityList.AddRange(statusModel.singleStatus.immunityList);
                statusModel.SaveTurnToReset();
                ResetOnValidTurn(statusModel, () =>
                {
                    statusModel.singleStatus.immunityList.ForEach(o =>
                    {
                        if (immunityList.Contains(o))
                        {
                            immunityList.Remove(o);
                        }
                    });
                    /*immunityList.ForEach(o =>
                    {
                        if (statusModel.singleStatus.immunityList.Contains(o))
                        {
                            immunityList.Remove(o);
                        }
                    });*/
                    ForceStatusOff(statusModel.singleStatus);
                });

                /*Battle_Manager.taskManager.RemoveLabelTask(statusModel.turnDuration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    ForceStatusOff( statusModel.singleStatus );
                    immunityList.Remove( GetStatus( statusModel.singleStatus.statusName ) );
                });*/

            }
            else
            if (statusModel.turnOff)
            {
                ForceStatusOff(statusModel.singleStatus);
                immunityList.Remove(GetStatus(statusModel.singleStatus.attributeName));
                if (statusModel.singleStatus.hitAnim != animationOptionsEnum.none)
                {
                    baseManager.animationManager.AddStatusAnimation(false, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                }
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
            //statusModel.turnDuration = statusModel.turnDuration == 0 ? Mathf.Infinity : statusModel.turnDuration;
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
                    BattleManager.soundManager.playSoundUsingAudioSource("buff", baseManager.gameObject.GetComponent<AudioSource>());
                }
                else
                {
                    BattleManager.soundManager.playSoundUsingAudioSource("debuff", baseManager.gameObject.GetComponent<AudioSource>());
                }

            }
        }
    }
}






