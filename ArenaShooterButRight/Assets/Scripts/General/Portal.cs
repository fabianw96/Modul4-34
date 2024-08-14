using System.Collections.Generic;
using General.Interfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

namespace General
{
    public class Portal : MonoBehaviour, IInteractable
    {
        [Header("Important")] 
        [SerializeField] private Portal linkedPortal;
        [SerializeField] private VisualEffect portalEffect;
        [SerializeField] private bool loadNewScene;
        [SerializeField] private SceneAsset sceneToLoad;

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
            if (!linkedPortal)
                return;

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

            if (linkedPortal!= null)
            {
                linkedPortal._portalCam.enabled = true;
            }
            
            portalEffect.enabled = true;
            portalEffect.Reinit();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.GetComponent<FPSController>()) return;


            portalEffect.enabled = false;
            portalEffect.Stop();

            if (linkedPortal == null) return;
            
            linkedPortal._portalCam.enabled = false;
            linkedPortal.portalEffect.Stop();
        }

        private void Render()
        {
            if (!linkedPortal)
                return;

            Vector3 pos = linkedPortal.transform.InverseTransformPoint(_playerCam.transform.position);
            float angle = SignedAngle(-linkedPortal.transform.forward, _playerCam.transform.forward, Vector3.up);
        
            _portalCam.transform.SetLocalPositionAndRotation(new Vector3(-pos.x, _portalCam.transform.localPosition.y, -pos.z), Quaternion.Euler(0f,angle,0f));
        }

        public void Interaction()
        {
            //set player to portal pos + local Z + 1
            if (loadNewScene)
            {
                SceneManager.LoadScene(sceneToLoad.name);
            }
            else
            {
                _playerCam.gameObject.transform.parent.SetPositionAndRotation(linkedPortal.transform.position + Vector3.forward, linkedPortal.transform.rotation);
            }
        }
    
        private float SignedAngle(Vector3 a, Vector3 b, Vector3 n) {
            // angle in [0,180]
            float angle = Vector3.Angle(a,b);
            float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a, b)));
		
            // angle in [-179,180]
            float signedAngle = angle * sign;
		
            while(signedAngle < 0) signedAngle += 360;
	
            return signedAngle;
        }
    }
}
