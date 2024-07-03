using System;
using System.Collections;
using System.Collections.Generic;
using General.Interfaces;
using General.Player;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class Portal : MonoBehaviour, IInteractable
{
    [Header("Important")] 
    [SerializeField] private Portal linkedPortal;
    [SerializeField] private VisualEffect portalEffect;
    [SerializeField] private BoxCollider enableTrigger;

    private RenderTexture _portalTexture;
    private Camera _portalCam;
    private Camera _playerCam;

    private void Awake()
    {
        _playerCam = Camera.main;
        _portalCam = GetComponentInChildren<Camera>();
        _portalCam.enabled = false;
        portalEffect.Stop();

        CreateViewTexture();
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

            _portalTexture = new RenderTexture(Screen.width, Screen.height, 0);
            
            _portalCam.targetTexture = _portalTexture;
            
            linkedPortal.portalEffect.SetTexture("PortalTexture", _portalTexture);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<FPSController>()) return;
        
        linkedPortal._portalCam.enabled = true;
        portalEffect.enabled = true;
        portalEffect.Reinit();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<FPSController>()) return;
        
        linkedPortal._portalCam.enabled = false;
        portalEffect.enabled = false;
        portalEffect.Stop();
        linkedPortal.portalEffect.Stop();
    }

    public void Render()
    {
        var m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix *
                _playerCam.transform.localToWorldMatrix;
        _portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
    }

    public void Interaction()
    {
        //set player to portal pos + local Z + 1
        _playerCam.gameObject.transform.parent.SetPositionAndRotation(linkedPortal.transform.position, linkedPortal.transform.rotation);
        
    }
}
