using UnityEngine;
using UnityEditor;
namespace AssemblyCSharp
{
    [System.Serializable]
    public class PreRequisiteModel
    {
        public preRequisiteTypeEnum preRequisiteType;
        public enum preRequisiteTypeEnum
        {
            none,
            summonPanels,
            buffMinions,
            spellsOnCooldown
        }
        [ConditionalHide("preRequisiteType", (int)preRequisiteTypeEnum.summonPanels, false)]
        public int amount;
    }
}