using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Route : MonoBehaviour
    {
        public string routeTag;
        public string location;
        public int position, curroptionAmount;
        public LockObject lockObj;
        public SpriteRenderer spriteRenderer;
        public List<Sprite> allowedPathSprites;
        public Color origColor;
        public Color hoverColor;
        ToolTipTriggerController toolTipController;
        bool curroptionAdded = false;

        void Start()
        {
            ChoosePathSprite();
            toolTipController = GetComponent<ToolTipTriggerController>();
            if (curroptionAmount > 0) {
                AddCurroptionToolTip();
            }
        }

        void AddCurroptionToolTip()
        {
            toolTipController.AddtoolTip("curroption", $"Curroption <b> +{curroptionAmount}</b>", $"");
            toolTipController.enabled = true;
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
                if(toolTipController.toolTipList.Any(o => o.id.ToLower() == "lock"))
                {
                    toolTipController.toolTipList.First(o => o.id.ToLower() == "lock").toolTipDesc = $"Open lock with <b>{lockObj.key.name}</b>. <i>Click to unlock</i>";
                }
            }
            /*if (!curroptionAdded)
            {
                if (toolTipController.toolTipList.Any(o => o.id.ToLower() == "curroption"))
                {
                    toolTipController.toolTipList.First(o => o.id.ToLower() == "curroption").toolTipName += $" <b color='#000'>+{curroptionAmount}</b>";
                }
            }*/
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
                if (!curroptionAdded)
                {
                    ExploreManager.AddStep(1, curroptionAmount);
                    curroptionAdded = true;
                }
                else
                {
                    ExploreManager.AddStep(1, 0);
                }
                toolTipController.DestroyToolTipDisplay("curroption");
            }
            else if (CanUnlock())
            {
                lockObj.locked = false;
                toolTipController.DestroyToolTipDisplay("lock");
                toolTipController.DestroyToolTipDisplay("curroption");
                toolTipController.enabled = false;
                ExploreManager.RemoveObtainedItem(lockObj.key);
            }
        }
    }
}