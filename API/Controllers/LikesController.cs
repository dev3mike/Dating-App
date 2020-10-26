using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly ILikesRepository likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            this.userRepository = userRepository;
            this.likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await likesRepository.GetUserWithLikes(sourceUserId);

            // Validation
            if (likedUser == null) return NotFound();
            if (sourceUser.UserName == username) return BadRequest("You Cannot Like Yourself !");

            var userLike = await likesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null) return BadRequest("You Already Liked This User");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to Like User");
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate)
        {
            var users = await likesRepository.GetUserLikes(predicate, User.GetUserId());
            return Ok(users);
        }
    }
}