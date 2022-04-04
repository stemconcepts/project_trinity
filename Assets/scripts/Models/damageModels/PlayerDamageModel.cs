using Spine.Unity;
using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class PlayerDamageModel : BaseDamageModel
    {
        public new BaseCharacterManager dmgSource;
        public new List<BaseCharacterManager> dueDmgTargets;
        public new BaseCharacterManagerGroup baseManager;
    }
}
