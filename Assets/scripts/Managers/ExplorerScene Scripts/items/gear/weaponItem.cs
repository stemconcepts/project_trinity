using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class weaponItem : ItemBase
    {
        public weaponModel weapon;
        [Range(0.0f, 1.0f)]
        public float dropChancePercentage;

        public void SetGearData()
        {
            itemDesc = weapon.WeaponDescription;
            canStack = false;
            quality = weapon.quality;
            switch (weapon.type)
            {
                case weaponModel.weaponType.bladeAndBoard:
                    classReq = classRestriction.Guardian;
                    break;
                case weaponModel.weaponType.heavyHanded:
                    classReq = classRestriction.Guardian;
                    break;
                case weaponModel.weaponType.dualBlades:
                    classReq = classRestriction.Stalker;
                    break;
                case weaponModel.weaponType.clawAndCannon:
                    classReq = classRestriction.Stalker;
                    break;
                case weaponModel.weaponType.glove:
                    classReq = classRestriction.Walker;
                    break;
                case weaponModel.weaponType.cursedGlove:
                    classReq = classRestriction.Walker;
                    break;
                default:
                    classReq = classRestriction.None;
                    break;
            }
        }
    }
}