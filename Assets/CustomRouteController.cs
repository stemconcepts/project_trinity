using AssemblyCSharp;
using Assets.scripts.Managers.ExplorerScene_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CustomRouteController : MonoBehaviour
{
    public string routeTag;
    public string roomLocation;
    public int curroptionAmount;
    public LockObject lockObj;
    ToolTipTriggerController toolTipController;
    bool curroptionAdded = false;
    public List<AudioClip> transitionSounds;
    public Color origColor;
    public Color hoverColor;

    void Start()
    {
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

    bool CanUnlock()
    {
        var fieldItemsController = MainGameManager.instance.exploreManager.inventoryHolder.GetComponent<fieldInventoryController>();
        return lockObj != null && lockObj.locked && fieldItemsController.fieldItems.Any(o => o.GetComponent<ExplorerItemsController>().itemBase.name == lockObj.key.name);
    }

    void ToggleHighlighTargetIcon(bool highlight)
    {
        /*var relevantIcon = MainGameManager.instance.exploreManager.iconControllers
            .Where(icon => ((miniMapCustomIcon)icon).customRoom.name == roomLocation.name)
            .FirstOrDefault();
        if(relevantIcon != null) relevantIcon.SetHighlighted(highlight);*/
    }

    void OnMouseEnter()
    {
        ToggleHighlighTargetIcon(true);
        this.gameObject.GetComponent<SpriteRenderer>().color = hoverColor;
        if (CanUnlock())
        {
            if (toolTipController.toolTipList.Any(o => o.id.ToLower() == "lock"))
            {
                toolTipController.toolTipList.First(o => o.id.ToLower() == "lock").toolTipDesc = $"Open lock with <b>{lockObj.key.name}</b>. <i>Click to unlock</i>";
            }
        }
    }

    void OnMouseExit()
    {
        ToggleHighlighTargetIcon(false);
        this.gameObject.GetComponent<SpriteRenderer>().color = origColor;
    }

    void OnMouseUp()
    {
        toolTipController.DestroyAllToolTips();
        if (lockObj == null || !lockObj.locked)
        {
            MainGameManager.instance.soundManager.playSoundsInOrder(transitionSounds, true, 0.3f);
            MainGameManager.instance.gameEffectManager.TransitionToScene(MainGameManager.instance.exploreManager.GetRoomTransition(), 1f, () =>
            {
                MainGameManager.instance.exploreManager.AddPreviousRoom(MainGameManager.instance.exploreManager.allRooms.Where(o => o.isActiveAndEnabled).FirstOrDefault());
                MainGameManager.instance.exploreManager.ToggleRooms(true);
                GameObject targetRoom = GameObject.Find(roomLocation);
                MainGameManager.instance.exploreManager.ToggleRooms(false);
                targetRoom.SetActive(true);
                MainGameManager.instance.exploreManager.SetCurrentRoom(roomLocation);
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
        //toolTipController.DestroyToolTipDisplay("lock");
        //toolTipController.DestroyToolTipDisplay("curroption");
        toolTipController.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
