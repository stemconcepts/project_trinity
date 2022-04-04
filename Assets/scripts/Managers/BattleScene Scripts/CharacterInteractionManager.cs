using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class CharacterInteractionManager : MonoBehaviour
    {
        public AudioClip characterSelectSound;
        public BaseCharacterManagerGroup baseManager;
        void Start()
        {
            baseManager = this.gameObject.GetComponent<BaseCharacterManagerGroup>();
        }

        public void DisplaySkills(){
            for( int x = 0; x < BattleManager.battleInterfaceManager.Count ; x++ ){
                BattleManager.battleInterfaceManager[x].SkillSet((PlayerSkillManager)baseManager.skillManager);
            }
        }

        void OnMouseUp(){
            //var charManager = this.gameObject.tag == "Enemy" ? (Character_Manager)baseManager.characterManager : (Enemy_Character_Manager)baseManager.characterManager;

            if (!BattleManager.waitingForSkillTarget && baseManager.characterManager.characterModel.isAlive && !baseManager.skillManager.isSkillactive && !baseManager.statusManager.DoesStatusExist("stun") ){
                if(baseManager.characterManager.characterModel.characterType != CharacterModel.CharacterTypeEnum.enemy ){
                    BattleManager.characterSelectManager.SetSelectedCharacter( baseManager.gameObject.name );
                    BattleManager.soundManager.playSound(characterSelectSound);
                    //DisplaySkills();
                }
            } else if (BattleManager.waitingForSkillTarget)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null)
                {
                    var activeCharacter = BattleManager.characterSelectManager.GetSelectedClassObject();
                    var isEnemy = hit.transform.gameObject.tag == "Enemy";
                    //var selectedTarget = hit.transform.gameObject.GetComponent<BaseCharacterManager>();
                    if ((BattleManager.offensiveSkill && !isEnemy) || (!BattleManager.offensiveSkill && isEnemy))
                    {
                        print("Not a valid target, select another");
                    } else
                    {
                        if (isEnemy)
                        {
                            activeCharacter.GetComponent<PlayerSkillManager>().currenttarget = hit.transform.gameObject.GetComponent<EnemyCharacterManager>();
                        } else
                        {
                            activeCharacter.GetComponent<PlayerSkillManager>().currenttarget = hit.transform.gameObject.GetComponent<CharacterManager>();
                        }
                    }
                }
            }
        }
        
        void OnMouseEnter(){
            baseManager.movementManager.currentPanel.GetComponent<Image>().color = new Color( 1f, 0.3f, 0.3f, 1f );
        }
        
        void OnMouseExit(){
            var currentPanel = baseManager.movementManager.currentPanel;
            if ( currentPanel.GetComponent<PanelsManager>().isVoidZone )
            {
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<PanelsManager>().voidZoneColor;
            } else if ( currentPanel.GetComponent<PanelsManager>().isVoidCounter )
            {
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<PanelsManager>().counterZoneColor;
            } else if (currentPanel.GetComponent<PanelsManager>().isThreatPanel)
            {
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<PanelsManager>().threatPanelColor;
            } else if (currentPanel.GetComponent<PanelsManager>().isEnemyPanel)
            {
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<PanelsManager>().enemyPanelColor;
            } else {
                currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<PanelsManager>().panelColor;
            }
        }
    }
}

