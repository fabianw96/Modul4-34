using General.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabian.Generation
{
    public class EnableCAButton : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject caGameObject;
        [SerializeField] private GameObject generateButton;
        [SerializeField] private GameObject iterateButton;
        public void Interaction()
        {
            caGameObject.SetActive(!caGameObject.activeSelf);
            generateButton.SetActive(!generateButton.activeSelf);
            iterateButton.SetActive(!iterateButton.activeSelf);
        }
    }
}
