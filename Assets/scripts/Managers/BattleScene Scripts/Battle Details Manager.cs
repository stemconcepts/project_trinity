using System;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Battle_Details_Manager : BasicManager
    {
        public GameObject status_symbol { get; set; }
        public Transform status_position { get; set; }
        public GameObject buffParent { get; set; }
        public GameObject debuffParent { get; set; }
        public GameObject gameDetailsObject {get; set; }
        public Status_Manager statusManager {get; set;}
        public Battle_Details_Manager()
        {
            statusManager = GetComponent<Status_Manager>();
        }

        public void AddStacks(statussinglelabel singleStatusLabel)
        {
            var stackLabel = singleStatusLabel.gameObject.GetComponentInChildren<Text>();
            stackLabel.text = singleStatusLabel.stacks < 1 ? "" : singleStatusLabel.stacks.ToString();
        }

        public void ShowLabel( singleStatus status ){
            GameObject live_object = status.buff ? 
                (GameObject)Instantiate( status_symbol, new Vector3 ( status_position.position.x, 0.5f + status_position.position.y, status_position.position.z ) , status_position.rotation ) : 
                (GameObject)Instantiate( status_symbol, new Vector3 ( status_position.position.x, status_position.position.y, status_position.position.z  ) , status_position.rotation );
            Image iconImageScript = live_object.GetComponent<Image>();
            iconImageScript.sprite = status.labelIcon;
            if( buffParent && status.buff ){
                live_object.transform.SetParent( buffParent.transform, true );
            } else if( debuffParent && !status.buff  ){
                live_object.transform.SetParent( debuffParent.transform, true );
            }
            var getprefab = live_object.GetComponent<statussinglelabel>();
            getprefab.buff = status.buff;
            getprefab.singleStatus = status;
            getprefab.statusname = status.name;
            getprefab.dispellable = status.debuffable;
        }

        public void RemoveLabel( string statuslabel, bool isbuff ){
            if ( statusManager.DoesStatusExist(statuslabel) ){
                var chosenStatus = statusManager.GetStatusIfExist( statuslabel );
                chosenStatus.DestroyMe();
            }
        }

        public void getDmg( DamageModel damageModel, string extraInfo = "" ){
            ShowDamageNumber( damageModel, extraInfo: extraInfo );
        }
    
        public void getHeal( DamageModel damageModel){
            ShowHealNumber( damageModel );
        }
    
        public void getAbsorb( float absorbValue, DamageModel damageModel ){
            ShowAbsorbNumber( damageModel.dmgSource, absorbValue );
        }
    
        public void Immune( DamageModel damageModel ){
            ShowImmune( damageModel );
        }

            
        public void AddStacks( statussinglelabel singleStatusLabel ){
            var stackLabel = singleStatusLabel.gameObject.GetComponentInChildren<Text>();
            stackLabel.text = singleStatusLabel.stacks < 1 ? "" : singleStatusLabel.stacks.ToString();
        }
    
        //controls damage/healing/absorb numbers
        public void ShowDamageNumber( DamageModel damageModel, string extraInfo = "" ){
            GameObject charObject = damageModel.characterManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x, charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<damagDataBehaviour>();
            damageData.damageData = (int)damageModel.incomingMDmg;
            damageData.extraInfo = extraInfo;
            damageData.isDmg = true;
            damageData.skillLabel = damageModel.skillSource;
        }
    
        public void ShowHealNumber( DamageModel damageModel ){
            GameObject charObject = damageModel.characterManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x + 1f , charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<damagDataBehaviour>();
            damageData.healData = (int)damageModel.incomingHeal;
            damageData.isDmg = false;
        }
    
        public void ShowAbsorbNumber( DamageModel damageModel, float absorbValue ){
            GameObject charObject = damageModel.characterManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x + 1f , charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<damagDataBehaviour>();
            damageData.absorbData = (int)absorbValue;
            damageData.isAbsorb = true;
            damageData.skillLabel = damageModel.dmgSource;
        }
    
        public void ShowImmune( DamageModel damageModel){
            GameObject charObject = damageModel.characterManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x + 1f , charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<damagDataBehaviour>();
            damageData.isImmune = true;
            damageData.skillLabel = damageModel.dmgSource;
        }
    }
}



