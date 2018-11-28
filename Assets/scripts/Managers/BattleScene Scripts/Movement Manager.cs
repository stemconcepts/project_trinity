using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Movement_Manager
    {
        public int origSortingOrder { get; set; }
        public Movement_Manager()
        {
        }

        public void SetSortingLayer(int sortingLayer ){
            origSortingOrder = sortingLayer;
        }
    }
}

