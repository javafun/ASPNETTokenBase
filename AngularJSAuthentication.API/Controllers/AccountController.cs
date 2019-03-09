using AngularJSAuthentication.API.Models;
using AngularJSAuthentication.API.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace AngularJSAuthentication.API.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private AuthRepository _authRepository;

        public AccountController()
        {
            _authRepository = new AuthRepository();
        }

        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            IdentityResult result = await _authRepository.RegisterUserAsync(userModel);

            IHttpActionResult errResult = GetErrResult(result);

            if(errResult != null)
            {
                return errResult;
            }

            return Ok();
        }

        private IHttpActionResult GetErrResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if(result.Errors != null)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
