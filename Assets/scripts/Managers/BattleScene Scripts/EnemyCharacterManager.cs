using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI.Table;
using System;

namespace AssemblyCSharp
{
    public class EnemyCharacterManager : BaseCharacterManager
    {
        public GameObject enemyCharacterSelector;
        public Sprite enemySprite;
        public List<CompatibleRow> SpawnableRows = new List<CompatibleRow>();

        PanelsManager GetValidStartingPanel()
        {
            PanelsManager validPanel = null;
            var availableRows = BattleManager.GetAllFreePanels(false);
            while (validPanel == null)
            {

                if (SpawnableRows.Count == 0 || SpawnableRows.Any(row => CompatibleRow.All == row))
                {
                    var randomPanelNumber = UnityEngine.Random.Range(0, availableRows.Count);
                    validPanel = availableRows[randomPanelNumber];
                    continue;
                }

                foreach (var row in SpawnableRows)
                {
                    var randomPanel = availableRows[
                            UnityEngine.Random.Range(0, availableRows.Count)
                        ];

                    switch (randomPanel.panelNumber)
                    {
                        case 0:
                            if (row.Equals(CompatibleRow.Back)) {
                                validPanel = randomPanel;
                            }
                            break;
                        case 1:
                            if (row.Equals(CompatibleRow.Middle))
                            {
                                validPanel = randomPanel;
                            }
                            break;
                        case 2:
                            if (row.Equals(CompatibleRow.Front))
                            {
                                validPanel = randomPanel;
                            }
                            break;
                        default:
                            validPanel = randomPanel;
                            break;
                    }

                    availableRows.Remove(randomPanel);
                }
            }

            return validPanel;
        }

        void SetStartingPanel()
        {
            if (baseManager.movementManager.currentPanel == null)
            {
                var panel = GetValidStartingPanel();

                if (!panel)
                {
                    throw new Exception($"Starting panel not found for {gameObject.name}");
                }

                panel.currentOccupier = gameObject;
                baseManager.movementManager.currentPanel = panel.gameObject;
            }
        }

        void Update()
        {
            if (initialSetupDone)
            {
                UpdateBarSize(characterModel);
                ResetAbsorbPoints(characterModel);
                MaintainHealthValue(characterModel);
                characterModel.currentRotation = this.transform.rotation;
            }
        }

        void Awake()
        {
            characterModel = this.gameObject.GetComponent<EnemyCharacterModel>();
            baseManager = this.gameObject.GetComponent<EnemyCharacterManagerGroup>();
            if (this.characterModel.role == RoleEnum.boss && healthBar == null)
            {
                healthBar = GameObject.Find("Slider_enemy");
            }
            if (template != null)
            {
                resistances = useResistance && template != null ? template.resistances : resistances;
            }
        }

        // Use this for initialization
        void Start()
        {
            if (healthBar == null)
            {
                BattleManager.GenerateEnemySelector(this.gameObject);
                BattleManager.GenerateLifeBars(this.gameObject);
            }
            if (this.characterModel != null)
            {
                (this.characterModel as EnemyCharacterModel).SetUp();
                this.characterModel.healthBarText = healthBar.gameObject.transform.Find("healthdata").GetComponent<Text>();
                this.characterModel.sliderScript = healthBar.GetComponent<Slider>();
                UpdateBarSize(this.characterModel);
                ((EnemyCharacterManagerGroup)baseManager).phaseManager.ChangeBossPhase();
                initialSetupDone = true;
            }
            SetStartingPanel();
        }
    }
}