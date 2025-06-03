using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Daraz101_Data;

namespace Daraz101_Services
{
    public class UserService : IUserProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<UserProfileDTO> GetProfile(string userId)
        {
            var user = await _userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new InvalidOperationException($"User with ID '{userId}' not found."); // Ensure non-null return

            var firstAddress = user.Addresses?.FirstOrDefault();

            return new UserProfileDTO
            {
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                FullAddress = firstAddress?.FullAddress ?? string.Empty // Ensure null check for FullAddress
            };
        }

        public async Task UpdateProfile(string userId, UserProfileDTO updatedProfile)
        {
            var user = await _userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) throw new InvalidOperationException($"User with ID '{userId}' not found."); // Ensure non-null user

            user.FullName = updatedProfile.FullName;
            user.Email = updatedProfile.Email;
            user.PhoneNumber = updatedProfile.PhoneNumber;

            var firstAddress = user.Addresses?.FirstOrDefault();

            if (firstAddress == null)
            {
                var newAddress = new Address
                {
                    FullAddress = updatedProfile.FullAddress ?? string.Empty, // Ensure non-null value
                    UserId = userId
                };

                user.Addresses = new List<Address> { newAddress };
                _context.Addresses.Add(newAddress);
            }
            else
            {
                firstAddress.FullAddress = updatedProfile.FullAddress ?? string.Empty; // Ensure non-null value
                _context.Addresses.Update(firstAddress);
            }

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
