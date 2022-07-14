using System.Collections;
#if PAGE_NAVIGATOR_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif

namespace UnityDebugSheet.Runtime.Foundation.PageNavigator
{
    public interface IPageLifecycleEvent
    {

#if PAGE_NAVIGATOR_USE_ASYNC_METHODS
        Task Initialize();
#else
        IEnumerator Initialize();
#endif

#if PAGE_NAVIGATOR_USE_ASYNC_METHODS
        Task WillPushEnter();
#else
        IEnumerator WillPushEnter();
#endif

        void DidPushEnter();

#if PAGE_NAVIGATOR_USE_ASYNC_METHODS
        Task WillPushExit();
#else
        IEnumerator WillPushExit();
#endif

        void DidPushExit();

#if PAGE_NAVIGATOR_USE_ASYNC_METHODS
        Task WillPopEnter();
#else
        IEnumerator WillPopEnter();
#endif

        void DidPopEnter();

#if PAGE_NAVIGATOR_USE_ASYNC_METHODS
        Task WillPopExit();
#else
        IEnumerator WillPopExit();
#endif

        void DidPopExit();

#if PAGE_NAVIGATOR_USE_ASYNC_METHODS
        Task Cleanup();
#else
        IEnumerator Cleanup();
#endif
    }
}
