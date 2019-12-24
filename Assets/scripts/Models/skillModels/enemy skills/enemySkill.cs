using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

namespace AssemblyCSharp
{
    public class enemySkill : GenericSkillModel
    {
    	[Header("Default Skill Variables:")]
    	
    	public bool skillConfirm;
    	public int skillCost;
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
        //public AssemblyCSharp.Skill_Manager.EnemyPhase bossPhase;
        public bool phaseOne;
        public bool phaseTwo;
        public bool phaseThree;
        public bool hasVoidzone;
        public float eventDuration;

        GameObject GetRandomPanel(GameObject[] panels = null)
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
                    GetRandomPanel(allPanels).GetComponent<Panels_Manager>().VoidZoneMark();
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
        public void SummonCreatures(List<GameObject> targetCreatures)
        {
            for (int i = 0; i < targetCreatures.Count; i++)
            {
                var creatureData = (GameObject)Instantiate(GameObject.Find("singleMinionData"), GameObject.Find("Panel MinionData").transform);
                var panel = GetRandomPanel();
                if (panel)
                {
                    creatureData.transform.GetChild(0).GetChild(0).GetComponentInChildren<UI_Display_Text>().On = true;
                    var newCreature = (GameObject)Instantiate(targetCreatures[i], creatureData.transform);
                    panel.GetComponent<Panels_Manager>().currentOccupier = newCreature;
                    creatureData.transform.GetChild(1).GetChild(0).gameObject.name = newCreature.GetComponent<Character_Manager>().characterModel.role.ToString() + i.ToString() + "status";
                    newCreature.gameObject.name = newCreature.GetComponent<Character_Manager>().characterModel.role.ToString() + i.ToString();
                    newCreature.GetComponent<Movement_Manager>().currentPanel = panel;
                    newCreature.GetComponent<Character_Manager>().characterModel.role = Character_Model.RoleEnum.minion;
                    creatureData.transform.GetChild(0).GetChild(0).GetComponentInChildren<UI_Display_Text>().SetDataObjects(i);
                    //panel.GetComponent<Panels_Manager>().Start();
                    newCreature.GetComponent<Animation_Manager>().skeletonAnimation.state.SetAnimation(0, "intro", false);
                    newCreature.GetComponent<Animation_Manager>().skeletonAnimation.state.AddAnimation(0, "idle", true, 0);
                    //StartCoroutine( DelayedStart( newCreature ) );
                    globalEffectsController.callEffectTarget(newCreature, fxObject, fxPos.ToString());
                }
                else
                {
                    Debug.Log("No Panel");
                }
            }
        }
    }
}