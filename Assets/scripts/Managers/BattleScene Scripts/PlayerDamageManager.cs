using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class PlayerDamageManager : BaseDamageManager
    {
        void Start()
        {
            baseManager = this.gameObject.GetComponent<CharacterManagerGroup>();
            battleDetailsManager = BattleManager.battleDetailsManager;
        }
    }
}