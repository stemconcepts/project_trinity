using UnityEngine;
using System.Collections;
using DG.Tweening;
using Assets.scripts.Models.statusModels;

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

            baseManager.EventManagerV2.AddDelegateToEvent(EventTriggerEnum.OnMove, () => BattleManager.AddToGearSwapPoints(2));
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
                        /*var eventModelFront = new EventModel
                        {
                            eventName = EventTriggerEnum.OnFirstRow.ToString(),
                            extTarget = baseManager.characterManager,
                            eventCaller = baseManager.characterManager
                        };
                        BattleManager.eventManager.BuildEvent(eventModelFront);*/
                        baseManager.EventManagerV2.CreateEventOrTriggerEvent(EventTriggerEnum.OnFirstRow);
                    }
                    isInBackRow = false;
                    isInMiddleRow = false;
                    isInFrontRow = true;
                    break;
                case 1:
                    if (!isInMiddleRow)
                    {
                        /*var eventModelMiddle = new EventModel
                        {
                            eventName = EventTriggerEnum.OnMiddleRow.ToString(),
                            extTarget = baseManager.characterManager,
                            eventCaller = baseManager.characterManager
                        };
                        BattleManager.eventManager.BuildEvent(eventModelMiddle);*/
                        baseManager.EventManagerV2.CreateEventOrTriggerEvent(EventTriggerEnum.OnMiddleRow);
                    }
                    isInBackRow = false;
                    isInMiddleRow = true;
                    isInFrontRow = false;
                    break;
                case backPanelNum:
                    if (!isInBackRow)
                    {
                        /*var eventModelBack = new EventModel
                        {
                            eventName = EventTriggerEnum.OnLastRow.ToString(),
                            extTarget = baseManager.characterManager,
                            eventCaller = baseManager.characterManager
                        };
                        BattleManager.eventManager.BuildEvent(eventModelBack);*/
                        baseManager.EventManagerV2.CreateEventOrTriggerEvent(EventTriggerEnum.OnLastRow);
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
            var isStunned = baseManager.statusManager.DoesStatusExist(StatusNameEnum.Stun);
            var isAttacking = baseManager.autoAttackManager.isAttacking;
            var isCasting = baseManager.skillManager.isCasting;
            var isPlayerTurn = BattleManager.turn == BattleManager.TurnEnum.PlayerTurn;
            var isPlayer = baseManager.characterManager.characterModel.characterType == CharacterModel.CharacterTypeEnum.Player;
            var hasTurnsLeft = ((PlayerSkillManager)baseManager.skillManager).turnsTaken < baseManager.characterManager.characterModel.Haste;
            return !isStunned && !isAttacking && !isCasting && isPlayerTurn && isPlayer && hasTurnsLeft;
        }

        //Spawn Move Pointer
        public void OnMouseDown()
        {
            if (PlayerCanMove())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var distance = Vector2.Distance(transform.position, Camera.main.transform.position);
                BattleManager.taskManager.CallTask(0.4f, () =>
                {
                    BattleManager.HitBoxControl(false);
                    positionArrow = Instantiate(BattleManager.battleDetailsManager.movementArrowObject, ray.GetPoint(distance), Quaternion.identity);
                    var movementArrowManager = positionArrow.GetComponent<MovementArrowManager>();
                    movementArrowManager.distance = distance;
                    movementArrowManager.occupier = baseManager;
                    //BattleManager.SetFadeOnAllPanels(0.5f, 0.5f);
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
            if (positionArrow && PlayerCanMove())
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
                else
                {
                    BattleManager.battleDetailsManager.BattleWarning("Panel is too far away", 3f);
                }
                Destroy(positionArrow);
                BattleManager.HitBoxControl(true);
            }
        }
    }
}