using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ForumContext context) : base(context)
        {
        }
    }
}