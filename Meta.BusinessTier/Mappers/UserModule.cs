using AutoMapper;
using Meta.BusinessTier.Payload.User;
using Meta.DataTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Mappers
{
    public class UserModule : Profile
    {
        public UserModule()
        {
            CreateMap<User, GetUsersResponse>();
            CreateMap<CreateNewUserRequest, User>();
        }
    }
}
