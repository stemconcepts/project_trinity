using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class StatusModel
    {
        public SingleStatusModel singleStatus;
        public BaseCharacterManagerGroup baseManager; 
        public SkillModel onHitSkill; 
        public int turnDuration = 2;
        public bool isFlat;
        public float power;
        public bool turnOff;
        public float triggerChance;
        public int stacks;
        public int turnToReset;
        public DamageColorEnum dmgTextColor;
        public Action effectOnEnd;

        public void SaveTurnToReset()
        {
            turnToReset = BattleManager.turnCount + (turnDuration * 2);
        }
    }

    public enum triggerGrp
    {
        None,
        Passive,
        OnTakingDmg,
        OnDealingDmg,
        OnHeal,
        OnMove,
        OnSkillCast,
        OnFirstRow,
        OnMiddleRow,
        OnLastRow
    };
}
