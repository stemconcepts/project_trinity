using System;
using TMPro;
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
                if (warningTextHolder != null)
                {
                    //warningText.text = "";
                    warningTextHolder.SetActive(false);
                }
            }, "battleWarningTask");
        }

        public void AddStacks(StatusLabelModel singleStatusLabel)
        {
            var stackLabel = singleStatusLabel.gameObject.GetComponentInChildren<TextMeshPro>();
            stackLabel.text = singleStatusLabel.stacks < 1 ? "" : singleStatusLabel.stacks.ToString();

            BattleManager.combatLogManager.GetStatusGainDescription(singleStatusLabel.statusModel);
        }

        public void ShowLabel(StatusModel status, GameObject statusHolder){
            GameObject live_object = Instantiate(status_symbol, statusHolder.transform);
            Image iconImageScript = live_object.GetComponent<Image>();
            iconImageScript.sprite = status.singleStatus.labelIcon;
            var getprefab = live_object.GetComponent<StatusLabelModel>();
            var statusTransform = live_object.GetComponent<RectTransform>();
            statusTransform.localScale = new Vector3(1, 1, 1);
            getprefab.buffPower = status.power;
            getprefab.statusModel = status;
            getprefab.dispellable = status.singleStatus.dispellable;

            BattleManager.combatLogManager.GetStatusGainDescription(status); 
        }

        public void RemoveLabel(StatusLabelModel statusLabel){
            if (statusLabel)
            {
                statusLabel.DestroyMe();

                BattleManager.combatLogManager.GetStatusExpireDescription(statusLabel.statusModel);
            }
        }

        public void getDmg(BaseDamageModel damageModel, string extraInfo = ""){

            BattleManager.combatLogManager.GetDamageDescription(damageModel, extraInfo);

            ShowDamageNumber(damageModel);
        }
    
        public void getHeal(BaseDamageModel damageModel){

            BattleManager.combatLogManager.GetHealingDescription(damageModel);

            ShowHealNumber(damageModel);
        }
    
        public void Immune(BaseDamageModel damageModel){
            ShowImmune(damageModel);
        }
    
        //controls damage/healing/absorb numbers
        public void ShowDamageNumber(BaseDamageModel damageModel, string extraInfo = ""){
            Debug.Log($"ShowDamageNumber : damageModel : {damageModel != null}, baseManager : {damageModel.baseManager != null}");

            Transform charObject = damageModel?.baseManager.transform;
            if(charObject == null)
            {
                return;
            }
            var gameDetails = Instantiate(gameDetailsObject, new Vector2 (charObject.position.x, charObject.position.y + 6f ), charObject.rotation);
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
    
        public void ShowAbsorbNumber(BaseDamageModel damageModel, float initialAbsorbAmount ){
            GameObject charObject = damageModel.baseManager.gameObject;
            var gameDetails = (GameObject)Instantiate( gameDetailsObject, new Vector2 ( charObject.transform.position.x + 1f , charObject.transform.position.y + 6f ) , charObject.transform.rotation );
            var damageData = gameDetails.GetComponent<Floating_Combat_Text>();
            damageData.fontSize = damageModel.fontSize;
            damageData.extraInfo = $"absorbed: {(int)damageModel.damageAbsorbed}/{(int)initialAbsorbAmount}";
            damageData.isAbsorb = true;
            damageData.skillLabel = damageModel.skillSource;

            BattleManager.combatLogManager.GetAbsorbDescription(damageModel, damageModel.damageAbsorbed.ToString());
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



