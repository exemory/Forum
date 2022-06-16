﻿using System;
using Data.Entities;

namespace Service.DbInitializer
{
    public static class Data
    {
        public static readonly string[] Roles = {"User", "Moderator", "Administrator"};

        public static readonly User AdminUser = new User
        {
            Id = new Guid("4a9614a9-a9c0-4571-b915-08cdd1dffdc9"),
            UserName = "admin",
            Email = "ivanmail@gmail.com",
            Name = "Ivan",
            RegistrationDate = new DateTime(2005, 7, 2)
        };

        public const string AdminPassword = "adminpass";
    }
}