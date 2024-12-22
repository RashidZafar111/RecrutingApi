using RecrutingApi.DataAccessLayer;
using RecrutingApi.DBContext;
using RecrutingApi.Model;

namespace RecrutingApi.Authentication
{
    public class UserService
    {
        private readonly RecrutingApiDBContext _recrutingApiDBContext;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserDAL _userDAL;

        /// <summary>
        /// Initilizing the user service constructor
        /// </summary>
        /// <param name="recrutingApiDBContext"></param>
        /// <param name="httpContext"></param>
        /// <param name="userDAL"></param>
        public UserService(RecrutingApiDBContext recrutingApiDBContext, IHttpContextAccessor httpContext, UserDAL userDAL)
        {
            _recrutingApiDBContext = recrutingApiDBContext;
            _httpContext = httpContext;
            _userDAL = userDAL;
        }

        /// <summary>
        /// Getting the user role based on authentication key prvided after login
        /// </summary>
        /// <param name="UserKey"></param>
        /// <returns>Users</returns>
        public async Task<Users> UserRoles(string UserKey)
        {
            try
            {
                Users users = new Users();
                var userDetails = _recrutingApiDBContext.users.Where(x => x.uAuthKey == UserKey).FirstOrDefault();
                if (userDetails != null)
                {
                    users = _recrutingApiDBContext.users.Where(x => x.Id == userDetails.Id).FirstOrDefault();

                    _httpContext.HttpContext.Session.SetString("UserId", users.uId.ToString());

                    return users;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}