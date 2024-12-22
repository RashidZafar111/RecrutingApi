using NuGet.Protocol;
using RecrutingApi.DBContext;
using RecrutingApi.Helper;
using RecrutingApi.Model;
using static RecrutingApi.Model.Role;

namespace RecrutingApi.DataAccessLayer
{
    /// <summary>
    /// Recruiter DAL helps to sepecrate the logic to retrieve data from the rest of the
    /// applicaiton. It helps to implement abstraction and encapsulates all data source operations.
    /// The DAL ensures that controller is clean and if any changes required in furutre, we need to
    /// change at one place only, to reflect the changes.
    /// </summary>
    public class RecruiterDAL
    {
        private readonly RecrutingApiDBContext _recrutingApiDBContext;
        private Helpers _helpers;
        private UserDAL _userDAL;

        /// <summary>
        /// constructor initialization to pass the Database instance
        /// </summary>
        /// <param name="recrutingApiDBContext"></param>
        public RecruiterDAL(RecrutingApiDBContext recrutingApiDBContext, Helpers helpers, UserDAL userDAL)
        {
            _recrutingApiDBContext = recrutingApiDBContext;
            _helpers = helpers;
            _userDAL = userDAL;
        }

        /// <summary>
        /// insert the new recruiter in the database with increment id if exists
        /// </summary>
        /// <param name="recruiter"></param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> CreateRecruiter(Recruiter recruiter)
        {
            try
            {
                Recruiter _recruiter = new Recruiter();
                ResponseResult responseResult = new ResponseResult();

                _recruiter = _recrutingApiDBContext.recruiters.Where(x => x.emailAddress == recruiter.emailAddress).FirstOrDefault();
                if (_recruiter == null)
                {
                    Users user = new Users();
                    recruiter.Id = _helpers.GetNewRecruiterId();
                    recruiter.recordCreateDateTime = DateTime.Now;
                    await _recrutingApiDBContext.recruiters.AddAsync(recruiter);
                    await _recrutingApiDBContext.SaveChangesAsync();
                    // creating user in user table to apply normilization seperaiton of logic
                    user = await _userDAL.CreateUser(recruiter.emailAddress, role.Recruiter, recruiter.Id);

                    return _helpers.bindResponseData("Recruiter Successfully Created", $"UserName : {user.emailAddress}, Password = {user.password}", true);
                }
                else
                {
                    return _helpers.bindResponseData("Candidate Email Address Already Exists", "", true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Update recruiter data
        /// </summary>
        /// <param name="recruiterId"></param>
        /// <param name="recruiter"></param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UpdateRecruiter(int recruiterId, Recruiter recruiter)
        {
            try
            {
                ResponseResult responseResult = new ResponseResult();

                Recruiter _recruiter = new Recruiter();
                _recruiter = _recrutingApiDBContext.recruiters.Where(x => x.Id == recruiterId).FirstOrDefault();
                if (_recruiter != null)
                {
                    _recruiter.firstName = recruiter.firstName;
                    _recruiter.middleName = recruiter.middleName;
                    _recruiter.lastName = recruiter.lastName;
                    _recruiter.mobileNumber = recruiter.mobileNumber;
                    _recruiter.title = recruiter.title;
                    _recruiter.city = recruiter.city;
                    _recruiter.country = recruiter.country;
                    _recruiter.postCode = recruiter.postCode;
                    _recruiter.recordCreateDateTime = recruiter.recordCreateDateTime;
                    _recruiter.recordUpdateDateTime = DateTime.Now;
                    await _recrutingApiDBContext.SaveChangesAsync();
                    return _helpers.bindResponseData("Record Updated", "", true);
                }
                else
                {
                    return _helpers.bindResponseData("No Record Found", "", true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// get all recruiter
        /// </summary>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> getAllRecruiter()
        {
            try
            {
                List<Recruiter> _recruiter = new List<Recruiter>();
                _recruiter = _recrutingApiDBContext.recruiters.ToList();
                if (_recruiter.Count > 0)
                {
                    return _helpers.bindResponseData("", _recruiter.ToJson(), true);
                }
                else
                {
                    return _helpers.bindResponseData("No Record Found", "", true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// get recruiter by recruiterId
        /// </summary>
        /// <param name="recruiterId"></param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> getRecruiterbyId(int recruiterId)
        {
            try
            {
                Recruiter _recruiter = new Recruiter();
                _recruiter = _recrutingApiDBContext.recruiters.Where(x => x.Id == recruiterId).FirstOrDefault();
                if (_recruiter != null)
                {
                    return _helpers.bindResponseData("", _recruiter.ToJson(), true);
                }
                else
                {
                    return _helpers.bindResponseData("No Record Found", "", true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// delete recruiter by rercruiterId
        /// </summary>
        /// <param name="recruiterId"></param>
        /// <returns>ResponseResult</returns>

        public async Task<ResponseResult> deleteRecruiterbyId(int recruiterId)
        {
            try
            {
                Recruiter _recruiter = new Recruiter();
                _recruiter = _recrutingApiDBContext.recruiters.Where(x => x.Id == recruiterId).FirstOrDefault();
                if (_recruiter != null)
                {
                    _recrutingApiDBContext.recruiters.Remove(_recruiter);
                    _recrutingApiDBContext.SaveChanges();
                    // delete user in user table to remove login access
                    _userDAL.DeleteUser(_recruiter.emailAddress, role.Recruiter);
                    return _helpers.bindResponseData("Recruiter Deleted Successfully", "", true);
                }
                else
                {
                    return _helpers.bindResponseData("No Record Found", "", true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}