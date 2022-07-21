using System.Threading.Tasks;

namespace Service.Interfaces
{
    /// <summary>
    /// Service for initialization and seeding database
    /// </summary>
    public interface IDbInitializer
    {
        /// <summary>
        /// Initializes and seeds database
        /// </summary>
        /// <param name="seedTestData">
        /// <c>true</c> if test data should be seeded, <c>false</c> otherwise
        /// </param>
        public Task InitializeAsync(bool seedTestData);
    }
}