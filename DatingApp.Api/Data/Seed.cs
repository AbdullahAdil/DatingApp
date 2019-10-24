using System.Collections.Generic;
using System.Linq;
using DatingApp.Api.Models;
using Newtonsoft.Json;

namespace DatingApp.Api.Data
{
    public static class Seed
    {
        public static void SeedUsers(DataContext dataContext)
        {
            if(!dataContext.Users.Any())
            {
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            byte[]passwordHash,passwordSalt;
            CreateUserPassword("password",out passwordHash,out passwordSalt);
            foreach(var user in users){
                user.PasswordSalt = passwordSalt;
                user.PasswordHash = passwordHash;
                user.Username = user.Username.ToLower();
                dataContext.Users.Add(user);
            }
            dataContext.SaveChanges();
            }
        }
        private static void CreateUserPassword(string password,out byte[]passwordHash,out byte[]passwordSalt)
        {
           using (var hash = new System.Security.Cryptography.HMACSHA512())
           {
               passwordSalt = hash.Key;
               passwordHash = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
           }
        }
    }
}