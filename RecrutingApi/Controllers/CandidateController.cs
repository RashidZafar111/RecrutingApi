using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using RecrutingApi.DataAccessLayer;
using RecrutingApi.Model;

namespace RecrutingApi.Controllers
{
    /// <summary>
    /// ///
    /// <summary>
    /// Creating a extra layer of secruity using Autorize attribute, it restricts access to certain
    /// parts of the application to only those users who meet specific authorization criteria, such
    /// as being authenticated or having a particular role or permission.
    /// </summary>
    /// </summary>
    [Authorize]
    [Authorize(Roles = "Recruiter")]
    public class CandidateController : ControllerBase
    {
        private readonly CandidateDAL _candidateDAL;
        private ResponseResult _responseResult;

        public CandidateController(CandidateDAL candidateDAL, ResponseResult responseResult)
        {
            _candidateDAL = candidateDAL;
            _responseResult = responseResult;
        }

        /// <summary>
        /// Get specific candidate details based on the candidateId
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [Route("candidate/getCandidatebyId/{candidateId}")]
        public async Task<IActionResult> GetCandidatebyIdAsync(int candidateId)
        {
            try
            {
                _responseResult = await _candidateDAL.getCandidatebyId(candidateId);
                return Ok(_responseResult.ToJson());
            }
            catch (Exception ex)
            {
                return Problem(type: "Get Candidate Failed",
               title: $"Error Getting Data",
               detail: $"{ex.ToString()}",
               statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get all candidate details
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [Route("candidate/getAllcandidate/")]
        public async Task<IActionResult> GetAllCandidateAsync()
        {
            try
            {
                _responseResult = await _candidateDAL.getAllCandidate();
                return Ok(_responseResult.ToJson());
            }
            catch (Exception ex)
            {
                return Problem(type: "Get Candidate Failed",
               title: $"Error Getting Data",
               detail: $"{ex.ToString()}",
               statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update specific candidate details based on the candidateId
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns>IActionResult</returns>
        [HttpPut]
        [Route("candidate/updateCandidate/{candidateId}")]
        public async Task<IActionResult> UpdateCandidateAsync(int candidateId, [FromBody] Candidate candidate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _responseResult = await _candidateDAL.UpdateCandidate(candidateId, candidate);
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
        /// Insert new candidate
        /// </summary>
        /// <param name="candiate"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("candidate/insertCandidate")]
        public async Task<IActionResult> insertCandidateAsync([FromBody] Candidate candidate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _responseResult = await _candidateDAL.CreateCandidate(candidate);
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
                return Problem(type: "Inser Candidate Error",
               title: $"Insert Failed",
               detail: $"{ex.ToString()}",
               statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete specific candidate details based on the candidateId
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("candidate/deleteCandidatebyId/{candidateId}")]
        public async Task<IActionResult> DeleteRecruiterbyIdAsync(int candidateId)
        {
            try
            {
                _responseResult = await _candidateDAL.deleteCandidatebyId(candidateId);
                return Ok(_responseResult.ToJson());
            }
            catch (Exception ex)
            {
                return Problem(type: "Delete Candidate Error",
               title: $"Delete Failed",
               detail: $"{ex.ToString()}",
               statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}