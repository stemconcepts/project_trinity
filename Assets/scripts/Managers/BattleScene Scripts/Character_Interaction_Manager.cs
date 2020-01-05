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
            if( !Battle_Manager.waitingForSkillTarget && baseManager.characterManager.characterModel.isAlive && !baseManager.skillManager.isSkillactive ){
                if( baseManager.characterManager.characterModel.characterType != Character_Model.CharacterTypeEnum.enemy ){
                    Battle_Manager.characterSelectManager.SetSelectedCharacter( baseManager.gameObject.name );
                    DisplaySkills();
                }
            } else if (Battle_Manager.waitingForSkillTarget)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null)
                {
                    var activeCharacter = Battle_Manager.characterSelectManager.GetSelectedClassObject();
                    var selectedTarget = hit.transform.gameObject.GetComponent<Character_Manager>();
                    if ((Battle_Manager.offensiveSkill && selectedTarget.tag != "Enemy") || (!Battle_Manager.offensiveSkill && selectedTarget.tag == "Enemy"))
                    {
                        print("Not a valid target, select another");
                    } else
                    {
                        activeCharacter.GetComponent<Skill_Manager>().currenttarget = selectedTarget;
                    }
                }
            }
            Battle_Manager.soundManager.playSound( Battle_Manager.soundManager.uiSounds[0] );
        }
        
        void OnMouseEnter(){
            baseManager.movementManager.currentPanel.GetComponent<Image>().color = new Color( 1f, 0.3f, 0.3f, 1f );
        }
        
        void OnMouseExit(){
            var currentPanel = baseManager.movementManager.currentPanel;
            if ( currentPanel.GetComponent<Panels_Manager>().isVoidZone )
            {
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<Panels_Manager>().voidZoneColor;
            } else if ( currentPanel.GetComponent<Panels_Manager>().isVoidCounter )
            {
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<Panels_Manager>().counterZoneColor;
            } else if (currentPanel.GetComponent<Panels_Manager>().isThreatPanel)
            {
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<Panels_Manager>().threatPanelColor;
            } else if (currentPanel.GetComponent<Panels_Manager>().isEnemyPanel)
            {
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<Panels_Manager>().enemyPanelColor;
            } else {
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<Panels_Manager>().panelColor;
            }
        }
    }
}

