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
        public delegate void OnUseItemAction(List<RoleEnum> roles, ItemBase item);
        public OnUseItemAction useItemAction;
        public ItemBase item;
        bool showOptions;
        string headerText;
        string descriptionText;
        string okayText;
        string cancelText;

        private void OnMouseUp()
        {
            ShowConfirmation(item);
        }

        /// <summary>
        /// Set text for okay, cancel and description text
        /// </summary>
        public void SetMessageText(string okayText, string cancelText, Consumable item)
        {
            this.descriptionText = item.itemDesc;
            this.okayText = okayText;
            this.cancelText = cancelText;
            this.showOptions = !item.affectAll;
            this.item = item;
            this.headerText = item.itemName;
        }

        /// <summary>
        /// Show confirmation display to confirm use of item
        /// </summary>
        private void ShowConfirmation(ItemBase itemToUse)
        {
            MainGameManager.instance.DisableEnableLiveBoxColliders(false);
            MainGameManager.instance.gameMessanger.DisplayChoiceMessageForItem(descriptionText, okayText, cancelText, item: itemToUse, headerText: headerText,
                waitTime: 0.5f, pauseGame: false, useItemAction: useItemAction, showOptions: showOptions);
        }
    }
}
