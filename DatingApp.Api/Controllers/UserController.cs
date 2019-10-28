using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
//using DatingApp.Api.Models;
using AutoMapper;
using DatingApp.Api.Dtos;
using System.Security.Claims;
//using System.Security.Claims;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;
        public UserController(IDatingRepository repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetUser(){
            var users = await _repository.GetUsers();
            var userToReturn = _mapper.Map<IEnumerable<UserForList>>(users);
            return Ok(userToReturn);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id){
            var user = await _repository.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailed>(user);
            return Ok(userToReturn);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,UserForUpdateDto userForUpdateDto){
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var userFromRepo =await _repository.GetUser(id);
            _mapper.Map(userForUpdateDto,userFromRepo);
            if(await _repository.SaveAll()){
                return NoContent();
            }
            throw new Exception($"Updating user {id} failed on save");
        }
    }
}
