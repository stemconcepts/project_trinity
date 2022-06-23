using AssemblyCSharp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static AssemblyCSharp.BaseCharacterModel;

namespace Assets.scripts.Managers.ExplorerScene_Scripts
{
    public class corruptionController : MonoBehaviour
    {
        public TextMeshPro text;
        int corruptionAmount = 0;

        /// <summary>
        /// Add corruption points to the player
        /// </summary>
        /// <param name="amount"></param>
        public void AddCorruption(int amount)
        {
            corruptionAmount += amount;
            text.text = corruptionAmount.ToString();
            if (true || GameManager.GetChanceByPercentage(corruptionAmount/100))
            {
                AddRandomStatus();
            }
        }

        /// <summary>
        /// Adds Random Start to players based on environmental settings
        /// </summary>
        public void AddRandomStatus()
        {
            var s = ExploreManager.GetDungeonStatus(true);
            var i = Random.Range(0, s.Count);

            var e = s[i];
            var l = new List<RoleEnum>()
                {
                    RoleEnum.tank, RoleEnum.healer, RoleEnum.dps
                };
            if (e.affectAll)
            {
                BattleManager.AddToPlayerStatus(l, e.status);
            }
            else
            {
                var c = Random.Range(0, 3);
                l.RemoveRange(c, 1);
                BattleManager.AddToPlayerStatus(l, e.status);
            }
        }

        private void Start()
        {
            text.text = corruptionAmount.ToString();
        }
    }
}