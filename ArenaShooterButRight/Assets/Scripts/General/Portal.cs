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
        Vector3 pos = linkedPortal.transform.InverseTransformPoint(_playerCam.transform.position);
        float angle = SignedAngle(-linkedPortal.transform.forward, _playerCam.transform.forward, Vector3.up);
        
        _portalCam.transform.SetLocalPositionAndRotation(new Vector3(-pos.x, _portalCam.transform.localPosition.y, -pos.z), Quaternion.Euler(0f,angle,0f));
        
    }

    public void Interaction()
    {
        //set player to portal pos + local Z + 1
        _playerCam.gameObject.transform.parent.SetPositionAndRotation(linkedPortal.transform.position + Vector3.forward, linkedPortal.transform.rotation);
        
    }
    
    float SignedAngle(Vector3 a, Vector3 b, Vector3 n) {
        // angle in [0,180]
        float angle = Vector3.Angle(a,b);
        float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a, b)));
		
        // angle in [-179,180]
        float signed_angle = angle * sign;
		
        while(signed_angle < 0) signed_angle += 360;
	
        return signed_angle;
    }
}
