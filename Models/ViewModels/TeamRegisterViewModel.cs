using Microsoft.AspNetCore.Http;

namespace Apollo.ViewModels
{
    public class TeamRegisterViewModel
    {
        public string TeamName { get; set; }
        public string MailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public IFormFile ProfilePhoto { get; set; }
    }
}