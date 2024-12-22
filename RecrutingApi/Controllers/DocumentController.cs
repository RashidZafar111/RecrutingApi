using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using RecrutingApi.DataAccessLayer;
using RecrutingApi.Model;

namespace RecrutingApi.Controllers
{
    /// <summary>
    /// Creating a extra layer of secruity using Autorize attribute, it restricts access to certain
    /// parts of the application to only those users who meet specific authorization criteria, such
    /// as being authenticated or having a particular role or permission.
    /// </summary>
    [Authorize]
    public class DocumentController : Controller
    {
        private IHttpContextAccessor _httpContext;
        private readonly DocumentDAL _documentDAL;
        private ResponseResult _responseResult;

        public DocumentController(DocumentDAL documentDAL, ResponseResult responseResult)
        {
            _documentDAL = documentDAL;
            _responseResult = responseResult;
        }

        /// <summary>
        /// Candidate can upload/re-upload the resume
        /// </summary>
        /// <param name="uploadResume"></param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [Authorize(Roles = "Candidate")]
        [Route("document/uploadDocumentCandidate/")]
        public async Task<IActionResult> uploadDocumentCandidate(IFormFile uploadResume)
        {
            try
            {
                ResponseResult responseResult = new ResponseResult();
                responseResult = await _documentDAL.UploadFiles(uploadResume);
                return Ok(responseResult.ToJson());
            }
            catch (Exception ex)
            {
                return Problem(type: "Upload Document Candidate Error",
              title: $"Document Upload",
              detail: $"{ex.ToString()}",
              statusCode: StatusCodes.Status500InternalServerError);
            }
           
        }

        /// <summary>
        /// Recruiter can see all the list of documents uploaded by all recruiters
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [Route("document/getAllDocumentList/")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> getAllDocumentListAsync()
        {
            try
            { 
            _responseResult = await _documentDAL.getAllDocumentList();
            return Ok(_responseResult.ToJson());
            }
            catch (Exception ex)
            {
                return Problem(type: "Get All Document List Error",
              title: $"Document List",
              detail: $"{ex.ToString()}",
              statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Recruiter can edit the document based on documentId
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="uploadCSV"></param>
        /// <returns>IActionResult</returns>
        [HttpPut]
        [Route("document/editDocument/{documentId}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> editDocument(int documentId, IFormFile uploadCSV)
        {
            try { 
            ResponseResult responseResult = new ResponseResult();
            var data = await _documentDAL.EditFiles(documentId, uploadCSV);
            return Ok(data.ToJson());
            }
            catch (Exception ex)
            {
                return Problem(type: "Edit Document Error",
              title: $"Edit Document",
              detail: $"{ex.ToString()}",
              statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Recruiter can delete the document based on documentId
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("document/deleteDocument/{documentId}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> deleteDocument(int documentId)
        {
            try
            { 
            ResponseResult responseResult = new ResponseResult();
            var data = await _documentDAL.DeleteFiles(documentId);
            return Ok(data.ToJson());
            }
            catch (Exception ex)
            {
                return Problem(type: "Delete Document Error",
              title: $"Delete Document",
              detail: $"{ex.ToString()}",
              statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Recruiter can create the document contaning the data of all candidates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("document/createDocument/")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> createDocumentAsync()
        {
            try
            { 
            var data = await _documentDAL.createDocument();
            return Ok(data.ToJson());
        }
            catch (Exception ex)
            {
                return Problem(type: "Create Document Error",
              title: $"Create Document",
        detail: $"{ex.ToString()}",
              statusCode: StatusCodes.Status500InternalServerError);
    }
}
    }
}