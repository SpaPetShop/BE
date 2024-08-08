﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.User
{
    public class CreateNewStaffRequest
    {
        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? FullName { get; set; }

        public string? Gender { get; set; }

        public string? PhoneNumber { get; set; }
        
        public string? Role { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public string? Image { get; set; }

        public int? YearsOfExperience { get; set; }

        //public List<CertificationRequest> Cetification { get; set; } = new List<CertificationRequest>();
    }
    public class CertificationRequest
    {
        public string? CertificationLink { get; set; }

        public DateTime? DateObtained { get; set; }

        public Guid? AccountId { get; set; }
    }
}
