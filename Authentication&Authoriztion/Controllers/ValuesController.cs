using Authentication_Authoriztion.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Net;
using System.Security.Claims;

namespace Authentication_Authoriztion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly UserManager<UserModel> _userManager;

        public ValuesController(UserManager<UserModel> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2", "value3" };
        }
        [HttpGet("for-admin")]
        [Authorize(Policy = Constants.Policies.ForAdminOnly)]
        public IEnumerable<string> GetForAdmin()
        {
            return new string[] { "value3", "value4" };
        }

        [HttpGet("for-testing")]
        [Authorize(Policy = Constants.Policies.ForTestingOnly)]
        public async Task<IEnumerable<string>> GetForTesting()
        {
            //User is a prop in ControllerBase of type ClaimsPrincipal
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            var user2 = await _userManager.GetUserAsync(User);// Assume id is stored in nameidentifier

            return new string[] { "value1", "value2" };
        }
    }
}
