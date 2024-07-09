using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.scripts.Models.statusModels;

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

        public void SelectUnit(GameObject characterObject = null)
        {
            if (!BattleManager.waitingForSkillTarget && baseManager.characterManager.characterModel.isAlive /*&& !baseManager.skillManager.isSkillactive*/ && !baseManager.statusManager.DoesStatusExist(StatusNameEnum.Stun))
            {
                if (baseManager.characterManager.characterModel.characterType != CharacterModel.CharacterTypeEnum.enemy)
                {
                    BattleManager.characterSelectManager.SetSelectedCharacter(baseManager.gameObject.name);
                }
            }
            else if (BattleManager.waitingForSkillTarget)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                var clickedObject = characterObject == null ? hit.transform.gameObject : characterObject;
                var tag = clickedObject.tag.ToLower();

                if (tag.ToLower() == "enemy" || tag.ToLower() == "player")
                {
                    var activeCharacter = BattleManager.characterSelectManager.GetSelectedClassObject();
                    var isEnemy = tag.ToLower() == "enemy";
                    if ((BattleManager.offensiveSkill && !isEnemy) || (!BattleManager.offensiveSkill && isEnemy))
                    {
                        print("Not a valid target, select another");
                    }
                    else
                    {
                        if (isEnemy)
                        {
                            activeCharacter.GetComponent<PlayerSkillManager>().currenttarget = clickedObject.GetComponent<EnemyCharacterManager>();
                        }
                        else
                        {
                            activeCharacter.GetComponent<PlayerSkillManager>().currenttarget = clickedObject.GetComponent<CharacterManager>();
                        }
                    }
                }
            }
        }

        void OnMouseUp(){
            SelectUnit();
        }
        
        void OnMouseEnter(){
            var panelController = baseManager.movementManager.currentPanel.GetComponent<PanelsManager>();
            baseManager.movementManager.currentPanel.GetComponent<Image>().color = new Color( 1f, 0.3f, 0.3f, 1f );
            panelController.ShowSelected(true);
        }
        
        void OnMouseExit(){
            var currentPanel = baseManager.movementManager.currentPanel;
            var panelController = currentPanel.GetComponent<PanelsManager>();
            var image = currentPanel.GetComponent<Image>();
            if (panelController.isVoidZone )
            {
                image.color = panelController.voidZoneColor;
            } else if ( currentPanel.GetComponent<PanelsManager>().isVoidCounter )
            {
                image.color = panelController.counterZoneColor;
            } else if (currentPanel.GetComponent<PanelsManager>().isThreatPanel)
            {
                image.color = panelController.threatPanelColor;
            } else if (currentPanel.GetComponent<PanelsManager>().isEnemyPanel)
            {
                image.color = panelController.enemyPanelColor;
            } else {
                image.color = panelController.panelColor;
            }
            panelController.ShowSelected(false);
        }
    }
}

