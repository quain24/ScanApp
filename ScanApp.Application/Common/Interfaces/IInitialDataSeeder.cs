using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IInitialDataSeeder
    {
        Task Initialize(bool force);
    }
}