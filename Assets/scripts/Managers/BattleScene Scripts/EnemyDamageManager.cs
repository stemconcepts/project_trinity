using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class EnemyDamageManager : BaseDamageManager
    {
        void Start()
        {
            baseManager = this.gameObject.GetComponent<EnemyCharacterManagerGroup>();
            battleDetailsManager = BattleManager.battleDetailsManager;
        }
    }
}
