namespace UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules.Animation
{
    public interface IAnimation
    {
        float Duration { get; }

        void SetTime(float time);
    }
}