﻿using Meta.BusinessTier.Enums.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.User
{
    public class GetUsersResponse
    {
        public Guid Id { get; set; }

        public string? Username { get; set; }

        public RoleEnum? Role { get; set; }

        public string? FullName { get; set; }

        public string? Gender { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public UserStatus? Status { get; set; }

        public string? Email { get; set; }

        public int? Point { get; set; }

        public int? YearsOfExperience { get; set; }

        public string? Image { get; set; }

        public RankResponse? Rank { get; set; }

        //public CertificationResponse? Certification { get; set; }



    }
    public class RankResponse
    {
        public string? Name { get; set; }

        public int? Range { get; set; }
    }
}
