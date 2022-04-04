using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace AssemblyCSharp
{
    public class CharacterManager : BaseCharacterManager/*<Character_Manager_Group>*/
    {
        //public override Character_Model characterModelTest { get; set; }
        //[Header("Character Model")]
        //public Character_Model characterModel;

        void Awake()
        {
            baseManager = this.gameObject.GetComponent<CharacterManagerGroup>();
            characterModel = this.gameObject.GetComponent<CharacterModel>();
            if (template != null)
            {
                resistances = useResistance ? template.resistances : resistances;
                //this.characterModel = useStats ? template.this.characterModel as Character_Model : this.characterModel;
            }
        }

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
        /*void UpdateBarSize(Character_Model characterModel)
        {
            characterModel.current_health = characterModel.Health;
            characterModel.sliderScript.maxValue = characterModel.full_health;
            characterModel.sliderScript.value = characterModel.current_health;
            characterModel.healthBarText.text = characterModel.current_health.ToString();
        }*/

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
                baseManager.movementManager.currentPanel = BattleManager.GetRandomPanel(true);
            }
            baseManager.movementManager.currentPanel.GetComponent<PanelsManager>().currentOccupier = gameObject;
        }

        /*public T GetFriendlyTarget<T>()
        {
            var otherTargets = Battle_Manager.GetFriendlyCharacterManagers();
            otherTargets = otherTargets.Where(o => o.name != gameObject.name).ToList();
            var targetCount = otherTargets.Count;
            var i = UnityEngine.Random.Range(0, targetCount);
            return otherTargets[i].GetComponent<T>();
        }*/
    }
}

