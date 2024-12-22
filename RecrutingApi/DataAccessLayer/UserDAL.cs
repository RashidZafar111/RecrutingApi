using RecrutingApi.DBContext;
using RecrutingApi.Helper;
using RecrutingApi.Model;
using static RecrutingApi.Model.Role;

namespace RecrutingApi.DataAccessLayer
{
    public class UserDAL
    {
        private readonly RecrutingApiDBContext _recrutingApiDBContext;

        private readonly Helpers _helpers;

        public UserDAL(RecrutingApiDBContext recrutingApiDBContext, Helpers helpers)

        {
            _recrutingApiDBContext = recrutingApiDBContext;
            _helpers = helpers;
        }

        /// <summary>
        /// </summary>
        /// <param name="authentciate"></param>
        /// <returns></returns>
        public async Task<ResponseResult> ValidateUser(Authentciate authentciate)
        {
            try
            {
                Authentciate authenticateUser = new Authentciate();
                Users users = new Users();
                users = _recrutingApiDBContext.users.
                   Where(x => x.emailAddress == authentciate.emailAddress &&
                   x.password == authentciate.password).SingleOrDefault();
                if (users != null)
                {
                    if (DateTime.Now > users.keyExpireTime)
                    {
                        users.keyExpireTime = DateTime.Now.AddDays(5);
                        users.uAuthKey = Guid.NewGuid().ToString();
                        _recrutingApiDBContext.SaveChanges();
                    }
                    else
                    {
                        users.keyExpireTime = DateTime.Now.AddDays(5);
                        users.uAuthKey = Guid.NewGuid().ToString();
                        _recrutingApiDBContext.SaveChanges();
                    }
                    return _helpers.bindResponseData("Authentication Successful", $"SessionAuthKey : {users.uAuthKey}", true);
                }
                else
                {
                    return _helpers.bindResponseData("Authentication Failed", "", true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Users> GetUserDetails(string sessionKey)
        {
            Users users = new Users();
            var userDetails = _recrutingApiDBContext.users.Where(x => x.uAuthKey == sessionKey).FirstOrDefault();
            if (userDetails != null)
            {
                users = _recrutingApiDBContext.users.Where(x => x.Id == userDetails.Id).FirstOrDefault();
                return users;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Users>> getAllUsers()
        {
            List<Users> users = new List<Users>();
            users = _recrutingApiDBContext.users.ToList();
            return users;
        }

        /// <summary>
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="roles"></param>
        /// <returns></returns>

        public async Task<Users> CreateUser(string emailAddress, role roles, int uId)
        {
            try
            {
                Users user = new Users();

                user.emailAddress = emailAddress;
                user.password = _helpers.CreatePassword();
                user.Id = _helpers.GetNewUserId();
                user.roles = roles;
                user.uId = uId;
                await _recrutingApiDBContext.users.AddAsync(user);
                await _recrutingApiDBContext.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Users> DeleteUser(string emailAddress, role role)
        {
            try
            {
                Users users = new Users();
                users = _recrutingApiDBContext.users.Where(x => x.emailAddress == emailAddress && x.roles == role).FirstOrDefault();
                _recrutingApiDBContext.users.Remove(users);
                await _recrutingApiDBContext.SaveChangesAsync();
                return users;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<bool> UsersExists(string emailAddress, string role)
        {
            try
            {
                return _recrutingApiDBContext.users.Any(e => e.emailAddress == emailAddress && e.roles.ToString() == role);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}