﻿namespace Mango.Services.AuthAPI.Modals.Dto
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
    }
}
