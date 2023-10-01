using AssemblyCSharp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts.Managers.CharacterMenuScene_Scripts.formationMenu
{
    public class FormationManager : MonoBehaviour
    {
        public List<FormationPanel> Panels = new List<FormationPanel>();
        public FormationPanel CurrentPanel;
        public FormationPanel LastPanel;
        public GameObject ClassSelector;
        public GameObject PanelsHolder;

        public void Start()
        {
            GetPanels();
        }

        private void GetPanels()
        {
            if (PanelsHolder != null)
            {
                var panelFormations = PanelsHolder.GetComponentsInChildren<FormationPanel>();
                Panels = panelFormations.ToList();
            }
        }

        /// <summary>
        /// Logic stops character select from vanishing if a new panel is selected while its visible
        /// </summary>
        public void ToggleClassSelector()
        {
            var visible = (LastPanel == CurrentPanel && ClassSelector.activeSelf) ? ClassSelector.activeSelf : false;
            ClassSelector.SetActive(!visible);
        }

        public void SetCurrentPanel(FormationPanel panel)
        {
            LastPanel = CurrentPanel;
            CurrentPanel = panel;
        }

        public void ClearRoleFromPanels(RoleEnum role)
        {
            var panel = Panels.Where(panel => panel.Occupier == role).FirstOrDefault();
            if(panel != null)
            {
                panel.ClearOccupier();
            }
        }

        public void SaveFormationData(FormationPanel panel)
        {
            var formation = new FormationData();
            formation.Occupier = panel.Occupier.ToString();
            formation.PanelNumber = panel.PanelNumber;
            formation.VerticalFlag = panel.voidZonesTypes.ToString();
            SavedDataManager.SavedDataManagerInstance.SaveFormationData(formation);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
