using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PluginInterface
{
    public interface IPlugin
    {
        Task Initialize(CancellationToken cancellationToken);
        void SetCryptData(string method, byte[] key, byte[] iv);
        Stream HandleData(Stream stream, CancellationToken cancellationToken);
    }
}
