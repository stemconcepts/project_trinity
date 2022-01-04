using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class enemyEncounterController : MonoBehaviour
    {
        public void GoToBattle()
        {
            Explore_Manager.gameManager.SceneManager.LoadBattle();
        }
    }
}