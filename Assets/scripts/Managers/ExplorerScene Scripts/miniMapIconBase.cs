using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class miniMapIconBase : MonoBehaviour
    {
        public bool isCurrent;
        public string label;
        public Color startColor;
        public Color endColor;
        public Color origColor;
        public Color hoverColor;
        public Color activeColor;
        public GameObject lineDown;
        public GameObject lineRight;
        public GameObject lineLeft;
        public lineDirectionEnum lineDirection;
        public enum lineDirectionEnum
        {
            down = 0,
            right = 1,
            left = 2,
            none = 3
        }

        public miniMapIconController ConvertToIconController(miniMapIconBase mmb)
        {
            return new miniMapIconController()
            {
                isCurrent = mmb.isCurrent,
                label = mmb.label,
                startColor = mmb.startColor,
                endColor = mmb.endColor,
                origColor = mmb.origColor,
                hoverColor = mmb.hoverColor,
                activeColor = mmb.activeColor,
                lineDown = mmb.lineDown,
                lineRight = mmb.lineRight,
                lineLeft = mmb.lineLeft,
                lineDirection = mmb.lineDirection
            };
        }

        public void ShowLine(lineDirectionEnum lineDirection)
        {
            switch (lineDirection)
            {
                case lineDirectionEnum.down:
                    lineDown.SetActive(true);
                    //lineLeft.SetActive(false);
                    //lineRight.SetActive(false);
                    break;
                case lineDirectionEnum.right:
                    lineRight.SetActive(true);
                    //lineLeft.SetActive(false);
                    //lineDown.SetActive(false);
                    break;
                case lineDirectionEnum.left:
                    lineLeft.SetActive(true);
                    //lineRight.SetActive(false);
                    //lineDown.SetActive(false);
                    break;
                case lineDirectionEnum.none:
                    lineLeft.SetActive(false);
                    lineRight.SetActive(false);
                    lineDown.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        public void SetStartIcon()
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = startColor;
        }

        public void SetEndIcon()
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = endColor;
        }

        public void SetActive(bool active)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = active ? activeColor : origColor;
        }
    }
}