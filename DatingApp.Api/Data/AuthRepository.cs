using System;
using System.Threading.Tasks;
using DatingApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string userName,string password){
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Username == userName);
            if(ReferenceEquals(user,null))
            return null;
            if(!VarifyPasswordHash(password,user.PasswordHash,user.PasswordSalt))
            return null;
            return user;
        }

        private bool VarifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
             using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(var i=0;i<computedHash.Length;i++){
                    if(computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Registor(User user,string password){
            byte[]passwordHash,passwordSalt;
            CreatePasswordHash(password,out passwordHash,out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<bool> UserExists(string userName){
            if(await _context.Users.AnyAsync(x=>x.Username == userName))
            return true;
            return false;
        }
        private void CreatePasswordHash(string password,out byte[]passwordHash,out byte[]passwordSalt){
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}