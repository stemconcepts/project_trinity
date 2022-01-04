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
            Explore_Manager.ToggleRooms(false);
            Explore_Manager.previousRooms[Explore_Manager.previousRooms.Count - 1].gameObject.SetActive(true);
            Explore_Manager.SetCurrentRoom(Explore_Manager.previousRooms[Explore_Manager.previousRooms.Count - 1].gameObject.name);
            Explore_Manager.previousRooms.RemoveAt(Explore_Manager.previousRooms.Count - 1);
            if (Explore_Manager.previousRooms.Count == 0)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
