﻿using Meta.BusinessTier.Enums.EnumTypes;
using Meta.BusinessTier.Enums.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.User
{
    public class UpdateUserInforRequest
    {

        public RoleEnum? Role { get; set; }

        public string? FullName { get; set; }

        public AccountTypeGender? Gender { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public UserStatus? Status { get; set; }

        public string? Email { get; set; }

        public string? Image { get; set; }

        //public int? Rank { get; set; }

        public int? YearsOfExperience { get; set; }
        //public string? FcmToken { get; set; }


        public UpdateUserInforRequest()
        {
        }
    }

}
