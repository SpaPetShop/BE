using Microsoft.AspNetCore.Authorization;
using Meta.BusinessTier.Utils;
using Meta.BusinessTier.Enums.Status;

namespace HiCamping.BusinessTier.Validators;

public class CustomAuthorizeAttribute : AuthorizeAttribute
{
	public CustomAuthorizeAttribute(params RoleEnum[] roleEnums)
	{
		var allowedRolesAsString = roleEnums.Select(x => x.GetDescriptionFromEnum());
		Roles = string.Join(",", allowedRolesAsString);
	}
}