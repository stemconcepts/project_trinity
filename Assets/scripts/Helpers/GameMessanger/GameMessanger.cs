using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;
using static Assets.scripts.Helpers.Utility.InteractWithObjectController;

namespace AssemblyCSharp
{
    public class GameMessanger : MonoBehaviour
    {
        GameObject GenerateDisplayPrefab(Transform parentObject)
        {
            var displayBoxPrefab = MainGameManager.instance.assetFinder.GetGameObjectFromPath("Assets/prefabs/helpers/GameMessagePanel.prefab");
            return Instantiate(displayBoxPrefab, parentObject);
        }

        GameObject GenerateBattleDisplayPrefab(Transform parentObject)
        {
            var displayBoxPrefab = MainGameManager.instance.assetFinder.GetGameObjectFromPath("Assets/prefabs/helpers/GameBattleMessagePanel.prefab");
            return Instantiate(displayBoxPrefab, parentObject);
        }

        /// <summary>
        /// Show game message with okay and cancel button
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parentObject"></param>
        /// <param name="waitTime"></param>
        /// <param name="headerText"></param>
        /// <param name="pauseGame"></param>
        /// <param name="okAction"></param>
        /// <param name="cancelAction"></param>
        public void DisplayChoiceMessage(string message, string okText, string cancelText, OnUseItemAction useItemAction, Transform parentObject = null, float waitTime = 0, string headerText = null, 
            bool pauseGame = false, Action cancelAction = null, bool showOptions = false)
        {
            parentObject ??= MainGameManager.instance.GlobalCanvas.transform;
            MainGameManager.instance.taskManager.CallTask(waitTime, action: () =>
            {
                var displayBox = GenerateDisplayPrefab(parentObject);
                var gameMessageController = displayBox.GetComponentInChildren<GameMessageController>();
                gameMessageController.pauseGame = pauseGame;
                gameMessageController.useItemAction = useItemAction;
                gameMessageController.closeAction = cancelAction;
                var b = gameMessageController.okText.transform.parent.GetComponent<Button>();
                b.onClick.AddListener(() => gameMessageController.PerformActionThenClose());
                gameMessageController.WriteMessage(message, headerText, okText, cancelText, showOptions);
                if (pauseGame)
                {
                    Time.timeScale = 0;
                }
            });
        }

        /// <summary>
        /// Show game message with a close button
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parentObject"></param>
        /// <param name="waitTime"></param>
        /// <param name="headerText"></param>
        /// <param name="pauseGame"></param>
        /// <param name="closeAction"></param>
        public void DisplayMessage(string message, Transform parentObject = null, float waitTime = 0, string headerText = null, bool pauseGame = false, Action closeAction = null)
        {
            parentObject ??= MainGameManager.instance.GlobalCanvas.transform;
            MainGameManager.instance.taskManager.CallTask(waitTime, action :() =>
            {
                var displayBox = GenerateDisplayPrefab(parentObject);
                var gameMessageController = displayBox.GetComponentInChildren<GameMessageController>();
                gameMessageController.pauseGame = pauseGame;
                gameMessageController.closeAction = closeAction;
                if (headerText != null)
                {
                    gameMessageController.WriteMessage(message, headerText);
                }
                else
                {
                    gameMessageController.WriteMessage(message);
                }
                if (pauseGame)
                {
                    Time.timeScale = 0;
                }
            });
        }

        public void DisplayBattleResults(Action closeAction = null)
        {
            MainGameManager.instance.taskManager.CallTask(3f, action: () =>
            {
                var displayBox = GenerateBattleDisplayPrefab(MainGameManager.instance.GlobalCanvas.transform);
                displayBox.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);

                /*var views = displayBox.GetComponentsInChildren<Transform>();
                views.ToList().ForEach(o =>
                {
                    o.DOLocalMoveX(-20f, 1f).SetEase(Ease.OutSine);
                });*/

                var gameMessageController = displayBox.GetComponentInChildren<GameBattleMessageController>();
                gameMessageController.closeAction = closeAction;
            });
        }
    }
}