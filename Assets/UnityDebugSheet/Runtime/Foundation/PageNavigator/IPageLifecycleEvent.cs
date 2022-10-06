using System.Collections;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif

namespace UnityDebugSheet.Runtime.Foundation.PageNavigator
{
    public interface IPageLifecycleEvent
    {

#if UDS_USE_ASYNC_METHODS
        Task Initialize();
#else
        IEnumerator Initialize();
#endif

#if UDS_USE_ASYNC_METHODS
        Task WillPushEnter();
#else
        IEnumerator WillPushEnter();
#endif

        void DidPushEnter();

#if UDS_USE_ASYNC_METHODS
        Task WillPushExit();
#else
        IEnumerator WillPushExit();
#endif

        void DidPushExit();

#if UDS_USE_ASYNC_METHODS
        Task WillPopEnter();
#else
        IEnumerator WillPopEnter();
#endif

        void DidPopEnter();

#if UDS_USE_ASYNC_METHODS
        Task WillPopExit();
#else
        IEnumerator WillPopExit();
#endif

        void DidPopExit();

#if UDS_USE_ASYNC_METHODS
        Task Cleanup();
#else
        IEnumerator Cleanup();
#endif
    }
}
