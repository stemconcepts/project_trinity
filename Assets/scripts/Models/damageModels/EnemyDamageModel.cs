using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class EnemyDamageModel : BaseDamageModel
    {
        public new EnemyCharacterManager dmgSource;
        public new List<CharacterManager> dueDmgTargets;
        public new BaseCharacterManagerGroup baseManager;
    }
}