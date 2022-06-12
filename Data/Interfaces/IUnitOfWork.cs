using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IUnitOfWork
    {
        public IPostRepository PostRepository { get; }
        public IThreadRepository ThreadRepository { get; }
        public IUserRepository UserRepository { get; }
        
        public Task SaveAsync();
    }
}