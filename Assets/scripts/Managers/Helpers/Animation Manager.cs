using System;
using Spine;
using UnityEngine;
using Spine.Unity;

namespace AssemblyCSharp
{
    public class Animation_Manager : BasicManager
    {
        [SpineAnimation]
        SkeletonAnimation skeletonAnimation;
        public MeshRenderer meshRenderer {get; set;}
        public bool inAnimation {get; set;}

        public Animation_Manager()
        {
            meshRenderer = transform.Find("Animations").GetComponent<MeshRenderer>();
        }

        public void SetSortingLayer(int sortingLayer ){
            meshRenderer.sortingOrder = sortingLayer;
        }
    }
}

