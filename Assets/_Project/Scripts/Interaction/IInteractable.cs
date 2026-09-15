namespace TreasureDivers.Interaction
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }

        void Interact(PlayerInteractor interactor);
    }
}
