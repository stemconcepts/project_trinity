using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Analytics;
using Assets.scripts.Models.statusModels;

namespace AssemblyCSharp
{
    public class CharacterManager : BaseCharacterManager
    {
        public int RecoverTime;
        int TurnsToRecover;

        public void Ressurect()
        {
            if (!characterModel.isAlive && BattleManager.turnCount >= TurnsToRecover)
            {
                characterModel.isAlive = true;
                characterModel.Health = characterModel.maxHealth * 0.3f;
            }
        }

        void Awake()
        {
            baseManager = this.gameObject.GetComponent<CharacterManagerGroup>();
            characterModel = this.gameObject.GetComponent<CharacterModel>();
            if (template != null)
            {
                resistances = useResistance ? template.resistances : resistances;
            }
            TurnsToRecover = BattleManager.turnCount + RecoverTime;
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

        void Start()
        {
            if (this.characterModel != null)
            {
                (this.characterModel as CharacterModel).SetUp();
                this.characterModel.healthBarText = healthBar.gameObject.transform.Find("healthdata").GetComponent<Text>();
                this.characterModel.sliderScript = healthBar.GetComponent<Slider>();
                UpdateBarSize(characterModel);
                initialSetupDone = true;
            }
            if (baseManager.movementManager.currentPanel == null)
            {
                var formation = MainGameManager.instance.GetCurrentPanelForRole(characterModel.role);
                if (formation != null)
                {
                    var panel = MainGameManager.instance.GetGameObjectFromFormation(formation, BattleManager.allPanelManagers);
                    panel.GetComponent<PanelsManager>().currentOccupier = this.gameObject;
                    baseManager.movementManager.currentPanel = panel;
                } else
                {
                    var panel = BattleManager.GetRandomPanel(true);
                    panel.GetComponent<PanelsManager>().currentOccupier = this.gameObject;
                    baseManager.movementManager.currentPanel = panel;
                }
            }
        }
    }
}

