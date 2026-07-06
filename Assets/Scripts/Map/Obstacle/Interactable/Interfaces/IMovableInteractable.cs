public interface IMovableInteractable : IInteractable
{
    void HandleInput(float horizontal, float vertical);

    float GetAnimationSpeed(float horizontal, float vertical);
}
