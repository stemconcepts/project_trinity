using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace AssemblyCSharp
{
    public class Character_Manager : MonoBehaviour
    {
        public bool initialSetupDone;
        public Base_Character_Manager baseManager;
        [Header("Character Template")]
        public Character_Template template;
        public bool useStats;
        public bool useResistance;
        [Header("Character Model")]
        public Character_Model characterModel;
        [Header("Character Resistances")]
        public Resistances resistances;
        [Header("Health Object:")]
        public GameObject healthBar;
        /*[Header("Action Object:")]
        public GameObject actionBar;
        private Text actionPointsText;*/
    
        void Awake(){
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            if (gameObject.tag == "Enemy" && characterModel.role == Character_Model.RoleEnum.boss && healthBar == null)
            {
                healthBar = GameObject.Find("Slider_enemy");
            }
            if (template != null)
            {
                resistances = useResistance && template != null ? template.resistances : resistances;
                characterModel = useStats && template != null ? template.characterModel : characterModel;
            }
        }

        void Start()
        {
            if (characterModel != null)
            {
                characterModel.SetUp();
                characterModel.healthBarText = healthBar.gameObject.transform.Find("healthdata").GetComponent<Text>();
                characterModel.sliderScript = healthBar.GetComponent<Slider>();
                updateBarSize();
                initialSetupDone = true;
            }
            if (baseManager.movementManager.currentPanel == null && gameObject.tag == "Enemy")
            {
                baseManager.movementManager.currentPanel = Battle_Manager.GetRandomPanel(false);
            }
            if (baseManager.movementManager.currentPanel != null)
            {
                baseManager.movementManager.currentPanel.GetComponent<Panels_Manager>().currentOccupier = gameObject;
            }
        }

        void Update()
        {
            if(initialSetupDone)
            {
                updateBarSize();
                ResetAbsorbPoints();
                maintainHealthValue();
                characterModel.attackedPos = baseManager.movementManager.posMarker.transform.position;
                characterModel.currentRotation = this.transform.rotation;
            }
        }

        void updateBarSize(){
            characterModel.current_health = characterModel.Health;
            characterModel.sliderScript.maxValue = characterModel.full_health;
            characterModel.sliderScript.value = characterModel.current_health;
            characterModel.healthBarText.text = characterModel.current_health.ToString();
            /*if( actionBar ){
                characterModel.apSliderScript.maxValue = characterModel.maxactionPoints;
                characterModel.apSliderScript.value = characterModel.actionPoints;
            }*/
        }

        void maintainHealthValue(){ 
            if( characterModel.current_health > characterModel.maxHealth ){
                characterModel.Health = characterModel.maxHealth;
            } else if( characterModel.current_health <= 0 && characterModel.isAlive ){
                characterModel.isAlive = false;
                Battle_Manager.characterSelectManager.UpdateCharacters(this.gameObject.name);
                //if ( !baseManager.animationManager.inAnimation ){
                    characterModel.Health = 0;
                    var duration = baseManager.animationManager.PlayAnimation("death");
                    Battle_Manager.gameEffectManager.FadeOutSpine(baseManager.animationManager.skeletonAnimation);

                    foreach (var task in Battle_Manager.taskManager.taskList)
                    {
                        if (task.Key == baseManager.name)
                        {
                            Battle_Manager.taskManager.taskList.Remove(task.Key);
                        }
                    }
                    Battle_Manager.taskManager.CallTask(duration, () =>
                    {
                        if (characterModel.characterType == Character_Model.CharacterTypeEnum.enemy)
                        {
                            Destroy(this.gameObject);
                            var dataUI = GameObject.Find(this.gameObject.name + "_data");
                            Destroy(dataUI);
                        }
                    });
                //}
            }
        }

        void ResetAbsorbPoints(){
            if( characterModel.absorbPoints <= 0 && baseManager.statusManager.DoesStatusExist( "damageAbsorb" ) ){
                characterModel.absorbPoints = 0;
                baseManager.statusManager.ForceStatusOff( baseManager.statusManager.GetStatus( "damageAbsorb" ) );
            }
            if( characterModel.blockPoints <= 0 && baseManager.statusManager.DoesStatusExist( "block" ) ){
                characterModel.blockPoints = 0;
                baseManager.statusManager.ForceStatusOff( baseManager.statusManager.GetStatus( "block" ) );
            }
        }

        /*public void UpdateAPAmount()
        {
            if (characterModel.actionPoints > characterModel.maxactionPoints)
            {
                characterModel.actionPoints = characterModel.maxactionPoints;
            }
            actionPointsText.text = ((int)characterModel.actionPoints).ToString();
        }*/

        /*void RegenApStart(){
            Battle_Manager.taskManager.CallTask( 6f, ()=> {
                if( characterModel.actionPoints < characterModel.maxactionPoints ){
                    characterModel.actionPoints += characterModel.vigor;
                } 
                RegenApStart();
            });
        }*/

        public bool DoesAttributeExist( string attributeName ){
            if( !string.IsNullOrEmpty(attributeName) ){
                return this.characterModel.GetType().GetField(attributeName) != null ? true : false;
            } else {
                Game_Manager.logger.Log(attributeName, "No attribute given");
                return false;
            }
        }

        public float GetAttributeValue( string attributeName ){
            if( DoesAttributeExist(attributeName) ){
                var attributeValue = (float)this.characterModel.GetType().GetField(attributeName).GetValue(this.characterModel);
                return attributeValue;
            } 
            return 0;
        }

        public void SetAttribute( string attributeName, float value ){
            if( DoesAttributeExist(attributeName) ){
                this.characterModel.GetType().GetField(attributeName).SetValue( this.characterModel, value );
            } 
        }

        public float GetResistanceValue(string resistance)
        {
            if (DoesResistanceExist(resistance))
            {
                var resistanceValue = (float)this.resistances.GetType().GetField(resistance).GetValue(this.resistances);
                return resistanceValue;
            }
            return 0;
        }

        public bool DoesResistanceExist(string resistance)
        {
            if (!string.IsNullOrEmpty(resistance))
            {
                return this.resistances.GetType().GetField(resistance) != null ? true : false;
            }
            else
            {
                Game_Manager.logger.Log(resistance, "No attribute given");
                return false;
            }
        }

        public bool GetChanceToBeHit(float accuracy)
        {
            var accuracyRemaining = accuracy - characterModel.evasion;
            var chanceToHit = UnityEngine.Random.Range(0.0f, 1.0f);
            return accuracyRemaining >= chanceToHit;
        }

        public void SetResistance(string resistance, float value)
        {
            if (DoesResistanceExist(resistance))
            {
                this.characterModel.GetType().GetField(resistance).SetValue(this.resistances, value);
            }
        }

        public Base_Character_Manager GetTarget( bool isAutoAttack = false){
            /*if (this.tag == "Enemy")
            {
                print(Battle_Manager.IsTankInThreatZone());
            }*/
            
            if (isAutoAttack && this.tag == "Enemy" && Battle_Manager.IsTankInThreatZone())
            {
                return Battle_Manager.characterSelectManager.guardianObject.GetComponent<Base_Character_Manager>();
            }
            var enemyCharacters = Battle_Manager.GetCharacterManagers(false);
            var friendlyCharacters = Battle_Manager.GetCharacterManagers(true);
            var targetCount = this.tag == "Player" ? enemyCharacters.Count : friendlyCharacters.Count;
            var i = UnityEngine.Random.Range(0, targetCount);
            return this.tag == "Player" ? enemyCharacters[i].GetComponent<Base_Character_Manager>() : friendlyCharacters[i].GetComponent<Base_Character_Manager>();
        }

        public Base_Character_Manager GetFriendlyTarget()
        {
            var otherTargets = characterModel.characterType == Character_Model.CharacterTypeEnum.enemy ? Battle_Manager.GetCharacterManagers(false) : Battle_Manager.GetCharacterManagers(true);
            otherTargets = otherTargets.Where(o => o.name != gameObject.name).ToList();
            var targetCount = otherTargets.Count;
            var i = UnityEngine.Random.Range(0, targetCount);
            return otherTargets[i].GetComponent<Base_Character_Manager>();
        }
    }
}

