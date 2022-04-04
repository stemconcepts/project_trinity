using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace AssemblyCSharp
{
    public class EnemyCharacterManager : BaseCharacterManager/*<Enemy_Character_Manager_Group>*/
    {
        //public override Enemy_Character_Model characterModelTest { get; set; }
        //[Header("Character Model")]
        //public Enemy_Character_Model characterModel;

        void Update()
        {
            if (initialSetupDone)
            {
                UpdateBarSize(characterModel);
                ResetAbsorbPoints(characterModel);
                MaintainHealthValue(characterModel);
                //characterModel.attackedPos = baseManager.movementManager.posMarker.transform.position;
                characterModel.currentRotation = this.transform.rotation;
            }
        }

        void Awake()
        {
            characterModel = this.gameObject.GetComponent<EnemyCharacterModel>();
            baseManager = this.gameObject.GetComponent<EnemyCharacterManagerGroup>();
            if (this.characterModel.role == CharacterModel.RoleEnum.boss && healthBar == null)
            {
                healthBar = GameObject.Find("Slider_enemy");
            }
            if (template != null)
            {
                resistances = useResistance && template != null ? template.resistances : resistances;
                //this.characterModel = useStats && template != null ? template.this.characterModel as Enemy_Character_Model : this.characterModel;
            }
        }

        /*void UpdateBarSize()
        {
            this.characterModel.current_health = this.characterModel.Health;
            this.characterModel.sliderScript.maxValue = this.characterModel.full_health;
            this.characterModel.sliderScript.value = this.characterModel.current_health;
            this.characterModel.healthBarText.text = this.characterModel.current_health.ToString();
        }*/

        // Use this for initialization
        void Start()
        {
            if (healthBar == null)
            {
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

        /*public Enemy_Character_Manager_Group GetFriendlyTarget()
        {
            var otherTargets = Battle_Manager.GetEnemyCharacterManagers();
            otherTargets = otherTargets.Where(o => o.name != gameObject.name).ToList();
            var targetCount = otherTargets.Count;
            var i = UnityEngine.Random.Range(0, targetCount);
            return otherTargets[i].GetComponent<Enemy_Character_Manager_Group>();
        }*/
    }
}