using FribergCarRental.Core.Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRental.DAL.Data
{
    public class ApplicationUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationUserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return result;

            var roleExists = await _roleManager.RoleExistsAsync("Customer");
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("Customer"));
                if (!roleResult.Succeeded)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Could not create 'Customer' role." });
                }
            }

            var roleAddResult = await _userManager.AddToRoleAsync(user, "Customer");
            if (!roleAddResult.Succeeded)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Could not add user to 'Customer' role." });
            }

            return result;
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser? user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}
