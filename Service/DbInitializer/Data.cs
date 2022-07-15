using System;
using Data.Entities;

namespace Service.DbInitializer
{
    /// <summary>
    /// Static class that holds initial data to be seeded
    /// </summary>
    public static class Data
    {
        public const string AdminPassword = "adminpass";

        public static readonly string[] Roles = {"User", "Moderator", "Administrator"};

        public static readonly User AdminUser = new User
        {
            Id = new Guid("4a9614a9-a9c0-4571-b915-08cdd1dffdc9"),
            UserName = "admin",
            Email = "admin@example.com",
            Name = "Ivan Vishnevsky"
        };
    }
}