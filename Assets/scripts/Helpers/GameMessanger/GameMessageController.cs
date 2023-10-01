using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using static Assets.scripts.Helpers.Utility.InteractWithObjectController;

namespace AssemblyCSharp
{
    public class GameMessageController : MonoBehaviour
    {
        public TextMeshProUGUI headerText;
        public TextMeshProUGUI bodyText;
        public TextMeshProUGUI okText;
        public TextMeshProUGUI cancelText;
        public GameObject optionsHolder;
        public CanvasGroup canvasGroup;
        public bool pauseGame;
        public Action closeAction;
        public Action okAction;
        public OnUseItemAction useItemAction;
        List<RoleEnum> rolesSelected = new List<RoleEnum>();

        /// <summary>
        /// Show/Hide Options view
        /// </summary>
        void ShowOptions(bool show)
        {
            optionsHolder.SetActive(show);
        }

        /// <summary>
        /// Show/Hide header text
        /// </summary>
        void ShowHideHeader(bool show)
        {
            headerText.gameObject.SetActive(show);
        }

        /// <summary>
        /// Show/Hide cancel button
        /// </summary>
        void ShowHideCancelButton(bool show)
        {
            cancelText.transform.parent.gameObject.SetActive(show);
        }

        public void WriteMessage(string message)
        {
            ShowHideHeader(headerText.gameObject.activeSelf);
            ShowHideCancelButton(false);
            bodyText.text = message;
        }

        public void WriteMessage(string message, string header)
        {
            ShowHideHeader(header != null);
            ShowHideCancelButton(false);
            bodyText.text = message;
            headerText.text = header;
        }

        public void WriteMessage(string message, string header, string okText, string cancelText, bool showOptions = false)
        {
            ShowHideHeader(header != null);
            ShowHideCancelButton(cancelText != null);
            ShowOptions(showOptions);
            bodyText.text = message;
            headerText.text = header;
            this.okText.text = okText;
            this.cancelText.text = cancelText;
        }

        public void UseItemThenClose(ItemBase item)
        {
            useItemAction.Invoke(rolesSelected, item);
            CloseMessage();
        }

        public void PerformActionThenClose()
        {
            okAction.Invoke();
            CloseMessage();
        }

        public void CloseMessage()
        {
            if (pauseGame)
            {
                Time.timeScale = 1;
            }
            closeAction?.Invoke();
            MainGameManager.instance.DisableEnableLiveBoxColliders(true);
            Destroy(this.gameObject.transform.parent.gameObject);
        }
        
        /// <summary>
        /// Set characters for item use
        /// </summary>
        /// <param name="character"></param>
        public void SelectCharacter(string character)
        {
            RoleEnum role = (RoleEnum)Enum.Parse(typeof(RoleEnum), character);
            if (!rolesSelected.Any(characterItem => characterItem == role))
            {
                rolesSelected.Add(role);
            } else
            {
                rolesSelected.Remove(role);
            }
        }
    }
}