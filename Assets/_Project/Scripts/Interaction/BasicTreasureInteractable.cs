using UnityEngine;

namespace TreasureDivers.Interaction
{
    [DisallowMultipleComponent]
    public sealed class BasicTreasureInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionPrompt = "Inspect treasure";

        public string InteractionPrompt => interactionPrompt;

        public void Interact(PlayerInteractor interactor)
        {
            Debug.Log($"{name} was interacted with. Treasure pickup comes in a later milestone.", this);
        }
    }
}
