using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

namespace AssemblyCSharp
{
    public class miniMapIconBase : MonoBehaviour
    {
        public string LocationId;
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
        public bool isMainRoute, isCustomRoute;
        public int depth;
        public int masterDepth;

        public GameObject GetGameObjectByDirection(lineDirectionEnum direction)
        {
            switch (lineDirection)
            {
                case lineDirectionEnum.right:
                    return lineRight;
                case lineDirectionEnum.left:
                    return lineLeft;
                case lineDirectionEnum.down:
                    return lineDown;
                default:
                    return null;
            }
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

        public (string alphabet, int index) GetLocationId()
        {
            if (LocationId == "")
            {
                print($"Icon {name} has no location Id? mainRoute: {isMainRoute}"); 
            }
            var letter = LocationId.Substring(0, 1);
            var index = LocationId.Length > 2 ? LocationId.Substring(1, 2) : LocationId.Substring(1, 1);
            return (letter, int.Parse(index));
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

        public lineDirectionEnum ChooseDirection(Transform iconParentTransform, lineDirectionEnum forceDirection = lineDirectionEnum.none)
        {
            var parentRoomIcon = iconParentTransform.gameObject.GetComponent<miniMapIconBase>();
            var direction = forceDirection == lineDirectionEnum.none ? (lineDirectionEnum)MainGameManager.instance.ReturnRandom(3) : forceDirection;

            // Check Direction isnt taken
            if (parentRoomIcon != null)
            {
                direction = parentRoomIcon.CorrectDirection(direction);
            }

            switch (direction)
            {
                case lineDirectionEnum.down:
                    ShowLine(lineDirectionEnum.down);
                    this.gameObject.transform.position = new Vector3(iconParentTransform.position.x, iconParentTransform.position.y + 0.4f);
                    break;
                case lineDirectionEnum.right:
                    ShowLine(lineDirectionEnum.right);
                    this.gameObject.transform.position = new Vector3(iconParentTransform.position.x - 0.4f, iconParentTransform.position.y);
                    break;
                case lineDirectionEnum.left:
                    ShowLine(lineDirectionEnum.left);
                    this.gameObject.transform.position = new Vector3(iconParentTransform.position.x + 0.4f, iconParentTransform.position.y);
                    break;
                default:
                    break;
            }

            return direction;
        }

        public lineDirectionEnum ChooseCustomRouteDirection(Transform iconParentTransform, lineDirectionEnum forceDirection = lineDirectionEnum.none)
        {
            var parentRoomIcon = iconParentTransform.gameObject.GetComponent<miniMapIconBase>();
            var direction = forceDirection == lineDirectionEnum.none ? (lineDirectionEnum)MainGameManager.instance.ReturnRandom(3) : forceDirection;
            var customRouteGroupHolder = this.gameObject.transform.parent;

            // Check Direction isnt taken
            if (parentRoomIcon != null)
            {
                direction = parentRoomIcon.CorrectDirection(direction);
            }

            switch (direction)
            {
                case lineDirectionEnum.down:
                    ShowLine(lineDirectionEnum.down);
                    customRouteGroupHolder.position = new Vector3(iconParentTransform.position.x, iconParentTransform.position.y + 0.4f);
                    break;
                case lineDirectionEnum.right:
                    ShowLine(lineDirectionEnum.right);
                    customRouteGroupHolder.position = new Vector3(iconParentTransform.position.x - 0.4f, iconParentTransform.position.y);
                    break;
                case lineDirectionEnum.left:
                    ShowLine(lineDirectionEnum.left);
                    customRouteGroupHolder.position = new Vector3(iconParentTransform.position.x + 0.4f, iconParentTransform.position.y);
                    break;
                default:
                    break;
            }

            return direction;
        }

        //THIS SUCKS .. should be using the above call ChooseDirection but for some reason the value i adjust by varies on live than it does in edit mode.. possibly because of the parent.... will have to fix with
        //in EDIT MODE catch or something so they use the same method
        public lineDirectionEnum ChooseDirectionEditor(Transform iconParentTransform, lineDirectionEnum forceDirection = lineDirectionEnum.none)
        {
            var parentRoomIcon = iconParentTransform.gameObject.GetComponent<miniMapIconController>();
            var direction = forceDirection == lineDirectionEnum.none ? (lineDirectionEnum)MainGameManager.instance.ReturnRandom(3) : forceDirection;

            // Check Direction isnt taken
            if (parentRoomIcon != null)
            {
                direction = parentRoomIcon.CorrectDirection(direction);
            }

            switch (direction)
            {
                case lineDirectionEnum.down:
                    ShowLine(lineDirectionEnum.down);
                    this.gameObject.transform.position = new Vector3(iconParentTransform.position.x, iconParentTransform.position.y + 20.6f);
                    break;
                case lineDirectionEnum.right:
                    ShowLine(lineDirectionEnum.right);
                    this.gameObject.transform.position = new Vector3(iconParentTransform.position.x - 20.6f, iconParentTransform.position.y);
                    break;
                case lineDirectionEnum.left:
                    ShowLine(lineDirectionEnum.left);
                    this.gameObject.transform.position = new Vector3(iconParentTransform.position.x + 20.6f, iconParentTransform.position.y);
                    break;
                default:
                    break;
            }

            return direction;
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