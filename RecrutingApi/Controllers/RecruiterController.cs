using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using RecrutingApi.DataAccessLayer;
using RecrutingApi.Model;
using System.Data;

namespace RecrutingApi.Controllers
{
    /// <summary>
    /// Creating a extra layer of secruity using Autorize attribute, it restricts access to certain
    /// parts of the application to only those users who meet specific authorization criteria, such
    /// as being authenticated or having a particular role or permission.
    /// </summary>
    [Authorize]
    public class RecruiterController : Controller
    {
        private readonly RecruiterDAL _recruiterDAL;
        private ResponseResult _responseResult;

        public RecruiterController(RecruiterDAL recruiterDAL, ResponseResult responseResult)
        {
            _recruiterDAL = recruiterDAL;
            _responseResult = responseResult;
        }

        /// <summary>
        /// Restricting the access to recruiter as candidates can not see recruiter details
        /// </summary>
        /// <param name="recruiterId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("recruiter/getRecruiterbyId/{recruiterId}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetRecruiterbyIdAsync(int recruiterId)
        {
            try
            {
                _responseResult = await _recruiterDAL.getRecruiterbyId(recruiterId);
                return Ok(_responseResult.ToJson());
            }
            catch (Exception ex)
            {
                return Problem(type: "Get Recruiter Failed",
               title: $"Error Getting Data",
               detail: $"{ex.ToString()}",
               statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Restricting the access to recruiter as candidates can not see recruiter details
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [Route("recruiter/getAllRecruiter/")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetAllRecruiterAsync()
        {
            try
            {
                _responseResult = await _recruiterDAL.getAllRecruiter();
                return Ok(_responseResult.ToJson());
            }
            catch (Exception ex)
            {
                return Problem(type: "Get Recruiter Failed",
               title: $"Error Getting Data",
               detail: $"{ex.ToString()}",
               statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Restricting the access to recruiter as candidates can not see recruiter details.
        /// Updating the recturiter based on the recruiterId
        /// </summary>
        /// <param name="recruiterId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("recruiter/updateRecruiter/{recruiterId}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateRecruiterAsync(int recruiterId, [FromBody] Recruiter recruiter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _responseResult = await _recruiterDAL.UpdateRecruiter(recruiterId, recruiter);
                    return Ok(_responseResult.ToJson());
                }
                else
                {
                    return Problem(type: "Bad Request",
                        title: $"{string.Join(",", ModelState.Keys.ToArray())}",
                        detail: $"{string.Join(",", ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                        .Select(m => m.ErrorMessage).ToArray())}",
                        statusCode: StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Problem(type: "Update Recruiter Error",
               title: $"Update Failed",
               detail: $"{ex.ToString()}",
               statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Inserting recruiters data
        /// </summary>
        /// <param name="recruiter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("recruiter/insertRecruiter")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> InsertRecruiterAsync([FromBody] Recruiter recruiter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _responseResult = await _recruiterDAL.CreateRecruiter(recruiter);
                    return Ok(_responseResult.ToJson());
                }
                else
                {
                    return Problem(type: "Bad Request",
                      title: $"{string.Join(",", ModelState.Keys.ToArray())}",
                      detail: $"{string.Join(",", ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                      .Select(m => m.ErrorMessage).ToArray())}",
                      statusCode: StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Problem(type: "Inser Recruiter Error",
               title: $"Insert Failed",
               detail: $"{ex.ToString()}",
               statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete recruiter based on recruiterId
        /// </summary>
        /// <param name="recruiterId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("recruiter/deleteRecruiterbyId/{recruiterId}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> DeleteRecruiterbyIdAsync(int recruiterId)
        {
            try
            {
                _responseResult = await _recruiterDAL.deleteRecruiterbyId(recruiterId);
                return Ok(_responseResult.ToJson());
            }
            catch (Exception ex)
            {
                return Problem(type: "Delete Recruiter Error",
               title: $"Delete Failed",
               detail: $"{ex.ToString()}",
               statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}