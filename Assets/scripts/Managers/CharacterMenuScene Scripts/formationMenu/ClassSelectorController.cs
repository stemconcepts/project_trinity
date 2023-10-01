using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts.Managers.CharacterMenuScene_Scripts.formationMenu
{

    public class ClassSelectorController : MonoBehaviour
    {
        public RoleEnum Class;
        public FormationManager FormationManager;

        public void OnMouseUp()
        {
            if (FormationManager != null)
            {
                FormationManager.ClearRoleFromPanels(Class);
                FormationManager.CurrentPanel.SetOccupier(Class);
                FormationManager.SaveFormationData(FormationManager.CurrentPanel);
            }
        }
    }
}
