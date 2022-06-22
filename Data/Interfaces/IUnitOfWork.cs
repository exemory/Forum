using System.Threading.Tasks;

namespace Data.Interfaces
{
    /// <summary>
    /// Unit of work
    /// </summary>
    public interface IUnitOfWork
    {
        public IPostRepository PostRepository { get; }
        public IThreadRepository ThreadRepository { get; }
        public IUserRepository UserRepository { get; }
        
        /// <summary>
        /// Saves all changes made through the repositories in the context to the database
        /// </summary>
        public Task SaveAsync();
    }
}