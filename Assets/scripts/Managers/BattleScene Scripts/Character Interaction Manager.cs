using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Character_Interaction_Manager : Base_Character_Manager
    {
        public Character_Interaction_Manager()
        {
        }

        public void DisplaySkills(){
            for( int x = 0; x < Battle_Manager.battleInterfaceManager.Count ; x++ ){
                Battle_Manager.battleInterfaceManager[x].SkillSet();
            }
        }
        
        void OnMouseUp(){
            if( skillManager.isSkillactive ){
                skill_targetting.instance.currentTarget = this.gameObject;
            }
            if( characterManager.characterModel.isAlive && !skillManager.isSkillactive ){
                if( characterManager.characterModel.role != "Enemy" ){
                    Battle_Manager.characterSelectManager.SetSelectedCharacter( characterManager.gameObject.name );
                    DisplaySkills();
                }
            }
            Battle_Manager.soundManager.playSound( Battle_Manager.soundManager.uiSounds[0] );
        }
        
        void OnMouseEnter(){
            characterManager.characterModel.currentPanel.GetComponent<Image>().color = new Color( 1f, 0.3f, 0.3f, 1f );
        }
        
        void OnMouseExit(){
            var currentPanel = characterManager.characterModel.currentPanel;
            if ( currentPanel.GetComponent<movementPanelController>().isVoidZone ){
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<movementPanelController>().voidZoneColor;
            } else if ( currentPanel.GetComponent<movementPanelController>().isVoidCounter ){
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<movementPanelController>().counterZoneColor;
            } else {
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<movementPanelController>().panelColor;
            }
        }
        
        void ToggleThroughLiveCharacters(){
            string swapTo = Battle_Manager.GetAlive( Battle_Manager.charClass );
            Battle_Manager.characterSelectManager.SetSelectedCharacter(swapTo);
            Battle_Manager.soundManager.playSound( Battle_Manager.soundManager.charSwapSound );
        }

        void Update () {
            if( Input.GetKeyUp( KeyCode.Tab ) ){
                ToggleThroughLiveCharacters();
            }
        }
    }
}

