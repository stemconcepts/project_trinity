using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Route : MonoBehaviour
    {
        public string routeTag;
        public string location;
        //public bool locked;
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
                spriteRenderer.sprite = allowedPathSprites[Explore_Manager.gameManager.ReturnRandom(allowedPathSprites.Count)];
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
            return lockObj != null && lockObj.locked && Explore_Manager.obtainedItems.Any(o => o.name == lockObj.key.name);
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
                Explore_Manager.AddPreviousRoom(Explore_Manager.allRooms.Where(o => o.isActiveAndEnabled).FirstOrDefault());
                //Explore_Manager.ChangeRouteInBackButton(Explore_Manager.allRooms.Where(o => o.isActiveAndEnabled).FirstOrDefault());
                Explore_Manager.ToggleRooms(true);
                GameObject targetRoom = GameObject.Find(location);
                Explore_Manager.ToggleRooms(false);
                targetRoom.SetActive(true);
                Explore_Manager.SetCurrentRoom(targetRoom.name);

                targetRoom.GetComponent<DungeonRoom>().CheckEncounterAndStart();
            } else if (CanUnlock())
            {
                lockObj.locked = false;
                toolTipController.DestroyToolTipDisplay();
                toolTipController.enabled = false;
                Explore_Manager.RemoveObtainedItem(lockObj.key);
            }
        }
    }
}