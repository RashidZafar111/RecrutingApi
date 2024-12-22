using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace RecrutingApi.Authentication;

public class ClaimsTransformationService : IClaimsTransformation
{
    /// <summary>
    /// private readonly UserService userService;
    /// </summary>
    /// <param name="userService"></param>
    private readonly UserService userService;

    public ClaimsTransformationService(UserService userService)
    {
        this.userService = userService;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        try
        {
            if (principal.Identity?.IsAuthenticated != true)
            {
                return principal;
            }

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var roles = await userService.UserRoles(userId);

            if (roles == null)
            {
                return principal;
            }
            string role = roles.roles.ToString();

            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim(ClaimTypes.Role, role));
        }
        catch (Exception ex)
        {
        }
        return principal;
    }
}