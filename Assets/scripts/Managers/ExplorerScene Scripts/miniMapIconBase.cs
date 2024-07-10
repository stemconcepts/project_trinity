using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class miniMapIconBase : MonoBehaviour
    {
        public string id;
        public bool isCurrent;
        public string label;
        public Color startColor;
        public Color endColor;
        public Color origColor;
        public Color hoverColor;
        public Color highlightColor;
        public Color activeColor;
        public Color visitedColor;
        public Color detourConnectorColor;
        public Color detourColor;
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
        public bool isMainIcon, isCustomIcon;
        public int depth;
        public int masterDepth;

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

        public lineDirectionEnum CorrectDirection(lineDirectionEnum direction)
        {
            if (direction == lineDirection || lineDirection == lineDirectionEnum.down)
            {
                return direction;
            } else 
            {

                var randomDirection = Random.Range(0, 1);

                switch (lineDirection)
                {
                    case lineDirectionEnum.right:
                        return randomDirection == 0 ? lineDirectionEnum.down : lineDirectionEnum.right;
                    case lineDirectionEnum.left:
                        return randomDirection == 0 ? lineDirectionEnum.down : lineDirectionEnum.left;
                    case lineDirectionEnum.none:
                        return lineDirectionEnum.none;
                    default:
                        return lineDirectionEnum.none;
                }
            }
        }

        public lineDirectionEnum GetFreeDirection()
        {
            var randomDirection = Random.Range(0, 1);

            switch (lineDirection)
            {
                case lineDirectionEnum.right:
                    return randomDirection == 0 ? (lineDirectionEnum)randomDirection : lineDirectionEnum.right;
                case lineDirectionEnum.left:
                    return randomDirection == 0 ? (lineDirectionEnum)randomDirection : lineDirectionEnum.left;
                case lineDirectionEnum.none:
                    return lineDirectionEnum.none;
                default:
                    return lineDirectionEnum.none;
            }
        }

        public void ShowLine(lineDirectionEnum lineDirection)
        {
            this.lineDirection = lineDirection;
            switch (lineDirection)
            {
                case lineDirectionEnum.down:
                    lineDown.SetActive(true);
                    break;
                case lineDirectionEnum.right:
                    lineRight.SetActive(true);
                    break;
                case lineDirectionEnum.left:
                    lineLeft.SetActive(true);
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
            origColor = startColor;
        }

        public void SetEndIcon()
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = endColor;
        }

        public void SetActive(bool active)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = active ? activeColor : origColor;
        }

        public void SetVisited()
        {
            origColor = visitedColor;
        }

        public void SetHighlighted(bool highlight)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = highlight ? highlightColor : origColor;
            /*switch (lineDirection)
            {
                case lineDirectionEnum.down:
                    lineDown.GetComponent<SpriteRenderer>().color = highlight ? highlightColor : origColor;
                    break;
                case lineDirectionEnum.right:
                    lineRight.GetComponent<SpriteRenderer>().color = highlight ? highlightColor : origColor;
                    break;
                case lineDirectionEnum.left:
                    lineLeft.GetComponent<SpriteRenderer>().color = highlight ? highlightColor : origColor;
                    break;
                case lineDirectionEnum.none:
                    break;
                default:
                    break;
            }*/
        }

        public void SetDetourColour()
        {
            origColor = detourColor;
        }

        public void SetDetourConnectorColour()
        {
            origColor = detourConnectorColor;
        }

        public void SetObjectName(string value)
        {
            this.gameObject.name = value;
        }
    }
}