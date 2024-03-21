using General.Interfaces;
using UnityEngine;

namespace General.Player
{
    public static class PlayerInteraction
    {
        private static RaycastHit _raycastHit;
        public static readonly float RaycastDistance = 1.5f;

        
        public static void Interact()
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Camera.main != null && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, RaycastDistance);

            if (!hit) return;
            GameObject hitObject = hitInfo.transform.gameObject;
            if (hitObject != null && hitObject.GetComponent<IInteractable>() != null)
            {
                hitObject.GetComponent<IInteractable>().Interaction();
            }
        }
    }
}
