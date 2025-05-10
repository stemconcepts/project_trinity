using AssemblyCSharp;
using DG.Tweening.Core.Easing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static AssemblyCSharp.miniMapIconBase;

namespace Assets.scripts.Managers.ExplorerScene_Scripts.RouteEditor
{
#if UNITY_EDITOR // => Ignore from here to next endif if not in editor

    //[ExecuteInEditMode]
    public class MapEditorIconTrigger : MonoBehaviour
    {
        public GameObject IconObject;
        public miniMapIconBase startingIconController;
        public miniMapIconBase parentIconController;
        public List<miniMapIconBase> ConnectedIcons = new List<miniMapIconBase>();

        public void CreateIconInDirection(lineDirectionEnum lineDirection)
        {
            if (startingIconController == null)
            {
                throw new Exception("Please add a starting icon controller to GameObject");
            };

            GameObject roomIcon = Instantiate(IconObject, this.transform.parent.transform);
            roomIcon.transform.position = this.gameObject.transform.position;
            miniMapIconBase mmc = roomIcon.GetComponent<miniMapIconBase>();
            mmc.isCustomRoute = true;
            mmc.ShowLine(lineDirectionEnum.none);

            MapEditorIconTrigger editor = roomIcon.GetComponent<MapEditorIconTrigger>();
            editor.startingIconController = startingIconController;

            miniMapCustomIcon mmi = this.gameObject.GetComponent<miniMapCustomIcon>();
            mmi.lineDirection = lineDirection;

            mmc.label = $"{IconObject.name}_{editor.ConnectedIcons.Count}";
            mmc.name = $"{IconObject.name}_{editor.ConnectedIcons.Count}_{lineDirection}";
            mmc.ChooseDirectionEditor(parentIconController.transform, lineDirection);

            ConnectedIcons.Add(mmc);
            editor.ConnectedIcons.Add(mmc);
        }

        public void ClearRoutes()
        {
            ConnectedIcons.ForEach(icon => {

                MapEditorIconTrigger editor = icon.gameObject.GetComponent<MapEditorIconTrigger>();
                editor.ConnectedIcons.ForEach((innerIcon) => GameObject.DestroyImmediate(innerIcon.gameObject));
                editor.ConnectedIcons.Clear();
                GameObject.DestroyImmediate(icon.gameObject);

            });
            ConnectedIcons.Clear();
        }

        private void Awake()
        {
            parentIconController = this.gameObject.GetComponent<miniMapIconController>();
        }
    }
#endif
}
