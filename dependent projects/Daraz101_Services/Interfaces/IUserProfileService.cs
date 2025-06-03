namespace Daraz101_Services
{
        public interface IUserProfileService
        {
            Task<UserProfileDTO> GetProfile(string userId);
            Task UpdateProfile(string userId, UserProfileDTO updatedProfile);
        }

}
