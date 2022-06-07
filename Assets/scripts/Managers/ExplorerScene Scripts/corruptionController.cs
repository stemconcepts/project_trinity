using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.scripts.Managers.ExplorerScene_Scripts
{
    public class corruptionController : MonoBehaviour
    {
        public TextMeshPro text;
        int corruptionAmount = 0;
        public void AddCorruption(int amount)
        {
            corruptionAmount += amount;
            text.text = corruptionAmount.ToString();
        }

        private void Start()
        {
            text.text = corruptionAmount.ToString();
        }
    }
}