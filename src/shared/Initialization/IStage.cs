using System.Threading.Tasks;

namespace Shared.Initialization;

public interface IStage
{
    int Order { get; }
    Task ExecuteAsync();
}