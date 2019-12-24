using UnityEngine;
using UnityEditor;
using static AssemblyCSharp.PhaseManager;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class PhaseBuff
    {
        public SingleStatusModel singleStatusModel;
        public float statusPower;
        public float statusDuration;
    }
    [System.Serializable]
    public class PhaseThreshHold
    {
        public float maxHealthPercentage;
        public float minHealthPercentage;
    }
    [System.Serializable]
    public class PhaseModel
    {
            public PhaseThreshHold healthThreshhold;
            public EnemyPhase enemyPhase;
            public List<enemySkill> phaseSkills;
            public List<PhaseBuff> phaseBuffs;
            public string skinChange;
            public string phaseChangeAnimation;
    }
}