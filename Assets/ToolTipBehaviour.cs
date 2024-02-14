using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class ToolTipBehaviour : MonoBehaviour
    {
        public RectTransform rectTransform;
        public RectTransform canvas;
        public GameObject title;
        public GameObject desc;
        private TweenerCore<Vector2, Vector2, VectorOptions> currentTween;

        void Awake()
        {
            SetInitPivot();
            if (!canvas)
            {
                canvas = GameObject.Find("Canvas - Tooltip").GetComponent<RectTransform>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            //SetInitPivot();
            SetPosition();
        }

        private void SetInitPivot()
        {
            var isaboveMiddleX = (Screen.width / 2) > Input.mousePosition.x;
            var isaboveMiddleY = (Screen.height / 2) > Input.mousePosition.y;
            var y = isaboveMiddleY ? 0 : 1;
            var x = isaboveMiddleX ? 0 : 1;
            rectTransform.pivot = new Vector2(x, y);
        }

        private void SetPosition()
        {
            var descWidth = desc.GetComponent<Text>();
            var titleWidth = title.GetComponent<Text>();
            if (titleWidth != null)
            {
                var newWidth = titleWidth.preferredWidth > 400 ? 400 : titleWidth.preferredWidth < 100 ? 100 : titleWidth.preferredWidth;
                this.gameObject.GetComponent<LayoutElement>().preferredWidth = newWidth;
            }
            if (descWidth && descWidth.preferredWidth > titleWidth.preferredWidth)
            {
                var newWidth = descWidth.preferredWidth > 400 ? 400 : descWidth.preferredWidth < 100 ? 100 : descWidth.preferredWidth;
                this.gameObject.GetComponent<LayoutElement>().preferredWidth = newWidth;
            }

            var isaboveMiddleY = (Screen.height / 2) > Input.mousePosition.y;

            var offsetY = isaboveMiddleY ? 10 : -40;
            var newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y + offsetY, Input.mousePosition.z);
            rectTransform.position = newPosition;
        }

        private void TweenToolTipPivot(int pivotX, int pivotY)
        {
            var pivotChanged = (int)rectTransform.pivot.x != pivotX || (int)rectTransform.pivot.y != pivotY;
            if ((pivotChanged && currentTween == null) || (pivotChanged && currentTween != null && !currentTween.IsPlaying()))
            {
                print("spam");
                currentTween = rectTransform.DOPivot(new Vector2(pivotX, pivotY), 0.4f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            var isaboveMiddleX = (Screen.width / 2) > Input.mousePosition.x;
            var isaboveMiddleY = (Screen.height / 2) > Input.mousePosition.y;
            var y = isaboveMiddleY ? 0 : 1;
            var x = isaboveMiddleX ? 0 : 1;
            /*var descWidth = desc.GetComponent<Text>();
            var titleWidth = title.GetComponent<Text>();
            if (titleWidth != null)
            {
                var newWidth = titleWidth.preferredWidth > 400 ? 400 : titleWidth.preferredWidth < 100 ? 100 : titleWidth.preferredWidth;
                this.gameObject.GetComponent<LayoutElement>().preferredWidth = newWidth;
            }
            if (descWidth && descWidth.preferredWidth > titleWidth.preferredWidth)
            {
                var newWidth = descWidth.preferredWidth > 400 ? 400 : descWidth.preferredWidth < 100 ? 100 : descWidth.preferredWidth;
                this.gameObject.GetComponent<LayoutElement>().preferredWidth = newWidth;
            }*/

            SetPosition();
            TweenToolTipPivot(x, y);

            /*var offsetY = isaboveMiddleY ? 10 : -40;
            var newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y + offsetY, Input.mousePosition.z);
            rectTransform.position = newPosition;*/
        }
    }
}