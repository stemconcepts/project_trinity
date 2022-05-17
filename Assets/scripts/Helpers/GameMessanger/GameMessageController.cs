using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;
using System;

namespace AssemblyCSharp
{
    public class GameMessageController : MonoBehaviour
    {
        public TextMeshProUGUI headerText;
        public TextMeshProUGUI bodyText;
        public CanvasGroup canvasGroup;
        public bool pauseGame;
        public Action closeAction;

        public void WriteMessage(string message)
        {
            if (headerText.gameObject.activeSelf)
            {
                headerText.gameObject.SetActive(false);
            }
            bodyText.text = message;
        }

        public void WriteMessage(string message, string header)
        {
            if (!headerText.gameObject.activeSelf)
            {
                headerText.gameObject.SetActive(true);
            }
            bodyText.text = message;
            headerText.text = header;
        }

        public void CloseMessage()
        {
            if (pauseGame)
            {
                Time.timeScale = 1;
            }
            closeAction?.Invoke();
            Destroy(this.gameObject.transform.parent.gameObject);
        }

        // Use this for initialization
        void OnEnable()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}