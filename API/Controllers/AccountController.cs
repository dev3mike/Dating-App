using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _tokenService = tokenService;
            this.context = context;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            // Check Username Exists OR Not
            if (await isUserExist(registerDTO.Username)) return BadRequest("Username is Taken");

            var user = _mapper.Map<AppUser>(registerDTO);

            using var hmac = new HMACSHA512();


            user.UserName = registerDTO.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            user.PasswordSalt = hmac.Key;


            // Track Adding Proccess
            context.Users.Add(user);

            // Save to Database
            await context.SaveChangesAsync();

            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                FullName = user.FullName
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            // Check User in Database
            var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());
            if (user == null) return Unauthorized();

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized();
            }

            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                FullName = user.FullName
            };
        }

        private async Task<bool> isUserExist(string username)
        {
            return await context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}