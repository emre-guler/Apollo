using System;
using System.Linq;
using System.Text.RegularExpressions;
using Apollo.Data;
using Apollo.Entities;
using Apollo.ViewModels;

namespace Apollo.Services
{
    public class TeamService
    {
        readonly ApolloContext _db;
        readonly AuthenticationService _authenticationService ;

        public TeamService(
            ApolloContext db,
            AuthenticationService authenticationService
        )
        {
            _db = db;
            _authenticationService = authenticationService;
        }

        public bool TeamRegisterFormDataControl(TeamRegisterViewModel teamVM)
        {
            teamVM.TeamName = teamVM.TeamName.Trim();
            teamVM.PhoneNumber = teamVM.PhoneNumber.Trim();
            teamVM.Password = teamVM.Password.Trim();
            teamVM.PasswordVerification = teamVM.PasswordVerification.Trim();
            teamVM.MailAddress = teamVM.MailAddress.Trim();
            Regex regForMail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"); 
            Regex regForPhone = new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");
            if(
                (!String.IsNullOrEmpty(teamVM.TeamName)) &&
                (!String.IsNullOrEmpty(teamVM.PhoneNumber)) &&
                (!String.IsNullOrEmpty(teamVM.Password)) &&
                (!String.IsNullOrEmpty(teamVM.PasswordVerification)) &&
                (!String.IsNullOrEmpty(teamVM.MailAddress)) &&
                (teamVM.Password == teamVM.PasswordVerification) &&
                (teamVM.Password.Length >= 8) &&
                (regForMail.Match(teamVM.MailAddress).Success) && 
                (regForPhone.Match(teamVM.PhoneNumber).Success)
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool NewAccountControl(string mailAddress, string phoneNumber)
        {
            bool teamControl = _db.Teams
                .Any(x => x.MailAddress == mailAddress || x.PhoneNumber == phoneNumber);
            if(teamControl)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateTeam(TeamRegisterViewModel teamVM)
        {
            _db.Teams.Add(new Team {
                CreatedAt = DateTime.Now,
                MailAddress = teamVM.MailAddress,
                Password = BCrypt.Net.BCrypt.HashPassword(teamVM.Password),
                PhoneNumber = teamVM.TeamName
            });
            _db.SaveChanges();
        }

        public Team TeamLoginControl(LoginViewModel teamVM)
        {
            if(!String.IsNullOrEmpty(teamVM.MailAddress) || !String.IsNullOrEmpty(teamVM.Password))
            {
                Team team = _db.Teams
                    .Where(x => x.MailAddress == teamVM.MailAddress)
                    .FirstOrDefault();
                bool passwordControl = BCrypt.Net.BCrypt.Verify(teamVM.Password, team.Password);
                if(team != null && passwordControl)
                {
                    return team;
                }
            }
            return null;
        }

        public string TeamLogin(int teamId)
        {
            return _authenticationService.GenerateToken(teamId);
        }
    }
}