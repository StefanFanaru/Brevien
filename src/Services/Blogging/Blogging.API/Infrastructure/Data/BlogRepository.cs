using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogging.API.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blogging.API.Infrastructure.Data
{
    public class BlogRepository : EfRepository<BloggingContext, Blog>, IBlogRepository
    {
        private readonly BloggingContext _context;

        public BlogRepository(BloggingContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<Blog>> GetByOwnerAsync(string id)
        {
            return _context.Blogs.Where(x => x.OwnerId == id && !x.SoftDeletedAt.HasValue).ToListAsync();
        }

        public Task<bool> ExistsAsync(string id)
        {
            return base.ExistsAsync(x => x.Id == id && !x.SoftDeletedAt.HasValue);
        }

        public Task<Blog> GetSoftDeleted(string id)
        {
            return _context.Blogs.Where(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}