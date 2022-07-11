using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;
using System.Linq;

namespace AssemblyCSharp
{
    public class GameMessanger : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

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
                } else
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