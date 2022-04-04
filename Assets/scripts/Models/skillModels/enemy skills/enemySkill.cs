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
        [Header("Requirements:")]
        //public PreRequisiteModel preRequisite;
        public List<PreRequisiteModel> preRequisites;

        //public preRequisiteTypeEnum preRequisiteType;
        /*[Header("Summon Creature:")]
    	public List<GameObject> summonedObjects = new List<GameObject>();*/
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
                if (!panel.GetComponent<PanelsManager>().currentOccupier)
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
            var randomVertical = PanelsManager.voidZoneType.VerticalA;

            switch (randomRowNumber)
            {
                case 0:
                    randomVertical = PanelsManager.voidZoneType.VerticalA;
                    break;
                case 1:
                    randomVertical = PanelsManager.voidZoneType.VerticalB;
                    break;
                case 2:
                    randomVertical = PanelsManager.voidZoneType.VerticalC;
                    break;
                default:
                    randomVertical = PanelsManager.voidZoneType.VerticalA;
                    break;
            }
            switch (voidZoneEnumVar)
            {
                case voidZoneType.All:
                    var allPanels = !monsterPanels ? GameObject.FindGameObjectsWithTag("movementPanels") : GameObject.FindGameObjectsWithTag("enemyMovementPanels");
                    for (int i = 0; allPanels.Length > i; i++)
                    {
                        var panelScript = allPanels[i].GetComponent<PanelsManager>();
                        panelScript.VoidZoneMark();
                    }
                    GetSafePanel(allPanels).GetComponent<PanelsManager>().SafePanel();
                    break;
                case voidZoneType.Hline:
                    var HPanel = !monsterPanels ? GameObject.Find("FriendlyMovementPanel") : GameObject.Find("EnemyMovementPanel");
                    var HRow = HPanel.transform.GetChild(randomRowNumber);
                    foreach (Transform panel in HRow.transform)
                    {
                        var panelScript = panel.GetComponent<PanelsManager>();
                        panelScript.VoidZoneMark();
                    }
                    break;
                case voidZoneType.Vline:
                    var VPanel = !monsterPanels ? GameObject.Find("FriendlyMovementPanel") : GameObject.Find("EnemyMovementPanel");
                    var VRows = VPanel.transform.GetComponentsInChildren<PanelsManager>();
                    foreach (PanelsManager panelManager in VRows)
                    {
                        if (panelManager.voidZonesTypes == randomVertical)
                        {
                            panelManager.VoidZoneMark();
                        }
                    }

                    break;
                case voidZoneType.Random:
                    allPanels = !monsterPanels ? GameObject.FindGameObjectsWithTag("movementPanels") : GameObject.FindGameObjectsWithTag("enemyMovementPanels");
                    GetRandomPanelFromPanels(allPanels).GetComponent<PanelsManager>().VoidZoneMark();
                    break;
            }
        }

        public void ClearVoidPanel()
        {
            var allPanels = GameObject.FindGameObjectsWithTag("movementPanels");
            for (var i = 0; allPanels.Length > i; i++)
            {
                var panelScript = allPanels[i].GetComponent<PanelsManager>();
                panelScript.ClearVoidZone();
            }
        }

        GameObject GetSafePanel(GameObject[] panels)
        {
            //var allPanels = GameObject.FindGameObjectsWithTag("movementPanels");
            var randomPanelNumber = Random.Range(0, panels.Length);
            var safePanelScript = panels[randomPanelNumber].GetComponent<PanelsManager>();
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
        public void SummonCreatures(List<GameObject> summonedObjects)
        {
            for (int i = 0; i < summonedObjects.Count; i++)
            {
                    var enemyIndex = BattleManager.characterSelectManager.enemyCharacters.Count() + i;
                    var singleMinionDataItem = BattleManager.assetFinder.GetGameObjectFromPath("Assets/prefabs/combatInfo/character_info/singleMinionData.prefab");
                    var creatureData = Instantiate(singleMinionDataItem, GameObject.Find("Panel MinionData").transform);
                    creatureData.name = "minion_" + enemyIndex + "_data";
                    var panel = GetRandomPanelFromPanels();
                    var panelManager = panel.GetComponent<PanelsManager>();
                    if (panel)
                    {
                        //creatureData.transform.GetChild(0).GetChild(0).GetComponentInChildren<UI_Display_Text>().On = true;
                            var newCreature = Instantiate(summonedObjects[i], GameObject.Find("enemyHolder").transform);
                            newCreature.name = "minion_" + enemyIndex;
                            var minionBaseManager = newCreature.GetComponent<BaseCharacterManagerGroup>();
                            minionBaseManager.autoAttackManager.hasAttacked = true;
                            panelManager.currentOccupier = newCreature;
                            minionBaseManager.characterManager.healthBar = creatureData.transform.Find("Panel Minion HP").Find("Slider_enemy").gameObject;
                            minionBaseManager.statusManager.statusHolderObject = creatureData.transform.Find("Panel Minion Status").Find("minionstatus").gameObject;
                            panelManager.SetStartingPanel(newCreature, true);
                        panelManager.SaveCharacterPositionFromPanel();
                    }
                    else
                    {
                        Debug.Log("No Panel");
                    }
            }
        }
    }
}