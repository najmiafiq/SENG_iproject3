using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


public interface ITokenService
{
    string CreateToken(ApplicationUser user);
}

