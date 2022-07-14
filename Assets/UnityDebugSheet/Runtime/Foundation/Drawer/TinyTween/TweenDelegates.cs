namespace UnityDebugSheet.Runtime.Foundation.Drawer.TinyTween
{
    public delegate T LerpDelegate<T>(T a, T b, float t);

    public delegate float GetDeltaTimeDelegate();

    public delegate void CompletedDelegate();

    public delegate void ValueChangedDelegate<in T>(T value);
}
