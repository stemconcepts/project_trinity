using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Generic_Manager : MonoBehaviour {
        public void DestroyObject(){
            BattleManager.battleDetailsManager.DestroyObject(this.gameObject);
        }
    }
}