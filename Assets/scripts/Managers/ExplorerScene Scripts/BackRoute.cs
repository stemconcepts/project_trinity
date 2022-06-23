using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class BackRoute : Route
    {
        void OnMouseEnter()
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = hoverColor;
        }

        void OnMouseExit()
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = origColor;
        }

        void OnMouseUp()
        {
            ExploreManager.ToggleRooms(false);
            ExploreManager.previousRooms[ExploreManager.previousRooms.Count - 1].gameObject.SetActive(true);
            ExploreManager.SetCurrentRoom(ExploreManager.previousRooms[ExploreManager.previousRooms.Count - 1].gameObject.name);
            SavedDataManager.SavedDataManagerInstance.EditPreviousRoom(ExploreManager.previousRooms[ExploreManager.previousRooms.Count - 1].gameObject.name, false);
            ExploreManager.previousRooms.RemoveAt(ExploreManager.previousRooms.Count - 1);
            if (ExploreManager.previousRooms.Count == 0)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
