using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace AssemblyCSharp
{
    public class CharacterManager : BaseCharacterManager
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
    }
}

