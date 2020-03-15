using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using System.Linq;

namespace AssemblyCSharp
{
    public class enemySkill : GenericSkillModel
    {
    	[Header("Default Skill Variables:")]

    	public bool skillConfirm;
    	public int skillCost;
        [Header("Requirements:")]
        public PreRequisiteModel preRequisite;
        //public preRequisiteTypeEnum preRequisiteType;
        [Header("Summon Creature:")]
    	public List<GameObject> summonedObjects = new List<GameObject>();
        public voidZoneType voidZoneTypes;
        public enum voidZoneType
        {
            All,
            Vline,
            Hline,
            Random
        }
        public bool monsterPanel = false;
        public bool hasVoidzone;
        public float eventDuration;
        
        /*public enum preRequisiteTypeEnum
        {
            none,
            summon
        }*/
        GameObject GetRandomPanelFromPanels(GameObject[] panels = null)
        {
            if (panels == null)
            {
                panels = !monsterPanel ? GameObject.FindGameObjectsWithTag("movementPanels") : GameObject.FindGameObjectsWithTag("enemyMovementPanels");
            }
            var chosenPanels = new List<GameObject>();
            foreach (var panel in panels)
            {
                if (!panel.GetComponent<Panels_Manager>().currentOccupier)
                {
                    chosenPanels.Add(panel);
                }
            }
            var randomPanelNumber = Random.Range(0, chosenPanels.Count);
            return chosenPanels[randomPanelNumber];
        }

        public void ShowVoidPanel(voidZoneType voidZoneEnumVar, bool monsterPanels = false)
        {
            var randomRowNumber = Random.Range(0, 3);
            switch (voidZoneEnumVar)
            {
                case voidZoneType.All:
                    var allPanels = !monsterPanels ? GameObject.FindGameObjectsWithTag("movementPanels") : GameObject.FindGameObjectsWithTag("enemyMovementPanels");
                    for (int i = 0; allPanels.Length > i; i++)
                    {
                        var panelScript = allPanels[i].GetComponent<Panels_Manager>();
                        panelScript.VoidZoneMark();
                    }
                    GetSafePanel(allPanels).GetComponent<Panels_Manager>().SafePanel();
                    break;
                case voidZoneType.Hline:
                    var HPanel = !monsterPanels ? GameObject.Find("FriendlyMovementPanel") : GameObject.Find("EnemyMovementPanel");
                    var HRow = HPanel.transform.GetChild(randomRowNumber);
                    foreach (Transform panel in HRow.transform)
                    {
                        var panelScript = panel.GetComponent<Panels_Manager>();
                        panelScript.VoidZoneMark();
                    }
                    break;
                case voidZoneType.Vline:

                    break;
                case voidZoneType.Random:
                    allPanels = !monsterPanels ? GameObject.FindGameObjectsWithTag("movementPanels") : GameObject.FindGameObjectsWithTag("enemyMovementPanels");
                    GetRandomPanelFromPanels(allPanels).GetComponent<Panels_Manager>().VoidZoneMark();
                    break;
            }
        }

        public void ClearVoidPanel()
        {
            var allPanels = GameObject.FindGameObjectsWithTag("movementPanels");
            for (var i = 0; allPanels.Length > i; i++)
            {
                var panelScript = allPanels[i].GetComponent<Panels_Manager>();
                panelScript.ClearVoidZone();
            }
        }

        GameObject GetSafePanel(GameObject[] panels)
        {
            //var allPanels = GameObject.FindGameObjectsWithTag("movementPanels");
            var randomPanelNumber = Random.Range(0, panels.Length);
            var safePanelScript = panels[randomPanelNumber].GetComponent<Panels_Manager>();
            if (safePanelScript.currentOccupier == null)
            {
                return panels[randomPanelNumber];
            }
            else
            {
                return GetSafePanel(panels);
                //return null;
            }
        }

        //summon object
        public void SummonCreatures()
        {
            for (int i = 0; i < summonedObjects.Count; i++)
            {
                var enemyIndex = Battle_Manager.GetCharacterManagers(false).Count - 1;
                var singleMinionDataItem = Battle_Manager.assetFinder.GetGameObjectFromPath("Assets/prefabs/combatInfo/character_info/singleMinionData.prefab");
                var creatureData = Instantiate(singleMinionDataItem, GameObject.Find("Panel MinionData").transform);
                creatureData.name = "minion_" + enemyIndex + "_data";
                var panel = GetRandomPanelFromPanels();
                var panelManager = panel.GetComponent<Panels_Manager>();
                if (panel)
                {
                    //creatureData.transform.GetChild(0).GetChild(0).GetComponentInChildren<UI_Display_Text>().On = true;
                    var newCreature = Instantiate(summonedObjects[i], GameObject.Find("enemyHolder").transform);
                    newCreature.name = "minion_" + enemyIndex;
                    var minionBaseManager = newCreature.GetComponent<Base_Character_Manager>();
                    panelManager.currentOccupier = newCreature;
                    minionBaseManager.characterManager.healthBar = creatureData.transform.Find("Panel Minion HP").Find("Slider_enemy").gameObject;
                    minionBaseManager.statusManager.statusHolderObject = creatureData.transform.Find("Panel Minion Status").Find("minionstatus").gameObject;
                    panelManager.SetStartingPanel(newCreature, true);
                    Battle_Manager.characterSelectManager.UpdateCharacters();
                }
                else
                {
                    Debug.Log("No Panel");
                }
            }
        }
    }
}