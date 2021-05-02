using Apollo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Apollo.Services;
using Apollo.Data;
using Apollo.Entities;
using Apollo.Enums;
using Microsoft.AspNetCore.Http;

namespace Apollo.Controllers 
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase 
    {
        readonly ApolloContext _db;
        readonly PlayerService _playerService;

        public PlayerController(
            ApolloContext db,
            PlayerService playerService
        )
        {
            _db = db;
            _playerService = playerService;
        }

        [HttpPost("/player-register")]
        public IActionResult PlayerRegister(PlayerRegisterViewModel playerVM) 
        {
            bool controlResult = _playerService.PlayerRegisterFormDataControl(playerVM);
            if(controlResult)
            {
                bool newUserControl = _playerService.NewAccountControl(playerVM.MailAddress, playerVM.PhoneNumber);
                if(newUserControl)
                {
                    _playerService.CreatePlayer(playerVM);
                    return Ok(true);
                }
                else 
                {
                    return BadRequest(error: new { errorCode = ErrorCode.UserExists });
                }
            }
            else 
            {
                return BadRequest(error: new { errorCode = ErrorCode.MustBeFilled });
            }
        }

        [HttpPost("/player-login")]
        public IActionResult PlayerLogin(PlayerLoginViewModel playerVM)
        {
            Player userControl =  _playerService.PlayerLoginControl(playerVM);
            if(userControl != null)
            { 
                string userJWT = _playerService.PlayerLogin(userControl);
                Response.Cookies.Append("apolloJWT", userJWT, new CookieOptions 
                {
                    HttpOnly = true
                });
                return Ok(true);
            }
            else
            {
                return BadRequest(error: new { errorCode = ErrorCode.InvalidCredentials });
            }
        }
    }
}