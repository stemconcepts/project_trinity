using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR // => Ignore from here to next endif if not in editor
using UnityEditor;
#endif

namespace Assets.scripts.Helpers.Assets
{
    public class EquipmentFinder : MonoBehaviour
    {
        public List<WeaponModel> AllWeapons = new List<WeaponModel>();
        public List<Bauble> AllBaubles = new List<Bauble>();

        #if UNITY_EDITOR
        public void GetAndAssignWeapons()
        {
            AllWeapons = AssetFinder.GetAllWeapons();
            AllBaubles = AssetFinder.GetAllBaubles();
        }
#endif

        public List<Bauble> GetAllBaubles(bool returnOwned)
        {
            return AllBaubles
                .Where(bauble =>
                    returnOwned == bauble.owned)
                .ToList();
        }

        public List<WeaponModel> GetAllWeapons(bool returnOwned)
        {
            return AllWeapons
                .Where(weapon =>
                    returnOwned == weapon.owned)
                .ToList();
        }

        public List<WeaponModel> GetTankWeapons(bool returnOwned)
        {
            return AllWeapons
                .Where(weapon =>
                    returnOwned == weapon.owned &&
                    weapon.type == WeaponModel.weaponType.bladeAndBoard || weapon.type == WeaponModel.weaponType.heavyHanded)
                .ToList();
        }

        public List<WeaponModel> GetHealerWeapons(bool returnOwned)
        {
            return AllWeapons
                .Where(weapon =>
                    returnOwned == weapon.owned &&
                    weapon.type == WeaponModel.weaponType.cursedGlove || weapon.type == WeaponModel.weaponType.glove)
                .ToList();
        }

        public List<WeaponModel> GetDPSWeapons(bool returnOwned)
        {
            return AllWeapons
                .Where(weapon =>
                    returnOwned == weapon.owned &&
                    weapon.type == WeaponModel.weaponType.clawAndCannon || weapon.type == WeaponModel.weaponType.dualBlades)
                .ToList();
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(EquipmentFinder))]
    internal class EquipmentFinderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EquipmentFinder finder = (EquipmentFinder)target;
            if (GUILayout.Button("Update Equipments"))
            {
                finder.GetAndAssignWeapons();
            }
        }
    }
    #endif
}
