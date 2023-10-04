using System;
namespace MobileAPI.Models
{
	public class MemberRegister
	{

        public required string MemberName { get; set; }

        public required string Password { get; set; }

        public required string MobileNumber { get; set; }

        public required string Email { get; set; }
    }
}

