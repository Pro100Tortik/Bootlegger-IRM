namespace Bootlegger
{
    public class PauseRequest
    {
        #region Custom
        public bool IsParry;
        public bool IsVictory;
        public bool IsDefeat;
        #endregion

        public bool IsFullPause;

        #region MonoBehaviour
        public bool IsSceneChanged;
        #endregion

        public Priority Priority;
    }
}
