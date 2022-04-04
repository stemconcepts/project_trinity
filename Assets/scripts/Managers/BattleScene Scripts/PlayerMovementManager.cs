using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace AssemblyCSharp
{
    public class PlayerMovementManager : BaseMovementManager
    {
        // Use this for initialization
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

        bool PlayerCanMove()
        {
            return BattleManager.turn == BattleManager.TurnEnum.PlayerTurn &&
                baseManager.characterManager.characterModel.characterType == CharacterModel.CharacterTypeEnum.Player &&
                baseManager.characterManager.characterModel.Haste > ((PlayerSkillManager)baseManager.skillManager).turnsTaken;
        }

        //Spawn Move Pointer
        public void OnMouseDown()
        {
            if (!baseManager.statusManager.DoesStatusExist("stun") && !baseManager.autoAttackManager.isAttacking && !baseManager.skillManager.isSkillactive)
            {
                BattleManager.HitBoxControl(false);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var distance = Vector2.Distance(transform.position, Camera.main.transform.position);
                /*if (Input.GetMouseButtonDown(0))
                {
                    if (baseManager.characterManager.characterModel.characterType == CharacterModel.CharacterTypeEnum.Player && baseManager.characterManager.characterModel.Haste > ((PlayerSkillManager)baseManager.skillManager).turnsTaken)
                    {
                        positionArrow = (GameObject)Instantiate(BattleManager.battleDetailsManager.movementArrowObject, ray.GetPoint(distance), Quaternion.identity);
                        var movementArrowManager = positionArrow.GetComponent<MovementArrowManager>();
                        movementArrowManager.originalPanel = baseManager.movementManager.currentPanel.GetComponent<PanelsManager>();
                        movementArrowManager.distance = distance;
                        movementArrowManager.occupier = baseManager;
                    }
                }*/
                BattleManager.taskManager.CallTask(0.4f, () =>
                {
                    if (PlayerCanMove())
                    {
                        positionArrow = (GameObject)Instantiate(BattleManager.battleDetailsManager.movementArrowObject, ray.GetPoint(distance), Quaternion.identity);
                        var movementArrowManager = positionArrow.GetComponent<MovementArrowManager>();
                        movementArrowManager.originalPanel = baseManager.movementManager.currentPanel.GetComponent<PanelsManager>();
                        movementArrowManager.distance = distance;
                        movementArrowManager.occupier = baseManager;
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
            if (positionArrow && !baseManager.statusManager.DoesStatusExist("stun") && !baseManager.autoAttackManager.isAttacking && !baseManager.skillManager.isSkillactive)
            {
                var positionArrowManager = positionArrow.GetComponent<MovementArrowManager>();
                if (positionArrowManager.hoveredPanel && !positionArrowManager.hoveredPanel.currentOccupier && BattleManager.actionPoints >= 1)
                {
                    BattleManager.actionPoints -= movementCost;
                    ++((PlayerSkillManager)baseManager.skillManager).turnsTaken;
                    BattleManager.UpdateAPAmount();
                    positionArrowManager.SetPanelandDestroy();
                    MoveToPanel(positionArrowManager.hoveredPanel.gameObject);
                    positionArrowManager.occupier.animationManager.meshRenderer.sortingOrder = origSortingOrder = positionArrowManager.hoveredPanel.sortingLayerNumber;
                    positionArrowManager.occupier.characterManager.characterModel.rowNumber = positionArrowManager.hoveredPanel.sortingLayerNumber;
                }
            }
            Destroy(positionArrow);
            BattleManager.HitBoxControl(true);
        }
    }
}