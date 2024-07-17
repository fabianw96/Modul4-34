using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;
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
    [SerializeField] private VisualEffect snowTrailVFX;
    [SerializeField] private VisualEffect hailVFX;
    [SerializeField] private Volume fogVolume;

    private float _prevRainIntensity;
    private float _prevSnowIntensity;
    private float _prevHailIntensity;
    private float _prevFogIntensity;
    private Fog _cachedFogComponent;
    
    private static readonly int Wetness = Shader.PropertyToID("_Wetness");
    private static readonly int SnowHeight = Shader.PropertyToID("_Snow_Height_Multi");
    private static readonly int SnowBlend = Shader.PropertyToID("_SnowBlend");


    // Start is called before the first frame update
    void Start()
    {
        rainVFX.SetFloat("Intensity", rainIntensity);
        snowVFX.SetFloat("Intensity", snowIntensity);
        hailVFX.SetFloat("Intensity", hailIntensity);
        Shader.SetGlobalFloat(Wetness, 0);
        Shader.SetGlobalFloat(SnowHeight, 0);
        Shader.SetGlobalFloat(SnowBlend, 0);

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
            
            if (snowIntensity >= 0.85f)
            {
                float culledSnowHeight = snowIntensity - 0.85f;
                Shader.SetGlobalFloat(SnowHeight, Mathf.Lerp(0,4, culledSnowHeight));
                snowTrailVFX.SetFloat("Rate", Mathf.Lerp(0, 100, culledSnowHeight));
            }
            
            Shader.SetGlobalFloat(SnowBlend, snowIntensity);
            snowVFX.SetFloat("Intensity", snowIntensity);
        }
        
        if (hailIntensity != _prevHailIntensity)
        {
            _prevHailIntensity = hailIntensity;
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
