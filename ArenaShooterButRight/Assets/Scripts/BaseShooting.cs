using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShooting : MonoBehaviour
{
    private static RaycastHit _raycastHit;

    public void Shoot()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Camera.main != null && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        
        Debug.Log("Shot!");
        
        if (!hit) return;
    }
}
