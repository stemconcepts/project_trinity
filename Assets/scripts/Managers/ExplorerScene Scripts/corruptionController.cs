using AssemblyCSharp;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static AssemblyCSharp.BaseCharacterModel;

namespace Assets.scripts.Managers.ExplorerScene_Scripts
{
    public class corruptionController : MonoBehaviour
    {
        public TextMeshPro text;
        public int corruptionAmount = 0;

        /// <summary>
        /// Add corruption points to the player
        /// </summary>
        /// <param name="amount"></param>
        public void AddCorruption(int amount)
        {
            corruptionAmount += amount;
            text.text = corruptionAmount.ToString();
            if (MainGameManager.instance.GetChanceByPercentage(corruptionAmount/10))
            {
                //AddRandomStatusToPlayers();
                AddRandomStatusToNextEnemy();
            }
        }

        public void ReduceCorruption(int amount)
        {
            corruptionAmount -= amount;
            corruptionAmount = corruptionAmount < 0 ? 0 : corruptionAmount;
            text.text = corruptionAmount.ToString();
        }

        /// <summary>
        /// Adds Random Start to Players based on environmental settings
        /// </summary>
        public void AddRandomStatusToPlayers()
        {
            var explorerStatuses = MainGameManager.instance.exploreManager.GetDungeonStatus(true);
            var i = Random.Range(0, explorerStatuses.Count);

            var explorerStatus = explorerStatuses[i];
            MainGameManager.instance.SceneManager.playerStatuses.Add(explorerStatus);
            /*var role = new List<RoleEnum>()
                {
                    RoleEnum.tank, RoleEnum.healer, RoleEnum.dps
                };
            if (explorerStatus.affectAll)
            {
                AddToPlayerStatus(role, explorerStatus.statuses);
            }
            else
            {
                var c = Random.Range(0, 3);
                role.RemoveRange(c, 1);
                AddToPlayerStatus(role, explorerStatus.statuses);
            }*/
        }

        /// <summary>
        /// Adds Random Start to Enemies based on environmental settings
        /// </summary>
        public void AddRandomStatusToNextEnemy()
        {
            var explorerStatuses = MainGameManager.instance.exploreManager.GetDungeonStatus(false);
            var i = Random.Range(0, explorerStatuses.Count);

            var explorerStatus = explorerStatuses[i];
            if (!MainGameManager.instance.SceneManager.enemyStatuses.Any(explorerStatusItem => explorerStatusItem.id == explorerStatus.id) || explorerStatus.canStack)
            {
                MainGameManager.instance.SceneManager.enemyStatuses.Add(explorerStatus);
            }
            /*if (explorerStatus.affectAll)
            {
                
                //AddToPlayerStatus(role, explorerStatus.statuses);
            }
            else
            {
                //var c = Random.Range(0, 3);
                //role.RemoveRange(c, 1);
                //AddToPlayerStatus(role, explorerStatus.statuses);
            }*/
        }

        /// <summary>
        /// Adds statuses to starting status that will be active at the start of the battle
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="explorerStatus"></param>
        void AddToPlayerStatus(List<RoleEnum> roles, List<ExplorerStatus> explorerStatus)
        {
            roles.ForEach(o =>
            {
                switch (o)
                {
                    case RoleEnum.tank:
                        //tankStatus = explorerStatus;
                        break;
                    case RoleEnum.healer:
                        //healerStatus = explorerStatus;
                        break;
                    case RoleEnum.dps:
                        //dpsStatus = explorerStatus;
                        break;
                    default:
                        break;
                }
            });
        }

        private void OnMouseUp()
        {
            MainGameManager.instance.DisableEnableLiveBoxColliders(false);
            MainGameManager.instance.gameMessanger.DisplayMessage(MainGameManager.instance.GetText("CorruptionCounter"), headerText: "Corruption", closeAction : () =>
            {
                MainGameManager.instance.DisableEnableLiveBoxColliders(true);
            });
        }

        private void Start()
        {
            text.text = corruptionAmount.ToString();
        }
    }
}