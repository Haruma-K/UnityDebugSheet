namespace UnityDebugSheet
{
    public interface IPoolableObject
    {
        void OnBeforeUse();
        void OnBeforeRelease();
        void OnBeforeClear();
    }
}