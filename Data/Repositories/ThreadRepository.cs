using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ThreadRepository : Repository<Thread>, IThreadRepository
    {
        public ThreadRepository(ForumContext context) : base(context)
        {
        }
    }
}