using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class LogIndicatorController : MonoBehaviour
    {
        public BattleManager.TurnEnum Turn;
        Image ImageController;
        private Color ENEMY_TURN_COLOR = new Color(0.2f, 0.2f, 0.2f, 0.4f);
        private Color PLAYER_TURN_COLOR = new Color(0f, 0f, 0f, 0f);

        public void SetTurn(BattleManager.TurnEnum turn)
        {
            Turn = turn;
            var turnColor = turn == BattleManager.TurnEnum.PlayerTurn ? PLAYER_TURN_COLOR : ENEMY_TURN_COLOR;
            ImageController.color = turnColor;
        }

        private void Awake()
        {
            ImageController = GetComponent<Image>();
            if (ImageController == null)
            {
                Debug.LogError("Image component not found on LogIndicatorController.");
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}