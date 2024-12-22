using CsvHelper;
using RecrutingApi.DBContext;
using RecrutingApi.Model;
using System.Globalization;
using System.Security.Cryptography;

namespace RecrutingApi.Helper
{
    /// <summary>
    /// Helper class is for common functions that we can use anywhere in the code by calling them
    /// instead of creating them again
    /// </summary>
    public class Helpers
    {
        private readonly RecrutingApiDBContext _recrutingApiDBContext;

        private readonly IHttpContextAccessor _httpContext;

        public Helpers(RecrutingApiDBContext recrutingApiDBContext, IHttpContextAccessor httpContext)

        {
            _recrutingApiDBContext = recrutingApiDBContext;
            _httpContext = httpContext;
        }

        /// <summary>
        /// Create random password for user to login
        /// </summary>
        /// <returns>String</returns>
        public string CreatePassword()
        {
            var randomGenerator = RandomNumberGenerator.Create();
            var data = new byte[16];
            randomGenerator.GetBytes(data);
            return BitConverter.ToString(data).Replace("-", "");
        }

        /// <summary>
        /// Create Directory if not exists
        /// </summary>
        /// <param name="filePath"></param>
        public void CreateDirectory(string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Generating filename to savefile
        /// </summary>
        /// <returns>tring</returns>
        public string GetFileName()
        {
            return $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
        }

        /// <summary>
        /// Upload file in the selected folder
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Generating unique userId
        /// </summary>
        /// <returns>Int32</returns>
        public int GetNewUserId()
        {
            int newId = _recrutingApiDBContext.users.Count() == 0 ? 1 :
               _recrutingApiDBContext.users.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            return newId;
        }

        /// <summary>
        /// Generating unique recruiterId
        /// </summary>
        /// <returns>Int32</returns>
        public int GetNewRecruiterId()
        {
            int newId = _recrutingApiDBContext.recruiters.Count() == 0 ? 1 :
               _recrutingApiDBContext.recruiters.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            return newId;
        }

        /// <summary>
        /// Generating unique candidateId
        /// </summary>
        /// <returns>Int32</returns>
        public int GetNewCandidateId()
        {
            int newId = _recrutingApiDBContext.candiates.Count() == 0 ? 1 :
               _recrutingApiDBContext.candiates.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            return newId;
        }

        /// <summary>
        /// Generating unique documentId
        /// </summary>
        /// <returns>Int32</returns>
        public int GetNewDocumentId()
        {
            int newId = _recrutingApiDBContext.documents.Count() == 0 ? 1 :
               _recrutingApiDBContext.documents.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            return newId;
        }

        /// <summary>
        /// Get Candidate data based on candidateId
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns>Candidate</returns>
        public Candidate getCandidateData(int candidateId)
        {
            Candidate candidate = new Candidate();
            candidate = _recrutingApiDBContext.candiates.Where(x => x.Id == candidateId).FirstOrDefault();
            return candidate;
        }

        /// <summary>
        /// Common funciton to bind response after request is successfull without any exception
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="responseData"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public ResponseResult bindResponseData(string Message, string responseData, bool isSuccess)
        {
            ResponseResult responseResult = new ResponseResult();
            responseResult.message = Message;
            responseResult.responseData = responseData;
            responseResult.isSuccess = isSuccess;
            return responseResult;
        }
    }
}