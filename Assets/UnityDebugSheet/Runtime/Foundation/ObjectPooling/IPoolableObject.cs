namespace UnityDebugSheet.Runtime.Foundation.ObjectPooling
{
    public interface IPoolableObject
    {
        void OnBeforeUse();
        void OnBeforeRelease();
        void OnBeforeClear();
    }
}