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
        public GameObject GameMessagePanel;
        public GameObject GameBattleMessagePanel;

        GameObject GenerateDisplayPrefab(Transform parentObject)
        {
            return Instantiate(GameMessagePanel, parentObject);
        }

        GameObject GenerateBattleDisplayPrefab(Transform parentObject)
        {
            return Instantiate(GameBattleMessagePanel, parentObject);
        }

        /// <summary>
        /// Show game message for item use with character option to use it on
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parentObject"></param>
        /// <param name="waitTime"></param>
        /// <param name="headerText"></param>
        /// <param name="pauseGame"></param>
        /// <param name="item"></param>
        /// <param name="useItemAction"></param>
        /// <param name="cancelAction"></param>
        public void DisplayChoiceMessageForItem(string message, string okText, string cancelText, OnUseItemAction useItemAction, ItemBase item, Transform parentObject = null, float waitTime = 0, 
            string headerText = null, bool pauseGame = false, Action cancelAction = null, bool showOptions = false)
        {
            parentObject ??= MainGameManager.instance.GlobalCanvas.transform;
            MainGameManager.instance.taskManager.CallTask(waitTime, action: () =>
            {
                var displayBox = GenerateDisplayPrefab(parentObject);
                var gameMessageController = displayBox.GetComponentInChildren<GameMessageController>();
                gameMessageController.pauseGame = pauseGame;
                gameMessageController.useItemAction = useItemAction;
                gameMessageController.closeAction = cancelAction;
                var button = gameMessageController.okText.transform.parent.GetComponent<Button>();
                button.onClick.AddListener(() => gameMessageController.UseItemThenClose(item));
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
            MainGameManager.instance.taskManager.CallTask(2f, action: () =>
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