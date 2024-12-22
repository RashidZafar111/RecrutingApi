using CsvHelper;
using RecrutingApi.DBContext;
using RecrutingApi.Model;
using System.Globalization;
using System.Security.Cryptography;

namespace RecrutingApi.Helper
{
    public class Helpers
    {
        private readonly RecrutingApiDBContext _recrutingApiDBContext;

        private readonly IHttpContextAccessor _httpContext;

        public Helpers(RecrutingApiDBContext recrutingApiDBContext, IHttpContextAccessor httpContext)

        {
            _recrutingApiDBContext = recrutingApiDBContext;
            _httpContext = httpContext;
        }

        public string CreatePassword()
        {
            var randomGenerator = RandomNumberGenerator.Create();
            var data = new byte[16];
            randomGenerator.GetBytes(data);
            return BitConverter.ToString(data).Replace("-", "");
        }

        public void CreateDirectory(string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }

        public async Task DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public string GetFileName()
        {
            return $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
        }

        public async Task UploadFileAsync(string filepath, IFormFile uploadedFile)
        {
            using (var stream = new FileStream(filepath, FileMode.CreateNew))
            {
                await uploadedFile.CopyToAsync(stream);
            }
        }

        /// <summary>
        /// Upload the document details created by recruiters
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>N/A</returns>
        public async Task CreateCSVFileAsync(string folderPath, string fileName, List<Candidate> candidate)
        {
            using (var writer = new StreamWriter(folderPath + fileName))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecordsAsync(candidate);
                }
            }
            Document document = new Document();
            document.fileName = fileName;
            document.uId = Convert.ToInt32(_httpContext.HttpContext.Session.GetString("UserId"));
            document.Id = GetNewDocumentId();
            document.CreateDateTime = DateTime.Now;
            await _recrutingApiDBContext.documents.AddAsync(document);
            await _recrutingApiDBContext.SaveChangesAsync();
        }

        /// <summary>
        /// Update candidate data
        /// </summary>
        /// <param name="candidateId"></param>
        /// <param name="candidate"></param>
        /// <returns>ResponseResult</returns>
        public async Task<bool> UpdateCandidateResumeInfo(int candidateId, string candidateResume)
        {
            try
            {
                Candidate _candidate = new Candidate();
                _candidate = _recrutingApiDBContext.candiates.Where(x => x.Id == candidateId).FirstOrDefault();
                if (_candidate.resumefName == null)
                {
                    _candidate.resumefName = candidateResume;
                    _candidate.recordUpdateDateTime = DateTime.Now;
                    await _recrutingApiDBContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int GetNewUserId()
        {
            int newId = _recrutingApiDBContext.users.Count() == 0 ? 1 :
               _recrutingApiDBContext.users.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            return newId;
        }

        public int GetNewRecruiterId()
        {
            int newId = _recrutingApiDBContext.recruiters.Count() == 0 ? 1 :
               _recrutingApiDBContext.recruiters.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            return newId;
        }

        public int GetNewCandidateId()
        {
            int newId = _recrutingApiDBContext.candiates.Count() == 0 ? 1 :
               _recrutingApiDBContext.candiates.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            return newId;
        }

        public int GetNewDocumentId()
        {
            int newId = _recrutingApiDBContext.documents.Count() == 0 ? 1 :
               _recrutingApiDBContext.documents.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            return newId;
        }

        public int getUserRole(string UserKey)
        {
            int newId = _recrutingApiDBContext.users.OrderBy(x => x.Id).FirstOrDefault().Id + 1;
            return newId;
        }

        public Candidate getCandidateData(int candidateId)
        {
            Candidate candidate = new Candidate();
            candidate = _recrutingApiDBContext.candiates.Where(x => x.Id == candidateId).FirstOrDefault();
            return candidate;
        }

        public ResponseResult bindResponseData(string Message, string responseData, bool isSuccess)
        {
            ResponseResult responseResult = new ResponseResult();
            responseResult.message = Message;
            responseResult.responsedata = responseData;
            responseResult.isSuccess = isSuccess;
            return responseResult;
        }
    }
}