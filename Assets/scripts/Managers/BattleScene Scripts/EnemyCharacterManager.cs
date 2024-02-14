using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace AssemblyCSharp
{
    public class EnemyCharacterManager : BaseCharacterManager
    {
        public GameObject enemyCharacterSelector;
        public Sprite enemySprite;

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
                initialSetupDone = true;
            }
            if (baseManager.movementManager.currentPanel == null)
            {
                baseManager.movementManager.currentPanel = BattleManager.GetRandomPanel(false);
            }
            baseManager.movementManager.currentPanel.GetComponent<PanelsManager>().currentOccupier = gameObject;
        }
    }
}