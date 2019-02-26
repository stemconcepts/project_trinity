using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Status_Manager : BasicManager
    {
        public Battle_Details_Manager battleDetailsManager { get; set; }
        public Button_Click_Manager buttonClickManager { get; set; }
        public Status_Manager()
        {
            battleDetailsManager = Battle_Manager.battleDetailsManager;
            buttonClickManager = this.gameObject.GetComponents<Button_Click_Manager>();
        }

        public void AttributeChange( StatusModel statusModel ){
            var currentStat = GetComponent<character_Manager>().GetAttributeValue( statusModel.singleStatus.attributeName );
            var originalStat = GetComponent<character_Manager>().GetAttributeValue( "original" + statusModel.singleStatus.attributeName );
            float buffedStat;
            if( statusModel.singleStatus.statusName == "thorns" ){
                buffedStat = (statusModel.power / 20) + originalStat;
            } else {
                buffedStat = (statusModel.power / 100) * originalStat;
            }
            float newStat;
            if( !statusModel.turnOff && !DoesStatusExist( statusModel.singleStatus.statusName ) ){ 
                battleDetailsManager.ShowLabel( statusModel.singleStatus );
                if ( statusModel.singleStatus.buff ){
                    newStat = statusModel.singleStatus.attributeName != "ATKspd" ? currentStat + buffedStat : currentStat - buffedStat;
                } else {
                    newStat = statusModel.singleStatus.attributeName != "ATKspd" ? currentStat - buffedStat : currentStat + buffedStat;
                } 
    
                //set New stat to character
                GetComponent<character_Manager>().SetAttribute( statusModel.singleStatus.attributeName, newStat);

                //Remove status 
                Battle_Manager.taskManager.CallTask(statusModel.duration, statusModel.singleStatus, () =>
                {
                    GetComponent<character_Manager>().SetAttribute(statusModel.singleStatus.attributeName, originalStat);
                });      
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff(statusModel.singleStatus, ()=> {
                    GetComponent<character_Manager>().SetAttribute( statusModel.singleStatus.attributeName, originalStat);
                });
            }
        }

        //Add to stat permanently
        public void AddToStatus(StatusModel statusModel)
        {
            var attributeName = statusModel.targetStat == "" ? statusModel.singleStatus.attributeName : statusModel.targetStat;
            var originalStat = GetComponent<character_Manager>().GetAttributeValue("original" + statusModel.singleStatus.attributeName);
            if (!statusModel.turnOff && !DoesStatusExist(statusModel.singleStatus.statusName))
            {
                battleDetailsManager.ShowLabel(statusModel.singleStatus);
                if (statusModel.singleStatus.attributeName != null)
                {
                    GetComponent<character_Manager>().SetAttribute(attributeName, statusModel.power);
                }
                Battle_Manager.taskManager.CallTask(statusModel.duration, statusModel.singleStatus, () =>
                {
                    GetComponent<character_Manager>().SetAttribute(statusModel.singleStatus.attributeName, originalStat);
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
                statussinglelabel status = GetStatusIfExist(statusModel.singleStatus.statusName);
                if (!DoesStatusExist(statusModel.singleStatus.statusName))
                {
                    battleDetailsManager.ShowLabel(statusModel.singleStatus);
                    status.tickTimer = Battle_Manager.taskManager.CallChangePointsTask(statusModel);
                }
                else
                {
                    if (statusModel.singleStatus.canStack)
                    {
                        status.stacks = status.stacks < singleStatus.maxStacks ? status.stacks + 1 : status.stacks;
                        battleDetailsManager.AddStacks(status);
                        statusModel.stacks = status.stacks;
                    }
                    status.tickTimer.Stop();
                    status.tickTimer = Battle_Manager.taskManager.CallChangePointsTask(statusModel);
                }
                Battle_Manager.taskManager.CallDurationTask(statusModel, () =>
                {
                    status.tickTimer.Stop();
                });
            }
            else
            {
                ForceStatusOff(statusModel.singleStatus, () => {
                    status.tickTimer.Stop();
                });
            }
        }

        public bool DoesStatusExist( string statusName ){
            Type statusModel = Type.GetType("statusModel");
            var statusList = (List<singleStatus>)statusModel.GetField("statusListSO").GetValue( this );
            var statusHolderObject = GameObject.Find( buttonClickManager.GetClassRole() + "status" );
            Transform statusPanel;
            singleStatus status;
            for( int i = 0; i < statusList.Count; i++ ){            
                if( statusList[i].name == statusName  ){
                    status = statusList[i];
                    if( status.buff ){
                        statusPanel = statusHolderObject.transform.Find( "Panel buffs" );
                    } else {
                        statusPanel = statusHolderObject.transform.Find( "Panel debuffs" ); 
                    }
                    int statusCount = statusPanel.childCount;
                    for( int x = 0; x < statusCount; x++ ){
                        var statusLabelScript = statusPanel.GetChild(x).GetComponent<statussinglelabel>();
                        if( statusLabelScript.singleStatus == status ){
                            return true;
                        } 
                    }
                }
            }
            return false;
        }

        public singleStatus GetStatus( string statusName ){
            Type statusData = Type.GetType("status");
            var statusList = (List<singleStatus>)statusData.GetField("statusListSO").GetValue( this );
            for( int i = 0; i < statusList.Count; i++ ){            
                if( statusList[i].name == statusName  ){
                    return statusList[i];
                }   
            }
            return null;
        }

        public statussinglelabel GetStatusIfExist( string statusName ){
            Type statusData = Type.GetType("status");
            var statusList = (List<singleStatus>)statusData.GetField("statusListSO").GetValue( this );
            var statusHolderObject = GameObject.Find( buttonClickManager.GetClassRole() + "status" );
            Transform statusPanel;
            singleStatus status;
            for( int i = 0; i < statusList.Count; i++ ){            
                if( statusList[i].name == statusName  ){
                    status = statusList[i];
                    if( status.buff ){
                        statusPanel = statusHolderObject.transform.Find( "Panel buffs" );
                    } else {
                        statusPanel = statusHolderObject.transform.Find( "Panel debuffs" ); 
                    }
                    int statusCount = statusPanel.childCount;
                    for( int x = 0; x < statusCount; x++ ){
                        var statusLabelScript = statusPanel.GetChild(x).GetComponent<statussinglelabel>();
                        if( statusLabelScript.singleStatus == status ){
                            return statusLabelScript;
                        } 
                    }
                }
            }
            return null;
        }

        public void ForceStatusOff(singleStatus singleStatus, System.Action statusAction = null)
        {
            if (statusAction != null)
            {
                statusAction();
            }
            battleDetailsManager.RemoveLabel(singleStatus.statusName, true);
            if (singleStatus.hitAnim != "")
            {
                var animationManager = GetComponent<Animation_Manager>();
                animationManager.AddStatusAnimation(false, singleStatus.hitAnim, singleStatus.holdAnim, false);
            }
        }

        public void RunStatusFunction( StatusModel statusModel ){
            //makes duration infinite if set to 0
            statusModel.duration = statusModel.duration == 0 ? Mathf.Infinity : statusModel.duration;
            switch ( statusModel.selectedStatusFunction ) {
            case singleStatus.statusFunction.AttributeChange:
                AttributeChange( statusModel );
                break;
            case singleStatus.statusFunction.AddToStat:
                AddToStatus( statusModel );
                break;
            case singleStatus.statusFunction.StatChange:
                StatChanges( statusModel );
                break;
            case singleStatus.statusFunction.StatusOn:
                StatusOn( statusModel );
                break;
            case singleStatus.statusFunction.Tumor:
                Tumor( statusModel );
                break;
            case singleStatus.statusFunction.OnHit:
                OnHit( statusModel);
                break;
            case singleStatus.statusFunction.OnHitEnemy:
                OnTakeDmg( statusModel );
                break;
            case singleStatus.statusFunction.Immune:
                SetImmunity( statusModel );
                break;
            }
        }
    }
}






