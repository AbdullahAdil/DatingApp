using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.Api.Data;
using DatingApp.Api.Dtos;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repository,IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.username = userForRegisterDto.username.ToLower();
            if(await _repository.UserExists(userForRegisterDto.username))
            return BadRequest("Username already exists");
            var userToCreate = new User{
                Username = userForRegisterDto.username
            };
            var createdUser = _repository.Registor(userToCreate,userForRegisterDto.password);
            return StatusCode(201);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto){
            var user = await _repository.Login(userForLoginDto.username.ToLower(),userForLoginDto.password);
            if(ReferenceEquals(user,null))
            return Unauthorized();
            var claims = new []{
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Username)
            };
             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:token").Value));
             var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
             var tokenDescriptor = new SecurityTokenDescriptor
             {
               Subject = new ClaimsIdentity(claims),
               Expires = DateTime.Now.AddDays(1),
               SigningCredentials = creds
             };
             var tokenHandler = new JwtSecurityTokenHandler();
             var token = tokenHandler.CreateToken(tokenDescriptor);
             return Ok(new{token = tokenHandler.WriteToken(token)});
        }
    }
}