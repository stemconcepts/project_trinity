using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using static UnityEditor.Progress;

namespace AssemblyCSharp
{
    public class RoomObject : DungeonObjectBase
    {
        public GameObject itemsHolder;
        public GameObject itemTemplate;
        public List<GenericItem> items;

        /*public void GenerateItems(int amount)
        {
            amount = amount > 3 ? 3 : amount;
            for (int i = 1; i <= amount; i++)
            {
                if (MainGameManager.instance.GetChanceByPercentage(1f))
                {
                    Instantiate(itemTemplate, itemsHolder.transform.GetChild(MainGameManager.instance.ReturnRandom(itemsHolder.transform.childCount)).transform);
                }
            }
        }*/

        void OnMouseUp()
        {
            MainGameManager.instance.DisableEnableLiveBoxColliders(false);
            var message = "";
            if (items.Count > 0)
            {
                items.ForEach(item =>
                {
                    message += $"{item.itemName}" + System.Environment.NewLine;
                    MainGameManager.instance.exploreManager.AddToObtainedItems(item);
                });
                MainGameManager.instance.gameMessanger.DisplayMessage($"{message}", MainGameManager.instance.GlobalCanvas.transform, 0, "You found something", closeAction: () =>
                {
                    MainGameManager.instance.DisableEnableLiveBoxColliders(true);
                    Destroy(this.gameObject);
                });
            } else
            {
                MainGameManager.instance.gameMessanger.DisplayMessage($"Nothing but a weird smell", MainGameManager.instance.GlobalCanvas.transform, 0, "You failed to find anything useful", closeAction : () =>
                {
                    MainGameManager.instance.DisableEnableLiveBoxColliders(true);
                    Destroy(this.gameObject);
                });
            }
        }
    }
}
