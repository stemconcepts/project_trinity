using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace AssemblyCSharp
{
    public class PlayerMovementManager : BaseMovementManager
    {
        void Start()
        {
            baseManager = this.gameObject.GetComponent<CharacterManagerGroup>();
            if (baseManager.animationManager.skeletonAnimation)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventMove;
            }
            else if (baseManager.animationManager.skeletonAnimationMulti)
            {
                baseManager.animationManager.skeletonAnimationMulti.GetSkeletonAnimations().ForEach(o => {
                    o.state.Event += OnEventMove;
                });
            }
        }

        public override void CheckPanelPosition()
        {
            if (currentPosition != (Vector2)this.gameObject.transform.position)
            {
                currentPosition = this.gameObject.transform.position;
            }
            var panel = currentPanel.GetComponent<PanelsManager>();
            const int frontPanelNum = 0;
            const int backPanelNum = 2;

            switch (panel.panelNumber)
            {
                case frontPanelNum:
                    if (!isInFrontRow)
                    {
                        var eventModelFront = new EventModel
                        {
                            eventName = "OnFirstRow",
                            extTarget = baseManager.characterManager,
                            eventCaller = baseManager.characterManager
                        };
                        BattleManager.eventManager.BuildEvent(eventModelFront);
                    }
                    isInBackRow = false;
                    isInMiddleRow = false;
                    isInFrontRow = true;
                    break;
                case 1:
                    if (!isInMiddleRow)
                    {
                        var eventModelMiddle = new EventModel
                        {
                            eventName = "OnMiddleRow",
                            extTarget = baseManager.characterManager,
                            eventCaller = baseManager.characterManager
                        };
                        BattleManager.eventManager.BuildEvent(eventModelMiddle);
                    }
                    isInBackRow = false;
                    isInMiddleRow = true;
                    isInFrontRow = false;
                    break;
                case backPanelNum:
                    if (!isInBackRow)
                    {
                        var eventModelBack = new EventModel
                        {
                            eventName = "OnLastRow",
                            extTarget = baseManager.characterManager,
                            eventCaller = baseManager.characterManager
                        };
                        BattleManager.eventManager.BuildEvent(eventModelBack);
                    }
                    isInBackRow = true;
                    isInMiddleRow = false;
                    isInFrontRow = false;
                    break;
                default:
                    isInBackRow = false;
                    isInMiddleRow = false;
                    isInFrontRow = false;
                    break;
            }
        }

        bool PlayerCanMove()
        {
            return BattleManager.turn == BattleManager.TurnEnum.PlayerTurn &&
                baseManager.characterManager.characterModel.characterType == CharacterModel.CharacterTypeEnum.Player &&
                baseManager.characterManager.characterModel.Haste > ((PlayerSkillManager)baseManager.skillManager).turnsTaken;
        }

        //Spawn Move Pointer
        public void OnMouseDown()
        {
            if (!baseManager.statusManager.DoesStatusExist("stun") && !baseManager.autoAttackManager.isAttacking /*&& !baseManager.skillManager.isSkillactive*/)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var distance = Vector2.Distance(transform.position, Camera.main.transform.position);
                BattleManager.taskManager.CallTask(0.4f, () =>
                {
                    if (PlayerCanMove())
                    {
                        BattleManager.HitBoxControl(false);
                        positionArrow = (GameObject)Instantiate(BattleManager.battleDetailsManager.movementArrowObject, ray.GetPoint(distance), Quaternion.identity);
                        var movementArrowManager = positionArrow.GetComponent<MovementArrowManager>();
                        //movementArrowManager.originalPanel = baseManager.movementManager.currentPanel.GetComponent<PanelsManager>();
                        movementArrowManager.distance = distance;
                        movementArrowManager.occupier = baseManager;
                        //BattleManager.SetFadeOnAllPanels(0.5f, 0.5f);
                    }
                }, "draggingTask_" + baseManager.name);
            }
        }

        public void OnMouseUp()
        {
            if (BattleManager.taskManager.taskList.ContainsKey("draggingTask_" + baseManager.name))
            {
                BattleManager.taskManager.taskList["draggingTask_" + baseManager.name].Stop();
                BattleManager.taskManager.taskList.Remove("draggingTask_" + baseManager.name);
            }
            if (positionArrow && !baseManager.statusManager.DoesStatusExist("stun") && !baseManager.autoAttackManager.isAttacking /*&& !baseManager.skillManager.isSkillactive*/)
            {
                var positionArrowManager = positionArrow.GetComponent<MovementArrowManager>();
                if (positionArrowManager.hoveredPanel && !positionArrowManager.hoveredPanel.currentOccupier && BattleManager.actionPoints >= 1)
                {
                    BattleManager.actionPoints -= movementCost;
                    ++((PlayerSkillManager)baseManager.skillManager).turnsTaken;
                    BattleManager.UpdateAPAmount();
                    positionArrowManager.hoveredPanel.SetOrigPositionInPanel(this);
                    MoveToPanel(positionArrowManager.hoveredPanel);
                    positionArrowManager.occupier.animationManager.meshRenderer.sortingOrder = origSortingOrder = positionArrowManager.hoveredPanel.sortingLayerNumber;
                    positionArrowManager.occupier.characterManager.characterModel.rowNumber = positionArrowManager.hoveredPanel.sortingLayerNumber;
                    //BattleManager.SetFadeOnAllPanels(0f, 0.5f);
                }
                Destroy(positionArrow);
                BattleManager.HitBoxControl(true);
            }
        }
    }
}