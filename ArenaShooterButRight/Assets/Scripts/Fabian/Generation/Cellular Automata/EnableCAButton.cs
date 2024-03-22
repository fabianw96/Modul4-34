using General.Interfaces;
using UnityEngine;

namespace Fabian.Generation.Cellular_Automata
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
