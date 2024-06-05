using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

public class WeatherManager : MonoBehaviour
{
    [Header("Weather Intensity")]
    [SerializeField, Range(0f, 1f)] private float rainIntensity;
    [SerializeField, Range(0f, 1f)] private float snowIntensity;
    [SerializeField, Range(0f, 1f)] private float hailIntensity;
    [SerializeField, Range(0f, 1f)] private float fogIntensity;

    [Header("Fog Values")]
    [SerializeField] private float minFogAttenDistance = 10f;
    [SerializeField] private float maxFogAttenDistance = 50f;

    [Header("VFX and Volumes")]
    [SerializeField] private VisualEffect rainVFX;
    [SerializeField] private VisualEffect snowVFX;
    [SerializeField] private VisualEffect hailVFX;
    [SerializeField] private Volume fogVolume;
    
    private float _prevRainIntensity;
    private float _prevSnowIntensity;
    private float _prevHailIntensity;
    private float _prevFogIntensity;
    private Fog _cachedFogComponent;
    private Vector3 _currentVelocity;
    
    private static readonly int Wetness = Shader.PropertyToID("_Wetness");


    // Start is called before the first frame update
    void Start()
    {
        rainVFX.SetFloat("Intensity", rainIntensity);
        snowVFX.SetFloat("Intensity", snowIntensity);
        hailVFX.SetFloat("Intensity", hailIntensity);
        fogVolume.weight = fogIntensity;

        fogVolume.profile.TryGet<Fog>(out _cachedFogComponent);

        if (_cachedFogComponent != null)
        {
            _cachedFogComponent.meanFreePath.Override(Mathf.Lerp(maxFogAttenDistance, minFogAttenDistance, fogIntensity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rainIntensity != _prevRainIntensity)
        {
            _prevRainIntensity = rainIntensity;
            Shader.SetGlobalFloat(Wetness, rainIntensity);
            rainVFX.SetFloat("Intensity", rainIntensity);
        }
        
        if (snowIntensity != _prevSnowIntensity)
        {
            _prevSnowIntensity = snowIntensity;
            snowVFX.SetFloat("Intensity", snowIntensity);
        }
        
        if (hailIntensity != _prevHailIntensity)
        {
            _prevHailIntensity = hailIntensity;
            Shader.SetGlobalFloat(Wetness, hailIntensity);
            hailVFX.SetFloat("Intensity", hailIntensity);
        }

        if (fogIntensity != _prevFogIntensity)
        {
            _prevFogIntensity = fogIntensity;
            fogVolume.weight = fogIntensity;
            
            if (_cachedFogComponent != null)
            {
                _cachedFogComponent.meanFreePath.Override(Mathf.Lerp(maxFogAttenDistance, minFogAttenDistance, fogIntensity));
            }
        }
    }
}
