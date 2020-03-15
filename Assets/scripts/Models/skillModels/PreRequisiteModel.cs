using UnityEngine;
using UnityEditor;
namespace AssemblyCSharp
{
    [System.Serializable]
    public class PreRequisiteModel
    {
        public int amount;
        public preRequisiteTypeEnum preRequisiteType;
        public enum preRequisiteTypeEnum
        {
            none,
            summon
        }
        /*private bool IsPreRequisiteMet(enemySkill skill)
        {
            switch (skill.preRequisite.preRequisiteType)
            {
                case preRequisiteTypeEnum.summon:
                    var y = Battle_Manager.GetCharacterManagers(false);
                    var x = Battle_Manager.GetCharacterManagers(false).Find(o => o.characterModel.role == Character_Model.RoleEnum.minion);
                    return x != null;
                default:
                    return true;
            }
        }*/
    }
}