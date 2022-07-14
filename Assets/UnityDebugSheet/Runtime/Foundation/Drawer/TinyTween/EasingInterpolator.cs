namespace UnityDebugSheet.Runtime.Foundation.Drawer.TinyTween
{
    public sealed class EasingInterpolator : IInterpolator
    {
        public EaseType EaseType { get; set; }

        public float Interpolate(float progress)
        {
            return Easings.Interpolate(progress, EaseType);
        }
    }
}
