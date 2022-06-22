using System.Threading.Tasks;
using Data.Interfaces;
using Data.Repositories;

namespace Data
{
    /// <inheritdoc />
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ForumContext _context;

        private IPostRepository _postRepository;
        private IThreadRepository _threadRepository;
        private IUserRepository _userRepository;

        /// <summary>
        /// Constructor for initializing a <see cref="UnitOfWork"/> class instance
        /// </summary>
        /// <param name="context">Context of the database</param>
        public UnitOfWork(ForumContext context)
        {
            _context = context;
        }

        public IPostRepository PostRepository => _postRepository ??= new PostRepository(_context);
        public IThreadRepository ThreadRepository => _threadRepository ??= new ThreadRepository(_context);
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}