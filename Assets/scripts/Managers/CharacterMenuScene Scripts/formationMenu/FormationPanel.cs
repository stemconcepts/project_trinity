using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static AssemblyCSharp.PanelsManager;

namespace Assets.scripts.Managers.CharacterMenuScene_Scripts.formationMenu
{
    public class FormationPanel : MonoBehaviour
    {
        public RoleEnum Occupier;
        [Range(0, 2)]
        public int PanelNumber;
        public voidZoneType voidZonesTypes;
        public bool IsThreatPanel;
        //public Color PanelColor;
        public Color DefaultColor = new Color(1f, 1f, 1f, 1f);
        public Image ImageScript;
        public FormationManager FormationManager;

        public void Start()
        {
            ImageScript = GetComponent<Image>();
            SetDefaultColor();
        }

        public void OnMouseUp()
        {
            if (FormationManager != null)
            {
                FormationManager.SetCurrentPanel(this);
                FormationManager.ToggleClassSelector();
            }
        }

        public void SetOccupier(RoleEnum newOccupier)
        {
            this.Occupier = newOccupier;
            switch (newOccupier)
            {
                case RoleEnum.none:
                    this.ImageScript.color = DefaultColor;
                    break;
                case RoleEnum.tank:
                    this.ImageScript.color = new Color(1f, 0.2f, 0.2f, 1f);
                    break;
                case RoleEnum.healer:
                    this.ImageScript.color = new Color(0.2f, 0.2f, 1f, 1f);
                    break;
                case RoleEnum.dps:
                    this.ImageScript.color = new Color(0.2f, 1f, 0.2f, 1f);
                    break;
                default:
                    SetDefaultColor();
                    break;
            }
        }

        private void SetDefaultColor()
        {
            this.ImageScript.color = IsThreatPanel ? new Color(0.9f, 0.9f, 0.1f, 1f) : DefaultColor;
        }

        public void ClearOccupier()
        {
            this.Occupier = RoleEnum.none;
            SetDefaultColor();
        }
    }
}
