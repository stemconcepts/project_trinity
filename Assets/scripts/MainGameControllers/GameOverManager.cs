using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts.MainGameControllers
{
    public class GameOverManager : MonoBehaviour
    {
        public void ReloadGame()
        {
            MainGameManager.instance.SceneManager.UnLoadScene("GameOver");
            MainGameManager.instance.SceneManager.UnLoadScene("battle");
        }
    }
}
