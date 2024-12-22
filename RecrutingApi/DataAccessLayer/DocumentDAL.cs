using NuGet.Protocol;
using RecrutingApi.DBContext;
using RecrutingApi.Helper;
using RecrutingApi.Model;
using Document = RecrutingApi.Model.Document;

namespace RecrutingApi.DataAccessLayer
{
    public class DocumentDAL
    {
        private readonly RecrutingApiDBContext _recrutingApiDBContext;
        private readonly IHttpContextAccessor _httpContext;
        private Helpers _helpers;
        private readonly IConfiguration _configuration;

        public DocumentDAL(RecrutingApiDBContext recrutingApiDBContext, IHttpContextAccessor httpContext, Helpers helpers, IConfiguration configuration)
        {
            _recrutingApiDBContext = recrutingApiDBContext;
            _httpContext = httpContext;
            _helpers = helpers;
            _configuration = configuration;
        }

        /// <summary>
        /// Getting the list of all the documents created by recruiters
        /// </summary>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> getAllDocumentList()
        {
            var docDetails = _recrutingApiDBContext.documents.
                Join(_recrutingApiDBContext.users,
                 doc => doc.uId,
                usr => usr.Id,
                (doc, usr) => new
                {
                    doc.fileName,
                    usr.emailAddress,
                    doc.CreateDateTime,
                    doc.Id
                });
            if (docDetails != null)
            {
                return _helpers.bindResponseData("", docDetails.ToJson(), true);
            }
            else
            {
                return _helpers.bindResponseData("No Record Found", "", true);
            }
        }

        /// <summary>
        /// Upload and re-upload the candidate resume
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UploadFiles(IFormFile uploadedFile)
        {
            if (uploadedFile.ContentType.ToLower().Contains("text/csv"))
            {
                string fileName = string.Empty;
                bool updtFileName = true;
                Candidate candidate = new Candidate();
                int candidateId = Convert.ToInt32(_httpContext.HttpContext.Session.GetString("UserId"));
                candidate = _helpers.getCandidateData(candidateId);

                if (candidate != null)
                {
                    if (uploadedFile != null && uploadedFile.Length != 0)
                    {
                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetValue<string>("UploadFilePathCandidate"));
                        _helpers.CreateDirectory(folderPath);
                        fileName = candidate.resumefName != null ? $"{candidate.resumefName}" : _helpers.GetFileName();
                        updtFileName = await _helpers.UpdateCandidateResumeInfo(candidateId, fileName);
                        if (updtFileName)
                        {
                            await _helpers.DeleteFile(folderPath + $"\\" + fileName);
                            await _helpers.UploadFileAsync(folderPath + $"\\" + fileName, uploadedFile);
                            return _helpers.bindResponseData("File Uploaded Successfully", "", true);
                        }
                        else
                        {
                            return _helpers.bindResponseData("Error Uploading File", "", true);
                        }
                    }
                    else
                    {
                        return _helpers.bindResponseData("No File Found", "", true);
                    }
                }
                else
                {
                    return _helpers.bindResponseData("No Record Found", "", true);
                }
            }
            else
            {
                return _helpers.bindResponseData("Only CSV File Allowed", "", true);
            }
        }

        /// <summary>
        /// Edit the document created by recruiter
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        public async Task<ResponseResult> EditFiles(int documentId, IFormFile uploadedFile)
        {
            if (uploadedFile.ContentType.ToLower().Contains("text/csv"))
            {
                Document document = new Document();
                string filePath = string.Empty;
                string fileName = string.Empty;
                document = _recrutingApiDBContext.documents.Where(x => x.Id == documentId).FirstOrDefault();
                if (document != null)
                {
                    if (uploadedFile != null && uploadedFile.Length != 0)
                    {
                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetValue<string>("UploadFilePathRecruiter"));
                        _helpers.CreateDirectory(folderPath);
                        fileName = document.fileName != null ? $"\\{document.fileName}" : _helpers.GetFileName();
                        await _helpers.DeleteFile(folderPath + fileName);
                        await _helpers.UploadFileAsync(folderPath + fileName, uploadedFile);

                        return _helpers.bindResponseData("File Edited Successfully", "", true);
                    }
                    else
                    {
                        return _helpers.bindResponseData("No File Found", "", true);
                    }
                }
                else
                {
                    return _helpers.bindResponseData("No Record Found", "", true);
                }
            }
            else
            {
                return _helpers.bindResponseData("Only CSV File Allowed", "", true);
            }
        }

        /// <summary>
        /// Delete candidate resume one candidate is deleted
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<ResponseResult> DeleteCandidateResume(string fileName)
        {
            if (fileName != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetValue<string>("UploadFilePathCandidate"));
                var filePath = folderPath + $"\\{fileName}";
                await _helpers.DeleteFile(filePath);
                return _helpers.bindResponseData("File Deleted Successfully", "", true);
            }
            else
            {
                return _helpers.bindResponseData("No Record Found", "", true);
            }
        }

        /// <summary>
        /// Delete the document created by recruiter
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public async Task<ResponseResult> DeleteFiles(int documentId)
        {
            Document document = new Document();
            document = _recrutingApiDBContext.documents.Where(x => x.Id == documentId).FirstOrDefault();
            if (document != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetValue<string>("UploadFilePathRecruiter"));
                var filePath = folderPath + $"\\{document.fileName}";
                await _helpers.DeleteFile(filePath);
                return _helpers.bindResponseData("File Deleted Successfully", "", true);
            }
            else
            {
                return _helpers.bindResponseData("No Record Found", "", true);
            }
        }

        /// <summary>
        /// Create the csv document by recruiter
        /// </summary>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> createDocument()
        {
            try
            {
                List<Candidate> candidate = new List<Candidate>();
                candidate = _recrutingApiDBContext.candiates.ToList();
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetValue<string>("UploadFilePathRecruiter"));
                string fileName = $"\\{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
                if (candidate != null)
                {
                    _helpers.CreateDirectory(folderPath);

                    await _helpers.CreateCSVFileAsync(folderPath, fileName, candidate);
                    return _helpers.bindResponseData("Paste the link in the browser download the file", folderPath + fileName, true);
                }
                else
                {
                    File.WriteAllText(folderPath, candidate.ToString());
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