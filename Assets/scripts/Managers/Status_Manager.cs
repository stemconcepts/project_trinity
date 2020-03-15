using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Status_Manager : MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        public Battle_Details_Manager battleDetailsManager;
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
            singleStatusList = Battle_Manager.assetFinder.GetAllStatuses();
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            battleDetailsManager = Battle_Manager.battleDetailsManager;
            /*if (baseManager.characterManager.characterModel.role == Character_Model.RoleEnum.minion)
            {
                statusHolderObject = GameObject.Find(gameObject.name + "_data_status");
            } else
            {*/
            if (statusHolderObject == null)
            {
                statusHolderObject = GameObject.Find(baseManager.characterManager.characterModel.role.ToString() + "status");
            }
                
            //}
        }

        public void AttributeChange( StatusModel statusModel ){
            var currentStat = GetComponent<Character_Manager>().GetAttributeValue( statusModel.singleStatus.attributeName );
            var originalStat = GetComponent<Character_Manager>().GetAttributeValue( "original" + statusModel.singleStatus.attributeName );
            float buffedStat;
            if( statusModel.singleStatus.statusName == "thorns" ){
                buffedStat = (statusModel.power / 20) + originalStat;
            } else if (statusModel.singleStatus.statusName == "vigor" )
            {
                buffedStat = statusModel.power + currentStat;
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
                baseManager.characterManager.SetAttribute( statusModel.singleStatus.attributeName, newStat);

                //Remove status 
                Battle_Manager.taskManager.RemoveLabelTask(statusModel.duration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, originalStat);
                });      
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff(statusModel.singleStatus, ()=> {
                    baseManager.characterManager.SetAttribute( statusModel.singleStatus.attributeName, originalStat);
                });
            }
        }

        //Add to stat permanently
        public void AddToStatus(StatusModel statusModel)
        {
            var attributeName = statusModel.singleStatus.attributeName;
            var originalStat = baseManager.characterManager.GetAttributeValue("original" + statusModel.singleStatus.attributeName);
            if (!statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName))
            {
                battleDetailsManager.ShowLabel(statusModel, statusHolderObject);
                if (statusModel.singleStatus.attributeName != null)
                {
                    baseManager.characterManager.SetAttribute(attributeName, statusModel.power);
                }
                Battle_Manager.taskManager.RemoveLabelTask(statusModel.duration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    baseManager.characterManager.SetAttribute(statusModel.singleStatus.attributeName, originalStat);
                });
            }
            else
            if (statusModel.turnOff)
            {
                ForceStatusOff(statusModel.singleStatus);
            }
        }

        //Stat changes - use for Ticking Stat changes 
        public void StatChanges(StatusModel statusModel)
        {
            if (!statusModel.turnOff)
            {
                
                if (!DoesStatusExist(statusModel.singleStatus.statusName))
                {
                    battleDetailsManager.ShowLabel(statusModel, statusHolderObject);
                    StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                    statusLabel.tickTimer = Battle_Manager.taskManager.CallChangePointsTask(statusModel);
                    statusLabel.durationTimer = Battle_Manager.taskManager.CallDurationTask(statusModel.duration, statusLabel, "durationTimer", () =>
                    {
                        //StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                        statusLabel.tickTimer.Stop();
                    });
                }
                else
                {
                    StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                    if (statusModel.singleStatus.canStack)
                    {
                        statusLabel.stacks = statusLabel.stacks < statusModel.singleStatus.maxStacks ? statusLabel.stacks + 1 : statusLabel.stacks;
                        battleDetailsManager.AddStacks(statusLabel);
                        statusModel.stacks = statusLabel.stacks;
                        statusLabel.tickTimer.Stop();
                        statusLabel.durationTimer.Stop();
                    }
                    statusLabel.tickTimer = Battle_Manager.taskManager.CallChangePointsTask(statusModel);
                    statusLabel.durationTimer = Battle_Manager.taskManager.CallDurationTask(statusModel.duration, statusLabel, "durationTimer", () =>
                    {
                        //StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                        statusLabel.tickTimer.Stop();
                    });
                }
            }
            else
            {
                StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
                ForceStatusOff(statusModel.singleStatus, () => {
                    statusLabel.tickTimer.Stop();
                    statusLabel.durationTimer.Stop();
                });
            }
        }

        public bool DoesStatusExist( string statusName ){
            Transform statusPanel;
            SingleStatusModel status;
            for( int i = 0; i < singleStatusList.Count; i++ ){            
                if( singleStatusList[i].name == statusName  ){
                    status = singleStatusList[i];
                    if( status.buff ){
                        statusPanel = statusHolderObject.transform.Find( "Panel buffs" );
                    } else {
                        statusPanel = statusHolderObject.transform.Find( "Panel debuffs" ); 
                    }
                    int statusCount = statusPanel.childCount;
                    for( int x = 0; x < statusCount; x++ ){
                        var statusLabelScript = statusPanel.GetChild(x).GetComponent<StatusLabelModel>();
                        if( statusLabelScript.statusname == statusName ){
                            return true;
                        } 
                    }
                }
            }
            return false;
        }

        public SingleStatusModel GetStatus( string statusName ){
            for( int i = 0; i < singleStatusList.Count; i++ ){            
                if( singleStatusList[i].name == statusName  ){
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
                    if( status.buff ){
                        statusPanel = statusHolderObject.transform.Find( "Panel buffs" );
                    } else {
                        statusPanel = statusHolderObject.transform.Find( "Panel debuffs" ); 
                    }
                    int statusCount = statusPanel.childCount;
                    for( int x = 0; x < statusCount; x++ ){
                        var statusLabelScript = statusPanel.GetChild(x).GetComponent<StatusLabelModel>();
                        if( statusLabelScript.statusname == statusName ){
                            return statusLabelScript;
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
            battleDetailsManager.RemoveLabel(GetStatusIfExist(singleStatus.statusName));
            if (singleStatus.hitAnim != "")
            {
                var animationManager = GetComponent<Animation_Manager>();
                animationManager.AddStatusAnimation(false, singleStatus.hitAnim, singleStatus.holdAnim);
            }
        }

        public void StatusOn( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName ) && !CheckImmunity( statusModel.singleStatus.attributeName ) ){ 
                battleDetailsManager.ShowLabel( statusModel, statusHolderObject );
                if (!string.IsNullOrEmpty(statusModel.singleStatus.hitAnim) && !string.IsNullOrEmpty(statusModel.singleStatus.holdAnim))
                {
                    baseManager.animationManager.AddStatusAnimation(true, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                }
                Battle_Manager.taskManager.RemoveLabelTask(statusModel.duration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    ForceStatusOff( statusModel.singleStatus );
                    if (!string.IsNullOrEmpty(statusModel.singleStatus.hitAnim))
                    {
                        baseManager.animationManager.AddStatusAnimation(false, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                    }
                });
                
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff( statusModel.singleStatus );
                if( statusModel.singleStatus.hitAnim != "" ){
                    baseManager.animationManager.AddStatusAnimation( false, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                }
            }
        }

        //Tumor set Timer then do damage
        public void Tumor( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName ) ){ 
                battleDetailsManager.ShowLabel( statusModel, statusHolderObject );
                if( statusModel.singleStatus.hitAnim != "" ){
                    baseManager.animationManager.AddStatusAnimation( true, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                }

                Battle_Manager.taskManager.RemoveLabelTask(statusModel.duration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
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
                });
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff( statusModel.singleStatus, ()=> {
                    var dmgModel = new DamageModel()
                    {
                        baseManager = statusModel.baseManager,
                        incomingDmg = GetStatusIfExist(statusModel.singleStatus.statusName).buffPower,
                        dueDmgTargets = new List<Character_Manager> { baseManager.characterManager },
                        hitEffectPositionScript = baseManager.effectsManager.fxCenter.transform,
                        isMagicDmg = true,
                        damageImmidiately = true
                    };
                    baseManager.damageManager.calculatedamage( dmgModel );
                });
            }
        }

        public void OnTakeDmg( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName ) ){ 
                battleDetailsManager.ShowLabel( statusModel, statusHolderObject );
                var attrValue = baseManager.characterManager.GetAttributeValue( statusModel.singleStatus.attributeName );
                var effect = ScriptableObject.CreateInstance<EffectOnEventModel>();
                effect.power = attrValue * 0.25f;
                effect.duration = statusModel.duration;
                effect.trigger = "OnTakingDmg";
                effect.triggerChance = 1; //comes from status
                effect.ready = true; //statusModel.singleStatus.active
                effect.effect = EffectOnEventModel.effectGrp.Status;
                effect.affectSelf = true; //comes from status
                effect.owner = gameObject;
                effect.coolDown = 0;
                Battle_Manager.eventManager.EventAction += effect.RunEffect;

                Battle_Manager.taskManager.RemoveLabelTask(statusModel.duration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    ForceStatusOff( statusModel.singleStatus );
                });
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff( statusModel.singleStatus );
            }
        }

        //set Immunity
        public void SetImmunity( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName ) ){ 
                immunityList.Add( GetStatus( statusModel.singleStatus.statusName ) );

                Battle_Manager.taskManager.RemoveLabelTask(statusModel.duration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    ForceStatusOff( statusModel.singleStatus );
                    immunityList.Remove( GetStatus( statusModel.singleStatus.statusName ) );
                });

            } else 
            if( statusModel.turnOff ){
                ForceStatusOff( statusModel.singleStatus );
                immunityList.Remove( GetStatus( statusModel.singleStatus.attributeName ) );
                if( statusModel.singleStatus.hitAnim != "" ){ 
                    baseManager.animationManager.AddStatusAnimation( false, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim);
                }
            }
        }

        public void RunStatusFunction( StatusModel statusModel ){
                statusModel.duration = statusModel.duration == 0 ? Mathf.Infinity : statusModel.duration;
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
        }
    }
}






