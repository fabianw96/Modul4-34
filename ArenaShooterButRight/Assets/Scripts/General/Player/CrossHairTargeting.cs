using System;
using UnityEngine;

namespace General.Player
{
    public class CrossHairTargeting : MonoBehaviour
    {
        private Camera _mainCamera;
        private Ray _ray;
        private RaycastHit _hit;


        private void Start()
        {
            _mainCamera = Camera.main;
        }
    }
}
