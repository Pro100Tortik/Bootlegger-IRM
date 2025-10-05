using UnityEngine;

namespace Bootlegger
{
    public interface IInteractor
    {
        // Some refrences for interactions, like hotbar and wallet
        void MinigameStarted(Vector3 position, Quaternion rotation);
        void MinigameStopped(Vector3 position, Quaternion rotation);
    }
}
