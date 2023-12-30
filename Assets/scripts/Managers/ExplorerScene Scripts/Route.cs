using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Assets.scripts.Managers.ExplorerScene_Scripts;

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
        public List<AudioClip> transitionSounds;

        void Start()
        {
            ChoosePathSprite();
            toolTipController = GetComponent<ToolTipTriggerController>();
            if (curroptionAmount > 0)
            {
                AddCurroptionToolTip();
            }
            if (lockObj != null)
            {
                AddLockedToolTip();
            }
        }

        void AddCurroptionToolTip()
        {
            toolTipController.AddtoolTip("curroption", $"Curroption <b> +{curroptionAmount}</b>", $"");
            toolTipController.enabled = true;
        }

        public void AddLockedToolTip()
        {
            toolTipController.AddtoolTip("lock", $"{lockObj.lockDesc}", $"You'll need {lockObj.key.itemName} to open it");
            toolTipController.enabled = true;
        }

        void ChoosePathSprite()
        {
            if(allowedPathSprites.Count > 0)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = allowedPathSprites[MainGameManager.instance.ReturnRandom(allowedPathSprites.Count)];
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
            var fieldItemsController = MainGameManager.instance.exploreManager.inventoryHolder.GetComponent<fieldInventoryController>();
            return lockObj != null && lockObj.locked && fieldItemsController.fieldItems.Any(o => o.GetComponent<ExplorerItemsController>().itemBase.name == lockObj.key.name);
                /*MainGameManager.instance.exploreManager.obtainedItems.Any(o => o.name == lockObj.key.name)*/;
        }

        void ToggleHighlighTargetIcon(bool highlight)
        {
            var relevantIcon = MainGameManager.instance.exploreManager.iconControllers
                .Where(icon => icon.label == location)
                .FirstOrDefault();

            //Depending on direction of entry change the highlighted icon to the new location
            var cRoom = MainGameManager.instance.exploreManager.GetCurrentRoom();
            if (MainGameManager.instance.exploreManager.previousRooms.Count > 1 && MainGameManager.instance.exploreManager.previousRooms[MainGameManager.instance.exploreManager.previousRooms.Count - 2].name == location)
            {
                string otherRouteLocation = cRoom.routeLocations.Where(o => o != location).Where(a => !cRoom.routes.Any(r => r.location == a)).FirstOrDefault();
                relevantIcon = MainGameManager.instance.exploreManager.iconControllers
                    .Where(icon => icon.label == otherRouteLocation)
                    .FirstOrDefault();
            }

            relevantIcon.SetHighlighted(highlight);
        }

        void OnMouseEnter()
        {
            ToggleHighlighTargetIcon(true);
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
            ToggleHighlighTargetIcon(false);
            this.gameObject.GetComponent<SpriteRenderer>().color = origColor;
        }

        void OnMouseUp()
        {
            if (lockObj == null || !lockObj.locked)
            {
                MainGameManager.instance.soundManager.playSoundsInOrder(transitionSounds, true, 0.3f);
                MainGameManager.instance.gameEffectManager.TransitionToScene(MainGameManager.instance.exploreManager.GetRoomTransition(), 1f, () =>
                {
                    MainGameManager.instance.exploreManager.AddPreviousRoom(MainGameManager.instance.exploreManager.allRooms.Where(o => o.isActiveAndEnabled).FirstOrDefault());
                    MainGameManager.instance.exploreManager.ToggleRooms(true);
                    var cRoom = MainGameManager.instance.exploreManager.GetCurrentRoom();
                    GameObject targetRoom = GameObject.Find(location);
                    if (MainGameManager.instance.exploreManager.previousRooms.Count > 1 && MainGameManager.instance.exploreManager.previousRooms[MainGameManager.instance.exploreManager.previousRooms.Count - 2].name == location)
                    {
                        string otherRouteLocation = cRoom.routeLocations.Where(o => o != location).Where(a => !cRoom.routes.Any(r => r.location == a)).FirstOrDefault();
                        targetRoom = GameObject.Find(otherRouteLocation);
                    }
                    MainGameManager.instance.exploreManager.ToggleRooms(false);
                    targetRoom.SetActive(true);
                    MainGameManager.instance.exploreManager.SetCurrentRoom(targetRoom.name);
                    DungeonRoom dr = targetRoom.GetComponent<DungeonRoom>();
                    dr.SetVisited();
                    dr.CheckEncounterAndStart();
                    if (!curroptionAdded)
                    {
                        MainGameManager.instance.exploreManager.AddStep(1, curroptionAmount);
                        curroptionAdded = true;
                    }
                    else
                    {
                        MainGameManager.instance.exploreManager.AddStep(1, 0);
                    }
                });

            }
            else if (CanUnlock())
            {
                lockObj.locked = false;
                MainGameManager.instance.exploreManager.RemoveObtainedItem(lockObj.key);
            }
            toolTipController.DestroyToolTipDisplay("lock");
            toolTipController.DestroyToolTipDisplay("curroption");
            toolTipController.enabled = false;
        }
    }
}