using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext context;

        public LikesRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            var users = context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = context.Likes.AsQueryable();

            if (predicate == "liked")
            {

                likes = likes.Where(likes => likes.SourceUserId == userId);
                users = likes.Select(like => like.LikedUser);

            }
            else if (predicate == "likeBy")
            {

                likes = likes.Where(likes => likes.LikedUserId == userId);
                users = likes.Select(like => like.SourceUser);

            }

            return await users
                .Select(user => new LikeDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    FullName = user.FullName
                })
                .ToListAsync();
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}