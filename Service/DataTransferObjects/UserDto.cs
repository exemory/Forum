﻿using System;

namespace Service.DataTransferObjects
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}