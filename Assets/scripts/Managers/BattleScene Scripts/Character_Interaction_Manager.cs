using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Character_Interaction_Manager : MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        void Start()
        {
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
        }

        public void DisplaySkills(){
            for( int x = 0; x < Battle_Manager.battleInterfaceManager.Count ; x++ ){
                Battle_Manager.battleInterfaceManager[x].SkillSet( baseManager.skillManager );
            }
        }
        
        void OnMouseUp(){
            if( baseManager.characterManager.characterModel.isAlive && !baseManager.skillManager.isSkillactive ){
                if( baseManager.characterManager.characterModel.characterType != Character_Model.CharacterTypeEnum.enemy ){
                    Battle_Manager.characterSelectManager.SetSelectedCharacter( baseManager.gameObject.name );
                    DisplaySkills();
                }
            }
            Battle_Manager.soundManager.playSound( Battle_Manager.soundManager.uiSounds[0] );
        }
        
        void OnMouseEnter(){
            baseManager.movementManager.currentPanel.GetComponent<Image>().color = new Color( 1f, 0.3f, 0.3f, 1f );
        }
        
        void OnMouseExit(){
            var currentPanel = baseManager.movementManager.currentPanel;
            if ( currentPanel.GetComponent<Panels_Manager>().isVoidZone ){
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<Panels_Manager>().voidZoneColor;
            } else if ( currentPanel.GetComponent<Panels_Manager>().isVoidCounter ){
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<Panels_Manager>().counterZoneColor;
            } else {
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<Panels_Manager>().panelColor;
            }
        }
        
        void ToggleThroughLiveCharacters(){
            var swapTo = Battle_Manager.characterSelectManager.GetAlive();
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

