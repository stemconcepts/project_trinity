using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class RoomObject : DungeonObjectBase
    {
        public GameObject itemsHolder;
        public GameObject itemTemplate;
        public ItemBase items;

        public void GenerateItems(int amount)
        {
            amount = amount > 3 ? 3 : amount;
            for (int i = 1; i <= amount; i++)
            {
                if (/*ExploreManager.GetChance(1)*/ GameManager.GetChanceByPercentage(0.5f))
                {
                    GameObject itemT = Instantiate(itemTemplate, itemsHolder.transform.GetChild(ExploreManager.gameManager.ReturnRandom(itemsHolder.transform.childCount)).transform);
                }
            }
        }
    }
}
