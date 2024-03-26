using General.Interfaces;
using UnityEngine;

namespace Fabian.Generation.Cellular_Automata
{
    public class GenerateButton : MonoBehaviour, IInteractable
    {
        [SerializeField] private CellAutomata CellAutomata;
        public void Interaction()
        {
            CellAutomata.GenerateNoise();
        }
    }
}
