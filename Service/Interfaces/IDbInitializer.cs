using System.Threading.Tasks;

namespace Service.Interfaces
{
    /// <summary>
    /// Service for initialization and seeding database at startup
    /// </summary>
    public interface IDbInitializer
    {
        /// <summary>
        /// Initializes and seeds database
        /// </summary>
        public Task InitializeAsync();
    }
}