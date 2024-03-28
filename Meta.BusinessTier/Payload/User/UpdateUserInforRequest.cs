﻿using Meta.BusinessTier.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.User
{
    public class UpdateUserInforRequest
    {
        public string Password { get; set; }
        public UserStatus Status { get; set; }
        

        public UpdateUserInforRequest()
        {
        }
    }

}
