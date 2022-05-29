using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Route : MonoBehaviour
    {
        public string routeTag;
        public string location;
        public int position;
        public LockObject lockObj;
        public SpriteRenderer spriteRenderer;
        public List<Sprite> allowedPathSprites;
        public Color origColor;
        public Color hoverColor;
        ToolTipTriggerController toolTipController;

        void Start()
        {
            ChoosePathSprite();
            toolTipController = GetComponent<ToolTipTriggerController>();
        }

        void ChoosePathSprite()
        {
            if(allowedPathSprites.Count > 0)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = allowedPathSprites[ExploreManager.gameManager.ReturnRandom(allowedPathSprites.Count)];
            }
        }

        public void UpdateRouteDetails(string tag, string location, LockObject lockObj)
        {
            routeTag = tag;
            this.gameObject.name = tag;
            this.location = location;
            this.lockObj = lockObj;
        }

        bool CanUnlock()
        {
            return lockObj != null && lockObj.locked && ExploreManager.obtainedItems.Any(o => o.name == lockObj.key.name);
        }

        void OnMouseEnter()
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = hoverColor;
            if (CanUnlock())
            {
                toolTipController.toolTipDesc = $"Open lock with <b>{lockObj.key.name}</b>. <i>Click to unlock</i>";
            }
        }

        void OnMouseExit()
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = origColor;
        }

        void OnMouseUp()
        {
            if (lockObj == null || !lockObj.locked)
            {
                ExploreManager.AddPreviousRoom(ExploreManager.allRooms.Where(o => o.isActiveAndEnabled).FirstOrDefault());
                //Explore_Manager.ChangeRouteInBackButton(Explore_Manager.allRooms.Where(o => o.isActiveAndEnabled).FirstOrDefault());
                ExploreManager.ToggleRooms(true);
                var cRoom = ExploreManager.GetCurrentRoom();
                GameObject targetRoom = GameObject.Find(location);
                if (ExploreManager.previousRooms.Count > 1 && ExploreManager.previousRooms[ExploreManager.previousRooms.Count - 2].name == location)
                {
                    string otherRouteLocation = cRoom.routeLocations.Where(o => o != location).Where(a => !cRoom.routes.Any(r => r.location == a)).FirstOrDefault();
                    targetRoom = GameObject.Find(otherRouteLocation);
                }
                ExploreManager.ToggleRooms(false);
                targetRoom.SetActive(true);
                ExploreManager.SetCurrentRoom(targetRoom.name);
                DungeonRoom dr = targetRoom.GetComponent<DungeonRoom>();
                dr.SetVisited();
                dr.CheckEncounterAndStart();
            } else if (CanUnlock())
            {
                lockObj.locked = false;
                toolTipController.DestroyToolTipDisplay();
                toolTipController.enabled = false;
                ExploreManager.RemoveObtainedItem(lockObj.key);
            }
        }
    }
}