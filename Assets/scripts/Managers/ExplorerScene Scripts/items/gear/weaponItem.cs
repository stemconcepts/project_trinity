using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class weaponItem : ItemBase
    {
        public WeaponModel weapon;

        public void SetGearData()
        {
            itemDesc = weapon.WeaponDescription;
            canStack = false;
            quality = weapon.quality;
            switch (weapon.type)
            {
                case WeaponModel.weaponType.bladeAndBoard:
                    classReq = classRestriction.Guardian;
                    break;
                case WeaponModel.weaponType.heavyHanded:
                    classReq = classRestriction.Guardian;
                    break;
                case WeaponModel.weaponType.dualBlades:
                    classReq = classRestriction.Stalker;
                    break;
                case WeaponModel.weaponType.clawAndCannon:
                    classReq = classRestriction.Stalker;
                    break;
                case WeaponModel.weaponType.glove:
                    classReq = classRestriction.Walker;
                    break;
                case WeaponModel.weaponType.cursedGlove:
                    classReq = classRestriction.Walker;
                    break;
                default:
                    classReq = classRestriction.None;
                    break;
            }
        }
    }
}