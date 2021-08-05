using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class LineRendererController : MonoBehaviour
    {
        private LineRenderer lr;
        private bool mouseAsPointA;
        public List<Vector2> points;

        void Awake()
        {
            lr = GetComponent<LineRenderer>();
        }

        public void SetUpLine(List<Vector2> points)
        {
            lr.positionCount = points.Count;
            this.points = points;
        }

        public void SetUpLineFromMouse(Vector2 point)
        {
            this.mouseAsPointA = true;
            this.points = new List<Vector2>() {point};
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (mouseAsPointA)
            {
                lr.SetPosition(0, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            foreach (var point in points)
            {
                lr.SetPosition(1, point);
            }
        }
    }
}