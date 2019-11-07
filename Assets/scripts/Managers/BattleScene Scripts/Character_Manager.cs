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
        public Base_Character_Manager baseManager;
        [Header("Character Model")]
        public Character_Model characterModel;
        [Header("Health Object:")]
        public GameObject healthBar;
        [Header("Action Object:")]
        public GameObject actionBar;
        [Header("Current Object:")]
        public GameObject currentPanel;
    
        void Awake(){
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            //characterModel = this.gameObject.GetComponent<Character_Model>();
        }

        void Start()
        {   
            if( this.currentPanel != null && this.characterModel != null ){
                characterModel.healthBarText = healthBar.gameObject.transform.Find("healthdata").GetComponent<Text>();
                characterModel.SetUp();
                baseManager.movementManager.currentPanel = this.currentPanel;
                baseManager.movementManager.currentPanel.GetComponent<Panels_Manager>().isOccupied = true;
                baseManager.movementManager.currentPanel.GetComponent<Panels_Manager>().currentOccupier = gameObject;
                characterModel.target = GetTarget();
                characterModel.sliderScript = healthBar.GetComponent<Slider>();
                characterModel.apSliderScript = actionBar != null ? actionBar.GetComponent<Slider>() : null;
                updateBarSize();
            }
        }

        void Update(){
            if( characterModel != null ){
                updateBarSize();
                maintainHealthValue();
                characterModel.attackedPos = baseManager.movementManager.posMarker.transform.position;
                characterModel.currentRotation = this.transform.rotation;
                if( characterModel.availableActionPoints ){
                    characterModel.availableActionPoints.text = characterModel.actionPoints.ToString();
                }
            }
        }

        void updateBarSize(){
            characterModel.current_health = characterModel.Health;
            characterModel.sliderScript.maxValue = characterModel.full_health;
            characterModel.sliderScript.value = characterModel.current_health;
            characterModel.healthBarText.text = characterModel.current_health.ToString();
            if( actionBar ){
                characterModel.apSliderScript.maxValue = characterModel.maxactionPoints;
                characterModel.apSliderScript.value = characterModel.actionPoints;
            }
        }

        void maintainHealthValue(){ 
            if( characterModel.current_health > characterModel.maxHealth ){
                characterModel.Health = characterModel.maxHealth;
            } else if( characterModel.current_health <= 0 && characterModel.isAlive ){
                if ( !baseManager.animationManager.inAnimation ){
                    characterModel.isAlive = false;
                    characterModel.Health = 0;
                    if( characterModel.deathStatus != null){
                        var sM = new StatusModel(){
                            singleStatus = characterModel.deathStatus.singleStatus,
                            duration = 0f
                        };
                        baseManager.statusManager.RunStatusFunction( sM );
                    }
                }
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

        void RegenApStart(){
            Battle_Manager.taskManager.CallTask( 7f, ()=> {
                if( characterModel.actionPoints < characterModel.maxactionPoints ){
                    characterModel.actionPoints += characterModel.vigor;
                } 
                RegenApStart();
            });
        }

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

        public Base_Character_Manager GetTarget(){
            var enemyCharacters = Battle_Manager.GetCharacterManagers(false);
            var friendlyCharacters = Battle_Manager.GetCharacterManagers(true);
            var targetCount = this.tag == "Player" ? enemyCharacters.Count : friendlyCharacters.Count;
            var i = UnityEngine.Random.Range(0, targetCount);
            return this.tag == "Player" ? enemyCharacters[i].GetComponent<Base_Character_Manager>() : friendlyCharacters[i].GetComponent<Base_Character_Manager>();
        }
    }
}

