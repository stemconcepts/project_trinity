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
        public int statusTurnDuration;
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
            public List<GameObject> summonList = new List<GameObject>();
            public List<enemySkill> phaseSkills  = new List<enemySkill>();
            public List<PhaseBuff> phaseBuffs = new List<PhaseBuff>();
            public string skinChange;
            public string phaseChangeAnimation;
    }
}