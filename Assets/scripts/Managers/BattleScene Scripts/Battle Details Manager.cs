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

        public void AddStacks(StatusLabelModel singleStatusLabel)
        {
            var stackLabel = singleStatusLabel.gameObject.GetComponentInChildren<Text>();
            stackLabel.text = singleStatusLabel.stacks < 1 ? "" : singleStatusLabel.stacks.ToString();
        }

        public void ShowLabel( StatusModel status ){
            GameObject live_object = status.singleStatus.buff ? 
                (GameObject)Instantiate( status_symbol, new Vector3 ( status_position.position.x, 0.5f + status_position.position.y, status_position.position.z ) , status_position.rotation ) : 
                (GameObject)Instantiate( status_symbol, new Vector3 ( status_position.position.x, status_position.position.y, status_position.position.z  ) , status_position.rotation );
            Image iconImageScript = live_object.GetComponent<Image>();
            iconImageScript.sprite = status.singleStatus.labelIcon;
            if( buffParent && status.singleStatus.buff ){
                live_object.transform.SetParent( buffParent.transform, true );
            } else if( debuffParent && !status.singleStatus.buff  ){
                live_object.transform.SetParent( debuffParent.transform, true );
            }
            var getprefab = live_object.GetComponent<StatusLabelModel>();
            getprefab.buff = status.singleStatus.buff;
            getprefab.statusModel = status;
            getprefab.statusname = status.singleStatus.name;
            getprefab.dispellable = status.singleStatus.debuffable;
        }

        public void RemoveLabel( StatusLabelModel statusLabel ){
            statusLabel.DestroyMe();
        }

        public void getDmg( DamageModel damageModel, string extraInfo = "" ){
            ShowDamageNumber( damageModel, extraInfo: extraInfo );
        }
    
        public void getHeal( DamageModel damageModel){
            ShowHealNumber( damageModel );
        }
    
        public void getAbsorb( float absorbValue, DamageModel damageModel ){
            ShowAbsorbNumber( damageModel );
        }
    
        public void Immune( DamageModel damageModel ){
            ShowImmune( damageModel );
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
    
        public void ShowAbsorbNumber( DamageModel damageModel ){
            GameObject charObject = damageModel.characterManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x + 1f , charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<damagDataBehaviour>();
            damageData.absorbData = (int)damageModel.absorbAmount;
            damageData.isAbsorb = true;
            damageData.skillLabel = damageModel.skillSource;
        }
    
        public void ShowImmune( DamageModel damageModel){
            GameObject charObject = damageModel.characterManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x + 1f , charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<damagDataBehaviour>();
            damageData.isImmune = true;
            damageData.skillLabel = damageModel.skillSource;
        }
    }
}



