using AssemblyCSharp;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using static AssemblyCSharp.miniMapIconBase;

namespace Assets.scripts.Managers.ExplorerScene_Scripts.RouteEditor
{
#if UNITY_EDITOR // => Ignore from here to next endif if not in editor

    [CustomEditor(typeof(MapEditorIconTrigger))]
    public class MapEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MapEditorIconTrigger miniMapController = (MapEditorIconTrigger)target;
            //Yes the directions are backwards because currently the maps are generated backwards and it shares the logic..
            if (GUILayout.Button("Add Route Left"))
            {
                miniMapController.CreateIconInDirection(lineDirectionEnum.right);
            }

            if (GUILayout.Button("Add Route Right"))
            {
                miniMapController.CreateIconInDirection(lineDirectionEnum.left);
            }

            if (GUILayout.Button("Add Route Up"))
            {
                miniMapController.CreateIconInDirection(lineDirectionEnum.down);
            }

            if (GUILayout.Button("Clear Connected Routes"))
            {
                miniMapController.ClearRoutes();
            }
        }

        /*RaycastHit2D DetectItem()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            Collider[] hitColliders = Physics.OverlapSphere(mousePos, 100000);
            Debug.Log(hitColliders.Count());
            foreach (Collider collider in hitColliders)
            {
                Debug.Log(collider.gameObject.name);
            }

            return hit;
        }

        private void OnEnable()
        {
            parentIcon = (MapEditorIconTrigger)target;
        }

        private void OnSceneGUI()
        {
            Event e = Event.current;

            if (e.type == EventType.MouseUp)
            {

                //HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                // Cast a ray from the mouse position in the Scene view
                //Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                var hit = DetectItem();
                if (hit && hit.collider.gameObject == parentIcon.gameObject)
                {
                    if (e.type == EventType.MouseDrag && !hoveredObject)
                    {
                        hoveredObject = hit.collider.gameObject;
                        parentIcon.ToggleDirectionVisibility(hit.collider.gameObject);
                       //Debug.Log($"Hovering over {hit.collider.gameObject.name}");
                    }
                    if (e.type == EventType.MouseUp)
                    {
                        parentIcon.TriggerEvent(hit.collider.gameObject);
                        hoveredObject = null;
                    }
                    //DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    /* if (e.type == EventType.MouseUp .DragPerform)
                     {
                         DragAndDrop.AcceptDrag();
                         foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                         {
                             if (draggedObject is GameObject draggedGameObject)
                             {
                                 targetTrigger.TriggerEvent(draggedGameObject);
                             }
                         }
                         
                     }*
                }

               // e.Use();
            }
        }*/
    }
#endif
}
