using System.Threading.Tasks;

namespace UnityDebugSheet.Runtime.Foundation.ObjectPooling
{
    public interface IAsyncPoolableObject
    {
        Task OnBeforeUseAsync();
        Task OnBeforeReleaseAsync();
        Task OnBeforeClearAsync();
    }
}