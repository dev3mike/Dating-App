using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var users = await userRepository.GetMembersAsync(userParams);

            // Add Pagination Header to Response
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            // System.Console.WriteLine(User);
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await userRepository.GetMemberAsync(username);
            return Ok(user);
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<string>> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userRepository.GetUserByUsernameAsync(username);

            mapper.Map(memberUpdateDto, user);

            userRepository.Update(user);

            if (await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Update Failed");
        }
    }
}