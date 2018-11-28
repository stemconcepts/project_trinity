using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class character_Manager : BasicManager
    {
        public CharacterModel characterModel { get; set; } = new CharacterModel();
        public Animation_Manager animationManager { get; set; } = new Animation_Manager();
        public Movement_Manager movementManager { get; set; } = new Movement_Manager();
        public Status_Manager statusManager { get; set; } = new Status_Manager();

        public character_Manager()
        {   
            //characterModel.actionPointsDisplay = null;
            //characterModel.posMarker = null;
            //characterModel.currentPanel = null;
            updateBarSize();
            animationManager = GetComponent<Animation_Manager>();
            movementManager = GetComponent<Movement_Manager>();

            characterModel.currentPanel.GetComponent<movementPanelController>().isOccupied = true;
            characterModel.currentPanel.GetComponent<movementPanelController>().currentOccupier = gameObject;
            characterModel.origPosition = this.transform.position;
        }

        void Update(){
            updateBarSize();
            maintainHealthValue();
            characterModel.attackedPos.x = characterModel.posMarker.transform.position.x;
            characterModel.attackedPos.y = characterModel.posMarker.transform.position.y;
            characterModel.currentPosition = this.transform.position;
            characterModel.currentRotation = this.transform.rotation;
            characterModel.availableActionPoints.text = characterModel.actionPoints.ToString();
        }

        void updateBarSize(){
            characterModel.current_health = characterModel.Health;
            characterModel.sliderScript.maxValue = characterModel.full_health;
            characterModel.sliderScript.value = characterModel.current_health;
            characterModel.apSliderScript.maxValue = characterModel.maxactionPoints;
            characterModel.apSliderScript.value = characterModel.actionPoints;
        }

        void maintainHealthValue(){ 
            if( characterModel.current_health > characterModel.maxHealth ){
                characterModel.Health = characterModel.maxHealth;
            } else if( characterModel.current_health <= 0 && characterModel.isAlive ){
                if ( !animationManager.inAnimation ){
                    characterModel.isAlive = false;
                    characterModel.Health = 0;
                    statusManager.RunStatusFunction( characterModel.deathStatus, 0f, 0f );
                }
            }
        }

        void ResetAbsorbPoints(){
            if( characterModel.absorbPoints <= 0 && statusManager.DoesStatusExist( "damageAbsorb" ) ){
                characterModel.absorbPoints = 0;
                statusManager.ForceStatusOff( statusManager.GetStatus( "damageAbsorb" ) );
            }
            if( characterModel.blockPoints <= 0 && statusManager.DoesStatusExist( "block" ) ){
                characterModel.blockPoints = 0;
                statusManager.ForceStatusOff( statusManager.GetStatus( "block" ) );
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
                Type charModel = Type.GetType("CharacterModel");
                return charModel.GetField(attributeName) != null ? true : false;
                if( charModel.GetField(attributeName) != null ){
                    return true;
                } else {
                    Game_Manager.logger.Log(attributeName, "This attribute does not exists");
                    return false;
                }
            } else {
                return false;
                Game_Manager.logger.Log(attributeName, "No attribute given");
            }
        }

        public float GetAttributeValue( string attributeName ){
            if( DoesAttributeExist(attributeName) ){
                Type charModel = Type.GetType("CharacterModel");
                var attributeValue = (float)charModel.GetField(attributeName).GetValue( this );
                return attributeValue;
            } 
            return 0;
        }

        public void SetAttribute( string attributeName, float value ){
            if( DoesAttributeExist(attributeName) ){
                Type charModel = Type.GetType("CharacterModel");
                charModel.GetField(attributeName).SetValue( this, value );
            } 
        }

    }
}

