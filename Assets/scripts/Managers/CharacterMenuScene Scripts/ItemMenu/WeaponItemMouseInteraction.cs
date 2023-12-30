using AssemblyCSharp;
using Assets.scripts.Helpers.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using static AssemblyCSharp.itemBehaviour;

namespace Assets.scripts.Managers.CharacterMenuScene_Scripts.ItemMenu
{
    public class WeaponItemMouseInteraction : GenericMouseInteraction
    {
        //public delegate void RightClickActionWithProp(weaponType type);
        public UnityEvent rightClickActionWithProp;

        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (rightClickActionWithProp != null)
                {
                  // rightClickActionWithProp.Invoke(gameObject.GetComponent<itemBehaviour>().type);
                }
            }
        }
    }
}
