using System.Threading.Tasks;

namespace UnityDebugSheet
{
    public interface IAsyncPoolableObject
    {
        Task OnBeforeUseAsync();
        Task OnBeforeReleaseAsync();
        Task OnBeforeClearAsync();
    }
}