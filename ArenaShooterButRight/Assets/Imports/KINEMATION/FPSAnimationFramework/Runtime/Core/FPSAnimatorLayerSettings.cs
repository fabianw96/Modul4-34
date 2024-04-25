﻿// Designed by KINEMATION, 2024.

using System;
using System.Collections.Generic;
using KINEMATION.KAnimationCore.Runtime.Attributes;
using KINEMATION.KAnimationCore.Runtime.Rig;
using UnityEngine;

namespace KINEMATION.FPSAnimationFramework.Runtime.Core
{
    public enum ECurveBlendMode
    {
        Direct,
        Mask
    }

    public enum ECurveSource
    {
        Animator,
        Playables,
        Input
    }
    
    [Serializable]
    public struct CurveBlend
    {
        public string name;
        public ECurveBlendMode mode;
        [HideInInspector] public ECurveSource source;
    }
    
    public abstract class FPSAnimatorLayerSettings : ScriptableObject, IRigUser
    {
        [HideInInspector] public KRig rigAsset;
        [Range(0f, 1f)] public float alpha = 1f;
        [CurveSelector] public List<CurveBlend> curveBlending = new List<CurveBlend>();
        
        [Tooltip("Will call OnUpdateSettings on the layer state if true.")]
        public bool linkDynamically;

        public virtual FPSAnimatorLayerState CreateState() { return null; }
        
        public KRig GetRigAsset() { return rigAsset; }
        
        public virtual void OnRigUpdated()
        {
            // Update bone indices here.
        }
        
#if UNITY_EDITOR
        protected void UpdateRigElement(ref KRigElement element)
        {
            element = rigAsset.GetElementByName(element.name);
        }

        protected void OnValidate() { }
#endif
    }
}