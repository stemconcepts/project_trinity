using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class EnemyMovementManager : BaseMovementManager
    {
        // Use this for initialization
        void Start()
        {
            baseManager = this.gameObject.GetComponent<EnemyCharacterManagerGroup>();
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
            const int frontPanelNum = 2;
            const int backPanelNum = 0;

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
    }
}