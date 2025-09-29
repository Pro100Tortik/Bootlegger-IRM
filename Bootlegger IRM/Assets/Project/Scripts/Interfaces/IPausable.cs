namespace Bootlegger
{
    public interface IPausable
    {
        bool IsPaused { get; }

        void Pause(in PauseRequest pauseRequest);
        void Resume(in PauseRequest pauseRequest);
    }
}
