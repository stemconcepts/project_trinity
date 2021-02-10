using System;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Battle_Details_Manager : MonoBehaviour
    {
        public GameObject status_symbol;
        public GameObject gameDetailsObject;
        public GameObject movementArrowObject;
        public GameObject warningTextHolder;
        private Text warningText;

        void Start(){
            warningText = warningTextHolder.GetComponent<Text>();
            warningTextHolder.SetActive(false);
        }

        public void BattleWarning(string message, float showTime)
        {
            warningTextHolder.SetActive(true);
            warningText.text = message;
            Battle_Manager.taskManager.CallTask(showTime, () =>
            {
                //warningText.text = "";
                warningTextHolder.SetActive(false);
            });
        }

        public void AddStacks(StatusLabelModel singleStatusLabel)
        {
            var stackLabel = singleStatusLabel.gameObject.GetComponentInChildren<Text>();
            stackLabel.text = singleStatusLabel.stacks < 1 ? "" : singleStatusLabel.stacks.ToString();
        }

        public void ShowLabel( StatusModel status, GameObject statusHolder ){
            GameObject live_object = status.singleStatus.buff ? 
                (GameObject)Instantiate( status_symbol, new Vector3 ( statusHolder.transform.position.x, 0.5f + statusHolder.transform.position.y, statusHolder.transform.position.z ) , statusHolder.transform.rotation ) : 
                (GameObject)Instantiate( status_symbol, new Vector3 ( statusHolder.transform.position.x, statusHolder.transform.position.y, statusHolder.transform.position.z  ) , statusHolder.transform.rotation );
            Image iconImageScript = live_object.GetComponent<Image>();
            iconImageScript.sprite = status.singleStatus.labelIcon;
            if( !status.singleStatus.buff ){
                live_object.transform.SetParent( statusHolder.transform.GetChild(0).transform, true );
            } else if( status.singleStatus.buff  ){
                live_object.transform.SetParent( statusHolder.transform.GetChild(1).transform, true );
            }
            var getprefab = live_object.GetComponent<StatusLabelModel>();
            getprefab.buff = status.singleStatus.buff;
            getprefab.statusModel = status;
            getprefab.statusname = status.singleStatus.name;
            getprefab.dispellable = status.singleStatus.dispellable;
        }

        public void RemoveLabel( StatusLabelModel statusLabel ){
            if (statusLabel)
            {
                statusLabel.DestroyMe();
            }
        }

        public void getDmg( DamageModel damageModel, string extraInfo = "" ){
            ShowDamageNumber( damageModel, extraInfo: extraInfo );
        }
    
        public void getHeal( DamageModel damageModel){
            ShowHealNumber( damageModel );
        }
    
        public void Immune( DamageModel damageModel ){
            ShowImmune( damageModel );
        }
    
        //controls damage/healing/absorb numbers
        public void ShowDamageNumber( DamageModel damageModel, string extraInfo = "" ){
            GameObject charObject = damageModel.baseManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x, charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<Floating_Combat_Text>();
            damageData.fontSize = damageModel.fontSize;
            damageData.damageData = (int)damageModel.damageTaken;
            damageData.extraInfo = extraInfo + (damageModel.modifiedDamage ? "*" : ""); ;
            damageData.isDmg = true;
            damageData.skillLabel = damageModel.skillSource;
        }
    
        public void ShowHealNumber( DamageModel damageModel ){
            GameObject charObject = damageModel.baseManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x + 1f , charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<Floating_Combat_Text>();
            damageData.fontSize = damageModel.fontSize;
            damageData.healData = (int)damageModel.incomingHeal;
            damageData.isDmg = false;
        }
    
        public void ShowAbsorbNumber( DamageModel damageModel, float absorbAmount ){
            GameObject charObject = damageModel.baseManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x + 1f , charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<Floating_Combat_Text>();
            damageData.fontSize = damageModel.fontSize;
            damageData.extraInfo = "<size=100><i>absorbed: " + (int)damageModel.damageAbsorbed + "/" + (int)absorbAmount + "</i></size>";
            damageData.isAbsorb = true;
            damageData.skillLabel = damageModel.skillSource;
        }
    
        public void ShowImmune( DamageModel damageModel){
            GameObject charObject = damageModel.baseManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x + 1f , charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<Floating_Combat_Text>();
            damageData.isImmune = true;
            damageData.skillLabel = damageModel.skillSource;
        }

        public void DestroyObject( GameObject gO ){
            if( gO ){
                Destroy (gO);
            }
        }
    }
}



