using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using sp_project_guide_api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace sp_project_guide_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private IConfiguration _config;
        public IdentityController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("token")]
        public IActionResult GenerateToken([FromBody]RequestCustomClaim rqc)
        {
            //validate the RQC value. 
            if (rqc != null)
            {
                //user  ID and Email from the Request Claim for a Token must match expected value, otherwise the user cannot be validated.
                if (rqc.UserId == "myPrivateUserId" && rqc.Email == "myPrivate@email.com")
                {
                    //Here is what we can use to differentiate claims. We verify the users 
                    //RQC data by checking that the data inside is what we actually want. 
                    //In real scenarios, youd probably have a list of accepted values / stored
                    //somewhere securely that can be accessed, and that data would be used to verify

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                    //for ease of access, the Issuer is used for both. it made testing easier for some reason.
                    var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
                      _config["Jwt:Issuer"],
                      null,
                      expires: DateTime.Now.AddMinutes(120),
                      signingCredentials: credentials);

                    var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

                    return Ok(token);
                }
            }

            //if they dont have any RQC or the right data, we want to return Unauthorised as they are not welcome if they cannot be validated
            return Unauthorized();
        }
    }
}
