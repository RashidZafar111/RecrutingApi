using NuGet.Protocol;
using RecrutingApi.DBContext;
using RecrutingApi.Helper;
using RecrutingApi.Model;
using static RecrutingApi.Model.Role;

namespace RecrutingApi.DataAccessLayer
{
    public class CandidateDAL
    {
        private readonly RecrutingApiDBContext _recrutingApiDBContext;
        private Helpers _helpers;
        private UserDAL _userDAL;
        private DocumentDAL _documentDAL;

        public CandidateDAL(RecrutingApiDBContext recrutingApiDBContext, Helpers helpers, UserDAL userDAL, DocumentDAL documentDAL)
        {
            _recrutingApiDBContext = recrutingApiDBContext;
            _helpers = helpers;
            _userDAL = userDAL;
            _documentDAL = documentDAL;
        }

        /// <summary>
        /// Creating new candidate
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> CreateCandidate(Candidate candidate)
        {
            try
            {
                Candidate _candidate = new Candidate();

                _candidate = _recrutingApiDBContext.candiates.Where(x => x.emailAddress == candidate.emailAddress).FirstOrDefault();
                if (_candidate == null)
                {
                    Users user = new Users();
                    candidate.Id = _helpers.GetNewCandidateId();
                    candidate.recordCreateDateTime = DateTime.Now;
                    await _recrutingApiDBContext.candiates.AddAsync(candidate);
                    await _recrutingApiDBContext.SaveChangesAsync();
                    // creating user in user table to apply normilization seperaiton of logic
                    user = await _userDAL.CreateUser(candidate.emailAddress, role.Candidate, candidate.Id);

                    return _helpers.bindResponseData("Candidate Successfully Created", $"UserName : {user.emailAddress}, Password = {user.password}", true);
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
        /// Update candidate
        /// </summary>
        /// <param name="candidateId"></param>
        /// <param name="candidate"></param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UpdateCandidate(int candidateId, Candidate candidate)
        {
            try
            {
                Candidate _candidate = new Candidate();
                _candidate = _recrutingApiDBContext.candiates.Where(x => x.Id == candidateId).FirstOrDefault();
                if (_candidate != null)
                {
                    _candidate.firstName = candidate.firstName;
                    _candidate.middleName = candidate.middleName;
                    _candidate.lastName = candidate.lastName;
                    _candidate.mobileNumber = candidate.mobileNumber;
                    _candidate.city = candidate.city;
                    _candidate.country = candidate.country;
                    _candidate.postCode = candidate.postCode;
                    _candidate.minSalary = candidate.minSalary;
                    _candidate.maxSalary = candidate.maxSalary;
                    _candidate.title = candidate.title;
                    _candidate.description = candidate.description;
                    _candidate.hightestEducation = candidate.hightestEducation;
                    _candidate.professionalCertificate = candidate.professionalCertificate;
                    _candidate.recordCreateDateTime = candidate.recordCreateDateTime;
                    _candidate.recordUpdateDateTime = DateTime.Now;
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
        /// Get all candidate
        /// </summary>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> getAllCandidate()
        {
            try
            {
                List<Candidate> _candidates = new List<Candidate>();
                _candidates = _recrutingApiDBContext.candiates.ToList();
                if (_candidates.Count > 0)
                {
                    return _helpers.bindResponseData("", _candidates.ToJson(), true);
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
        /// Get candidate by candidateId
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> getCandidatebyId(int candidateId)
        {
            try
            {
                Candidate _candidate = new Candidate();
                _candidate = _recrutingApiDBContext.candiates.Where(x => x.Id == candidateId).FirstOrDefault();
                if (_candidate != null)
                {
                    return _helpers.bindResponseData("", _candidate.ToJson(), true);
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
        /// get candidate by candidateId for CSV file creation
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns>ResponseResult</returns>
        public async Task<Candidate> getCandidateDatabyId(int candidateId)
        {
            try
            {
                Candidate _candidate = new Candidate();
                _candidate = _recrutingApiDBContext.candiates.Where(x => x.Id == candidateId).FirstOrDefault();
                if (_candidate != null)
                {
                    return _candidate;
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

        /// <summary>
        /// Delete candidate by candidateId
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns>ResponseResult</returns>

        public async Task<ResponseResult> deleteCandidatebyId(int candidateId)
        {
            try
            {
                Candidate _candidate = new Candidate();
                _candidate = _recrutingApiDBContext.candiates.Where(x => x.Id == candidateId).FirstOrDefault();
                if (_candidate != null)
                {
                    _recrutingApiDBContext.candiates.Remove(_candidate);
                    _recrutingApiDBContext.SaveChanges();

                    // delete user in user table to remove login access
                    _userDAL.DeleteUser(_candidate.emailAddress, role.Candidate);
                    _documentDAL.DeleteCandidateResume(_candidate.resumefName);
                    return _helpers.bindResponseData("Candidate Deleted Successfully", "", true);
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