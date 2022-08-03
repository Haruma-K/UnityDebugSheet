namespace UnityDebugSheet.Runtime.Foundation.Gestures.InputAdapters
{
    public enum TouchPhase
    {
        None,

        /// <summary>
        ///     <para>A finger touched the screen.</para>
        /// </summary>
        Began,

        /// <summary>
        ///     <para>A finger moved on the screen.</para>
        /// </summary>
        Moved,

        /// <summary>
        ///     <para>A finger is touching the screen but hasn't moved.</para>
        /// </summary>
        Stationary,

        /// <summary>
        ///     <para>A finger was lifted from the screen. This is the final phase of a touch.</para>
        /// </summary>
        Ended,

        /// <summary>
        ///     <para>The system cancelled tracking for the touch.</para>
        /// </summary>
        Canceled
    }
}
