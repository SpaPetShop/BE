using AutoMapper;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.Pet;
using Meta.BusinessTier.Payload.Task;
using Meta.BusinessTier.Services.Interfaces;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Models;
using Meta.DataTier.Paginate;
using Meta.DataTier.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Meta.BusinessTier.Services.Implements
{
    public class PetService : BaseService<PetService>, IPetService
    {
        public PetService(IUnitOfWork<SpaPetContext> unitOfWork, ILogger<PetService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<Guid> CreateNewPets(CreateNewPetRequest createNewPetRequest)
        {
            var currentUser = GetUsernameFromJwt();
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Username.Equals(currentUser));
            DateTime currentTime = TimeUtils.GetCurrentSEATime();

            var petType = await _unitOfWork.GetRepository<TypePet>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(createNewPetRequest.TypePetId));
            if (petType == null)
            {
                throw new BadHttpRequestException(MessageConstant.Category.NotFoundFailedMessage);
            }

            Pet newPet = new Pet
            {
                Id = Guid.NewGuid(),
                Name = createNewPetRequest.Name,
                Age = createNewPetRequest.Age,
                Weight = createNewPetRequest.Weight,
                Image = createNewPetRequest.Image,
                TypePetId = createNewPetRequest.TypePetId,
                AccountId = account.Id,
            };

            await _unitOfWork.GetRepository<Pet>().InsertAsync(newPet);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccess)
            {
                throw new BadHttpRequestException(MessageConstant.Pet.CreateNewFailedMessage);
            }
            return newPet.Id;
        }

        public async Task<GetPetResponse> GetPetById(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadHttpRequestException(MessageConstant.Pet.EmptyPetIdMessage);

            var pet = await _unitOfWork.GetRepository<Pet>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id),
                include: x => x.Include(p => p.TypePet)
                                .Include(p => p.Account))
            ?? throw new BadHttpRequestException(MessageConstant.Pet.NotFoundMessage);

            var response = new GetPetResponse
            {
                Id = pet.Id,
                Name = pet.Name,
                Age = pet.Age,
                Weight = pet.Weight,
                Image = pet.Image,
                TypePet = new TypePetResponse 
                { 
                    Id = pet.TypePet.Id,
                    Name = pet.TypePet.Name 
                },
                Customer =  new AccountResponse
                { 
                    Id = pet.Account.Id,
                    FullName = pet.Account.FullName,
                    Role = EnumUtil.ParseEnum<RoleEnum>(pet.Account.Role),
                }
            };

            return response;
        }

        public async Task<IPaginate<GetPetResponse>> GetPetList(PetFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetPetResponse> petList = await _unitOfWork.GetRepository<Pet>().GetPagingListAsync(
                selector: x => new GetPetResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Age = x.Age,
                    Weight = x.Weight,
                    Image = x.Image,
                    TypePet = new TypePetResponse
                    {
                        Id = x.TypePet.Id,
                        Name = x.TypePet.Name
                    },
                    Customer = new AccountResponse
                    {
                        Id = x.Account.Id,
                        FullName = x.Account.FullName,
                        Role = EnumUtil.ParseEnum<RoleEnum>(x.Account.Role),
                    }
                },
                filter: filter,
                orderBy: x => x.OrderByDescending(x => x.Id), // Assuming Id is used to sort, modify as needed
                include: x => x.Include(p => p.TypePet).Include(p => p.Account),
                page: pagingModel.page,
                size: pagingModel.size
            ) ?? throw new BadHttpRequestException(MessageConstant.Pet.NotFoundMessage);

            return petList;
        }




        public async Task<bool> UpdatePet(Guid id, UpdatePetRequest updatePetRequest)
        {
            if (id == Guid.Empty)
                throw new BadHttpRequestException(MessageConstant.Pet.EmptyPetIdMessage);

            Pet pet = await _unitOfWork.GetRepository<Pet>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
            ?? throw new BadHttpRequestException(MessageConstant.Pet.NotFoundMessage);

            // Validate and retrieve the TypePet if needed
            if (updatePetRequest.TypePetId != Guid.Empty)
            {
                TypePet typePet = await _unitOfWork.GetRepository<TypePet>().SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(updatePetRequest.TypePetId))
                ?? throw new BadHttpRequestException(MessageConstant.TypePet.NotFoundFailedMessage);

                pet.TypePet = typePet;
            }

            // Update fields
            pet.Name = string.IsNullOrEmpty(updatePetRequest.Name) ? pet.Name : updatePetRequest.Name;
            pet.Age = updatePetRequest.Age.HasValue ? updatePetRequest.Age.Value : pet.Age;
            pet.Weight = updatePetRequest.Weight.HasValue ? updatePetRequest.Weight.Value : pet.Weight;
            pet.Image = string.IsNullOrEmpty(updatePetRequest.Image) ? pet.Image : updatePetRequest.Image;

            // Update the AccountId if provided
            if (updatePetRequest.CustomerId != null)
            {
                Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(updatePetRequest.CustomerId.Value))
                ?? throw new BadHttpRequestException(MessageConstant.Account.NotFoundFailedMessage);

                pet.Account = account;
            }

            // Mark the pet as updated
            _unitOfWork.GetRepository<Pet>().UpdateAsync(pet);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;
            return isSuccess;
        }
    }
}
