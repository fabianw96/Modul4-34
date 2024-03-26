using General.Interfaces;
using UnityEngine;

namespace Fabian.Generation.Cellular_Automata
{
    public class IterateButton : MonoBehaviour, IInteractable
    {
        [SerializeField] private CellAutomata CellAutomata;
        public void Interaction()
        {
            CellAutomata.ApplyCellularAutomata(1);
        }
    }
}
