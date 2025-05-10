using UnityEngine;
using System.Collections;
using NUnit.Framework;
using static AssemblyCSharp.miniMapIconBase;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class LineDirectionList
    {
        public lineDirectionEnum lineDirection;
    }
    public class miniMapCustomIcon : miniMapIconBase
    {
        public mapPositionEnum mapPosition;
        public bool mainPathOnly;
        public enum mapPositionEnum
        {
            none,
            start,
            end
        }
        public GameObject customRoom;
        public miniMapCustomIcon nextRoomIcon;
        public List<LineDirectionList> AllowedDirections = new List<LineDirectionList>();

        public bool IsDirectionFree(lineDirectionEnum direction)
        {
            return AllowedDirections.Any(blockedDir => blockedDir.lineDirection == direction);
        }

        public lineDirectionEnum CorrectDirectionCustom(lineDirectionEnum direction)
        {
            if (IsDirectionFree(direction))
            {
                return direction;
            }
            else
            {
                var randomDirection = Random.Range(0, 1);

                switch (direction)
                {
                    case lineDirectionEnum.right:
                        return randomDirection == 0 ? lineDirectionEnum.down : lineDirectionEnum.left;
                    case lineDirectionEnum.left:
                        return randomDirection == 0 ? lineDirectionEnum.down : lineDirectionEnum.right;
                    default:
                        return lineDirectionEnum.none;
                }
            }
        }

        public lineDirectionEnum ChooseCustomRouteDirection(Transform iconParentTransform, Transform customRouteTransform, lineDirectionEnum forceDirection = lineDirectionEnum.none)
        {
            var parentRoomIcon = iconParentTransform.gameObject.GetComponent<miniMapIconBase>();
            var direction = forceDirection == lineDirectionEnum.none ? (lineDirectionEnum)MainGameManager.instance.ReturnRandom(3) : forceDirection;

            // Check Direction isnt taken
            if (parentRoomIcon != null)
            {
                if (this.AllowedDirections.Count > 0)
                {
                    direction = this.CorrectDirectionCustom(direction);
                } else
                {
                    direction = parentRoomIcon.CorrectDirection(direction);
                }
            }

            switch (direction)
            {
                case lineDirectionEnum.down:
                    ShowLine(lineDirectionEnum.down);
                    customRouteTransform.position = new Vector3(iconParentTransform.position.x, iconParentTransform.position.y + 0.4f);
                    break;
                case lineDirectionEnum.right:
                    ShowLine(lineDirectionEnum.right);
                    customRouteTransform.position = new Vector3(iconParentTransform.position.x - 0.4f, iconParentTransform.position.y);
                    break;
                case lineDirectionEnum.left:
                    ShowLine(lineDirectionEnum.left);
                    customRouteTransform.position = new Vector3(iconParentTransform.position.x + 0.4f, iconParentTransform.position.y);
                    break;
                default:
                    break;
            }

            return direction;
        }
    }
}