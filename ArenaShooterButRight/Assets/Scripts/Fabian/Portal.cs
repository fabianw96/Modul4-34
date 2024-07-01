using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class Portal : MonoBehaviour
{
    [Header("Important")] 
    [SerializeField] private Portal linkedPortal;
    [SerializeField] private VisualEffect portalEffect;

    private RenderTexture _portalTexture;
    private Camera portalCam;
    private Camera playerCam;

    private void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
    }

    private void LateUpdate()
    {
        Render();
    }

    private void CreateViewTexture()
    {
        if (_portalTexture == null || _portalTexture.width != Screen.width || _portalTexture.height != Screen.height)
        {
            if (_portalTexture != null)
            {
                _portalTexture.Release();
            }

            _portalTexture = new RenderTexture(Screen.width, Screen.height, 0)
            {
                name = this.name + "PortalTex"
            };

            portalCam.targetTexture = _portalTexture;
            
            portalEffect.SetTexture("PortalTexture", _portalTexture);
        }
    }

    public void Render()
    {
        portalCam.enabled = true;
        
        CreateViewTexture();

        var m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix *
                playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
        
        portalCam.Render();
    }
    
}
