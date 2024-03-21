using UnityEngine;

namespace General.Interfaces
{
    public interface IInteractable
    {
        void Interaction()
        {
            Debug.Log(this);
        }
    }
}