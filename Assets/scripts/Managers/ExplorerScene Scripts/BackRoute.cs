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
            MainGameManager.instance.soundManager.playSoundsInOrder(transitionSounds, true, 0.5f);
            if (MainGameManager.instance.exploreManager.previousRooms.Count > 0)
            {
                MainGameManager.instance.gameEffectManager.TransitionToScene(MainGameManager.instance.exploreManager.GetRoomTransition(), 1f, () =>
                {
                    MainGameManager.instance.exploreManager.ToggleRooms(false);
                    MainGameManager.instance.exploreManager.previousRooms[MainGameManager.instance.exploreManager.previousRooms.Count - 1].gameObject.SetActive(true);
                    MainGameManager.instance.exploreManager.SetCurrentRoom(MainGameManager.instance.exploreManager.previousRooms[MainGameManager.instance.exploreManager.previousRooms.Count - 1].gameObject.name);
                    SavedDataManager.SavedDataManagerInstance.EditPreviousRoom(MainGameManager.instance.exploreManager.previousRooms[MainGameManager.instance.exploreManager.previousRooms.Count - 1].gameObject.name, false);
                    MainGameManager.instance.exploreManager.previousRooms.RemoveAt(MainGameManager.instance.exploreManager.previousRooms.Count - 1);
                });
            }
        }
    }
}
