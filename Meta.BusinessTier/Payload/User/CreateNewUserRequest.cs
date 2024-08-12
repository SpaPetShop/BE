using Meta.BusinessTier.Enums.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.User
{
    public class CreateNewUserRequest
    {
        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? FullName { get; set; }

        public string? Gender { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Image { get; set; }
    }
}
