using System;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class BattleDetailsManager : MonoBehaviour
    {
        public GameObject status_symbol;
        public GameObject gameDetailsObject;
        public GameObject movementArrowObject;
        public GameObject warningTextHolder;
        //public GameObject canvasHolder;
        private Text warningText;

        void Start(){
            warningText = warningTextHolder.GetComponent<Text>();
            warningTextHolder.SetActive(false);
        }

        public void BattleWarning(string message, float showTime)
        {
            warningTextHolder.SetActive(true);
            warningText.text = message;
            BattleManager.taskManager.CallTask(showTime, () =>
            {
                //warningText.text = "";
                warningTextHolder.SetActive(false);
            }, "battleWarningTask");
        }

        public void AddStacks(StatusLabelModel singleStatusLabel)
        {
            var stackLabel = singleStatusLabel.gameObject.GetComponentInChildren<Text>();
            stackLabel.text = singleStatusLabel.stacks < 1 ? "" : singleStatusLabel.stacks.ToString();
        }

        public void ShowLabel(StatusModel status, GameObject statusHolder){
            GameObject live_object = Instantiate(status_symbol, statusHolder.transform); /*status.singleStatus.buff ? 
                (GameObject)Instantiate( status_symbol, new Vector3 ( statusHolder.transform.position.x, 0.5f + statusHolder.transform.position.y, statusHolder.transform.position.z ) , statusHolder.transform.rotation ) : 
                (GameObject)Instantiate( status_symbol, new Vector3 ( statusHolder.transform.position.x, statusHolder.transform.position.y, statusHolder.transform.position.z  ) , statusHolder.transform.rotation );*/
            Image iconImageScript = live_object.GetComponent<Image>();
            iconImageScript.sprite = status.singleStatus.labelIcon;
            //live_object.transform.SetParent(statusHolder.transform, true);
            /*if (!status.singleStatus.buff){
                live_object.transform.SetParent(statusHolder.transform.GetChild(0).transform, true);
            } else if(status.singleStatus.buff){
                live_object.transform.SetParent(statusHolder.transform.GetChild(1).transform, true);
            }*/
            var getprefab = live_object.GetComponent<StatusLabelModel>();
            getprefab.buff = status.singleStatus.buff;
            getprefab.buffPower = status.power;
            getprefab.statusModel = status;
            getprefab.statusname = status.singleStatus.name;
            getprefab.dispellable = status.singleStatus.dispellable;
        }

        public void RemoveLabel(StatusLabelModel statusLabel){
            if (statusLabel)
            {
                statusLabel.DestroyMe();
            }
        }

        public void getDmg(BaseDamageModel damageModel, string extraInfo = ""){
            ShowDamageNumber(damageModel, extraInfo: extraInfo);
        }
    
        public void getHeal(BaseDamageModel damageModel){
            ShowHealNumber(damageModel);
        }
    
        public void Immune(BaseDamageModel damageModel){
            ShowImmune(damageModel);
        }
    
        //controls damage/healing/absorb numbers
        public void ShowDamageNumber(BaseDamageModel damageModel, string extraInfo = ""){
            GameObject charObject = damageModel.baseManager.gameObject;
            var gameDetails = (GameObject)Instantiate(gameDetailsObject, new Vector2 (charObject.transform.position.x, charObject.transform.position.y + 6f ), charObject.transform.rotation);
            var damageData = gameDetails.GetComponent<Floating_Combat_Text>();
            damageData.fontSize = damageModel.fontSize;
            damageData.textColor = damageModel.textColor;
            damageData.showDmgNumber = damageModel.showDmgNumber;
            damageData.damageData =  (int)damageModel.damageTaken;
            damageData.extraInfo = $"{extraInfo}{(damageModel.modifiedDamage ? "*" : "")}";
            damageData.isDmg = true;
            damageData.skillLabel = damageModel.skillSource;
        }
    
        public void ShowHealNumber(BaseDamageModel damageModel ){
            GameObject charObject = damageModel.baseManager.gameObject;
            var gameDetails = (GameObject)Instantiate(gameDetailsObject, new Vector2 (charObject.transform.position.x + 1f, charObject.transform.position.y + 6f ), charObject.transform.rotation);
            var damageData = gameDetails.GetComponent<Floating_Combat_Text>();
            damageData.fontSize = damageModel.fontSize;
            damageData.healData = (int)damageModel.incomingHeal;
            damageData.isDmg = false;
        }
    
        public void ShowAbsorbNumber(BaseDamageModel damageModel, float absorbAmount ){
            GameObject charObject = damageModel.baseManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x + 1f , charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<Floating_Combat_Text>();
            damageData.fontSize = damageModel.fontSize;
            damageData.extraInfo = "<size=100><i>absorbed: " + (int)damageModel.damageAbsorbed + "/" + (int)absorbAmount + "</i></size>";
            damageData.isAbsorb = true;
            damageData.skillLabel = damageModel.skillSource;
        }
    
        public void ShowImmune(BaseDamageModel damageModel){
            GameObject charObject = damageModel.baseManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x + 1f , charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<Floating_Combat_Text>();
            damageData.textColor = damageModel.textColor;
            damageData.isImmune = true;
            damageData.skillLabel = damageModel.skillSource;
        }

        public void DestroyObject(GameObject gO ){
            if( gO ){
                Destroy (gO);
            }
        }
    }
}



