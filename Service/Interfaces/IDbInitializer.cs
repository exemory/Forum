using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IDbInitializer
    {
        public Task InitializeAsync();
    }
}