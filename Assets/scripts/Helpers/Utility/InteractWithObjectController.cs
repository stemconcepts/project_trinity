using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts.Helpers.Utility
{
    public class InteractWithObjectController : MonoBehaviour
    {
        public delegate void OnMouseUpAction();
        public OnMouseUpAction mouseUpAction;
        public delegate void OnUseItemAction(List<RoleEnum> roles);
        public OnUseItemAction useItemAction;
        bool showOptions;
        string headerText;
        string descriptionText;
        string okayText;
        string cancelText;

        private void OnMouseUp()
        {
            ShowConfirmation();
        }

        /// <summary>
        /// Set text for okay, cancel and description text
        /// </summary>
        public void SetMessageText(string descriptionText, string okayText, string cancelText, string headerText = null, bool showOptions = false)
        {
            this.descriptionText = descriptionText;
            this.okayText = okayText;
            this.cancelText = cancelText;
            this.showOptions = showOptions;
            if (headerText != null)
            {
                this.headerText = headerText;
            }
        }

        /// <summary>
        /// Show confirmation display to confirm use of item
        /// </summary>
        private void ShowConfirmation()
        {
            MainGameManager.instance.DisableEnableLiveBoxColliders(false);
            MainGameManager.instance.gameMessanger.DisplayChoiceMessage(descriptionText, okayText, cancelText, headerText: headerText,
                //waitTime: 0.5f, pauseGame: false, okAction: () => mouseUpAction.Invoke(), showOptions: showOptions);
                waitTime: 0.5f, pauseGame: false, useItemAction: useItemAction, showOptions: showOptions);
        }
    }
}
