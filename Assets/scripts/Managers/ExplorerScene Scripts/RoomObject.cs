using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class RoomObject : DungeonObjectBase
    {
        public GameObject itemsHolder;
        public GameObject itemTemplate;

        public void GenerateItems(int amount)
        {
            amount = amount > 3 ? 3 : amount;
            for (int i = 1; i <= amount; i++)
            {
                if (Explore_Manager.GetChance(1))
                {
                    GameObject itemT = Instantiate(itemTemplate, itemsHolder.transform.GetChild(Explore_Manager.gameManager.ReturnRandom(itemsHolder.transform.childCount)).transform);
                }
            }
        }
    }
}
