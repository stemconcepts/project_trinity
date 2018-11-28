using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Status_Manager : BasicManager
    {
        Battle_Details_Manager battleDetailsManager = new Battle_Details_Manager();
        public Status_Manager()
        {
            
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
                Battle_Manager.taskManager.CallStatusTask( statusModel.duration, statusModel.singleStatus );

                StartCoroutine( DurationTimer( statusModel.duration, statusModel.singleStatus, ()=> {
                        GetComponent<character_Manager>().SetAttribute( statusModel.singleStatus.attributeName, originalStat);
                    })
                );        
            } else 
            if( statusModel.turnOff ){
                ForceStatusOff( statusDetail.singleStatus, ()=> {
                    GetComponent<character_Manager>().SetAttribute( statusModel.singleStatus.attributeName, originalStat);
                });
            }
        }

        public bool DoesStatusExist( string statusName ){
            Type statusModel = Type.GetType("statusModel");
            var statusList = (List<singleStatus>)statusModel.GetField("statusListSO").GetValue( this );
            var statusHolderObject = GameObject.Find( buttonClickScript.GetClassRole() + "status" );
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

        public statussinglelabel GetStatusIfExist( string statusName ){
            Type statusData = Type.GetType("status");
            var statusList = (List<singleStatus>)statusData.GetField("statusListSO").GetValue( this );
            var statusHolderObject = GameObject.Find( buttonClickScript.GetClassRole() + "status" );
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

        public void RunStatusFunction( StatusModel statusModel ){
            //makes duration infinite if set to 0
            statusModel.duration = statusModel.duration == 0 ? Mathf.Infinity : statusModel.duration;
            switch ( singleStatus.selectedStatusFunction ) {
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

