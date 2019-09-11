using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Status_Manager : Base_Character_Manager
    {
        public Battle_Details_Manager battleDetailsManager { get; set; }
        public Button_Click_Manager buttonClickManager { get; set; }
        public GameObject statusHolderObject { get; set; }
        [Header("Immunity List:")]
        public List<SingleStatusModel> immunityList = new List<SingleStatusModel>();
        [Header("Status List:")]
        public List<SingleStatusModel> singleStatusList = new List<SingleStatusModel>();
        void Start()
        {
            battleDetailsManager = Battle_Manager.battleDetailsManager;
            buttonClickManager = this.gameObject.GetComponent<Button_Click_Manager>();
            statusHolderObject = GameObject.Find( characterManager.characterModel.role + "status" );
        }

        public void AttributeChange( StatusModel statusModel ){
            var currentStat = GetComponent<Character_Manager>().GetAttributeValue( statusModel.singleStatus.attributeName );
            var originalStat = GetComponent<Character_Manager>().GetAttributeValue( "original" + statusModel.singleStatus.attributeName );
            float buffedStat;
            if( statusModel.singleStatus.statusName == "thorns" ){
                buffedStat = (statusModel.power / 20) + originalStat;
            } else {
                buffedStat = (statusModel.power / 100) * originalStat;
            }
            float newStat;
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName ) ){ 
                battleDetailsManager.ShowLabel( statusModel );
                if ( statusModel.singleStatus.buff ){
                    newStat = statusModel.singleStatus.attributeName != "ATKspd" ? currentStat + buffedStat : currentStat - buffedStat;
                } else {
                    newStat = statusModel.singleStatus.attributeName != "ATKspd" ? currentStat - buffedStat : currentStat + buffedStat;
                } 
    
                //set New stat to character
                GetComponent<Character_Manager>().SetAttribute( statusModel.singleStatus.attributeName, newStat);

                //Remove status 
                Battle_Manager.taskManager.RemoveLabelTask(statusModel.duration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    GetComponent<Character_Manager>().SetAttribute(statusModel.singleStatus.attributeName, originalStat);
                });      
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff(statusModel.singleStatus, ()=> {
                    GetComponent<Character_Manager>().SetAttribute( statusModel.singleStatus.attributeName, originalStat);
                });
            }
        }

        //Add to stat permanently
        public void AddToStatus(StatusModel statusModel)
        {
            var attributeName = statusModel.targetStat == "" ? statusModel.singleStatus.attributeName : statusModel.targetStat;
            var originalStat = GetComponent<Character_Manager>().GetAttributeValue("original" + statusModel.singleStatus.attributeName);
            if (!statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName))
            {
                battleDetailsManager.ShowLabel(statusModel);
                if (statusModel.singleStatus.attributeName != null)
                {
                    GetComponent<Character_Manager>().SetAttribute(attributeName, statusModel.power);
                }
                Battle_Manager.taskManager.RemoveLabelTask(statusModel.duration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    GetComponent<Character_Manager>().SetAttribute(statusModel.singleStatus.attributeName, originalStat);
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
            StatusLabelModel statusLabel = GetStatusIfExist(statusModel.singleStatus.statusName);
            if (!statusModel.turnOff)
            {
                if (!DoesStatusExist(statusModel.singleStatus.statusName))
                {
                    battleDetailsManager.ShowLabel(statusModel);
                    statusLabel.tickTimer = Battle_Manager.taskManager.CallChangePointsTask(statusModel);
                }
                else
                {
                    if (statusModel.singleStatus.canStack)
                    {
                        statusLabel.stacks = statusLabel.stacks < statusModel.singleStatus.maxStacks ? statusLabel.stacks + 1 : statusLabel.stacks;
                        battleDetailsManager.AddStacks(statusLabel);
                        statusModel.stacks = statusLabel.stacks;
                    }
                    statusLabel.tickTimer.Stop();
                    statusLabel.tickTimer = Battle_Manager.taskManager.CallChangePointsTask(statusModel);
                }
                Battle_Manager.taskManager.CallDurationTask(statusModel.duration, GetStatusIfExist( statusModel.singleStatus.statusName ), () =>
                {
                    statusLabel.tickTimer.Stop();
                });
            }
            else
            {
                ForceStatusOff(statusModel.singleStatus, () => {
                    statusLabel.tickTimer.Stop();
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
            List<StatusLabelModel> currentStatusList = null;
                //this need to change to a variable stored in the Manager for the GameObject that holds the statuses 
            var statusHolderObject = GameObject.Find( characterManager.name + "status" );
            Transform statusPanel;
            //singleStatus status;
            //for( int i = 0; i < singleStatusList.Count; i++ ){          
              //if( singleStatusList[i].statusName == statusName  ){
                    //status = statusList[i];
                    if( buff ){
                        statusPanel = statusHolderObject.transform.Find( "Panel buffs" );
                    } else {
                        statusPanel = statusHolderObject.transform.Find( "Panel debuffs" ); 
                    }
                    int statusCount = statusPanel.childCount;
                    for( int x = 0; x < statusCount; x++ ){
                        currentStatusList.Add( statusPanel.GetChild(x).GetComponent<StatusLabelModel>() );
                    }
               // }
            //}
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
                animationManager.AddStatusAnimation(false, singleStatus.hitAnim, singleStatus.holdAnim, false);
            }
        }

        public void StatusOn( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName ) && !CheckImmunity( statusModel.singleStatus.attributeName ) ){ 
                battleDetailsManager.ShowLabel( statusModel );
                Battle_Manager.taskManager.RemoveLabelTask(statusModel.duration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    ForceStatusOff( statusModel.singleStatus );
                });
                if( statusModel.singleStatus.hitAnim != "" ){
                    animationManager.AddStatusAnimation( true, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim, false );
                }
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff( statusModel.singleStatus );
                if( statusModel.singleStatus.hitAnim != "" ){
                    animationManager.AddStatusAnimation( false, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim, false );
                }
            }
        }

        //Tumor set Timer then do damage
        public void Tumor( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName ) ){ 
                battleDetailsManager.ShowLabel( statusModel );
                if( statusModel.singleStatus.hitAnim != "" ){
                    animationManager.AddStatusAnimation( true, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim, false );
                }

                Battle_Manager.taskManager.RemoveLabelTask(statusModel.duration, GetStatusIfExist(statusModel.singleStatus.statusName), () =>
                {
                    var dmgModel = new DamageModel{ 
                        incomingDmg = GetStatusIfExist( statusModel.singleStatus.statusName ).buffPower
                    };
                    characterManager.damageManager.calculateMdamge( dmgModel );
                });
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff( statusModel.singleStatus, ()=> {
                    var dmgModel = new DamageModel{ 
                        incomingDmg = GetStatusIfExist( statusModel.singleStatus.statusName ).buffPower
                    };
                    characterManager.damageManager.calculateMdamge( dmgModel );
                });
            }
        }

        public void OnTakeDmg( StatusModel statusModel ){
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName ) ){ 
                battleDetailsManager.ShowLabel( statusModel );
                var attrValue = characterManager.GetAttributeValue( statusModel.singleStatus.attributeName );
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
                EventManager.EventAction += effect.RunEffect;

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
                    animationManager.AddStatusAnimation( false, statusModel.singleStatus.hitAnim, statusModel.singleStatus.holdAnim, false );
                }
            }
        }

        public void RunStatusFunction( StatusModel statusModel ){
            //makes duration infinite if set to 0
            statusModel.duration = statusModel.duration == 0 ? Mathf.Infinity : statusModel.duration;
            switch ( statusModel.selectedStatusFunction ) {
            case StatusModel.statusFunction.AttributeChange:
                AttributeChange( statusModel );
                break;
            case StatusModel.statusFunction.AddToStat:
                AddToStatus( statusModel );
                break;
            case StatusModel.statusFunction.StatChange:
                StatChanges( statusModel );
                break;
            case StatusModel.statusFunction.StatusOn:
                StatusOn( statusModel );
                break;
            case StatusModel.statusFunction.Tumor:
                Tumor( statusModel );
                break;
            case StatusModel.statusFunction.OnHit:
                //OnHit( statusModel);
                break;
            case StatusModel.statusFunction.OnHitEnemy:
                OnTakeDmg( statusModel );
                break;
            case StatusModel.statusFunction.Immune:
                SetImmunity( statusModel );
                break;
            }
        }
    }
}






