using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using RecrutingApi.DataAccessLayer;
using RecrutingApi.Helper;
using RecrutingApi.Model;
using static RecrutingApi.Model.Role;

namespace RecrutingApi.Controllers
{
    public class UserController : ControllerBase
    {
        private UserDAL _userDAL;

        private readonly Helpers _helpers;

        public UserController(UserDAL userDAL, Helpers helper)

        {
            _userDAL = userDAL;
            _helpers = helper;
        }

        /// <summary>
        /// Authenticate Candidate or Recruiter or SuperUser
        /// </summary>
        /// <param name="authentciate"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("authenticate/authenticateUser")]
        public async Task<IActionResult> authenticateUser([FromBody] Authentciate authentciate)
        {
            try
            {
                ResponseResult responseResult = new ResponseResult();
                string result = string.Empty;
                if (!ModelState.IsValid)
                {
                    return Problem(type: "Bad Request",
                        title: $"{string.Join(",", ModelState.Keys.ToArray())}",
                        detail: $"{string.Join(",", ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                    .Select(m => m.ErrorMessage).ToArray())}",
                        statusCode: StatusCodes.Status400BadRequest);
                }
                else
                {
                    responseResult = await _userDAL.ValidateUser(authentciate);

                    return Ok(responseResult.ToJson());
                }
            }
            catch (Exception ex)
            {
                return Problem(type: "Internal Server Error",
                         title: $"Authenticate User",
                         detail: $"{ex}",
                         statusCode: StatusCodes.Status404NotFound);
            }
        }

        /// <summary>
        /// Authenticate Candidate or Recruiter or SuperUser
        /// </summary>
        /// <param name="authentciate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("authenticate/getSuperUser/")]
        public async Task<IActionResult> CreateSuperUser()
        {
            try
            {
                Users user = new Users();
                string result = string.Empty;
                if (!ModelState.IsValid)
                {
                    return Problem(type: "Bad Request",
                        title: $"{string.Join(",", ModelState.Keys.ToArray())}",
                        detail: $"{string.Join(",", ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                    .Select(m => m.ErrorMessage).ToArray())}",
                        statusCode: StatusCodes.Status400BadRequest);
                }
                else
                {
                    user = await _userDAL.CreateUser("superusr@example.com", role.SuperAdmin, 0);
                    if (user != null)
                    {
                        return Ok(_helpers.bindResponseData("Super User Created Successfully", $"UserName : {user.emailAddress}, Password = {user.password}", true));
                    }
                    else
                    {
                        return Ok(_helpers.bindResponseData("Super User Created Successfully", $"UserName : {user.emailAddress}, Password = {user.password}", true));
                    }
                }
            }
            catch (Exception ex)
            {
                return Problem(type: "Internal Server Error",
                         title: $"Super User",
                         detail: $"{ex}",
                         statusCode: StatusCodes.Status404NotFound);
            }
        }
    }
}