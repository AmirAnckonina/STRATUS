using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StratusApp.Data;
using StratusApp.Models;
using StratusApp.Models.Responses;

namespace StratusApp.Services
{
    public class StratusService : IStratusService
    {
        private readonly List<StratusUser> _users = new List<StratusUser>()
        {
            new StratusUser {Id = 1, Name = "user A"},
            new StratusUser {Id = 2, Name = "user B"},
        };

        public Task<ActionResult<StratusResponse<List<StratusUser>>>> GetAllStratusUsers()
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<StratusResponse<StratusUser>>> GetStratusUser(string username)
        {
            throw new NotImplementedException();
        }

        //private readonly DataContext _dataContext;

        /*public StratusService(DataContext dataContext) 
        {
            _dataContext = dataContext;
        }*/


        /*public async Task<ActionResult<StratusResponse<StratusUser>>> GetStratusUser(string username)
        {
            var getStratusUserResp = new StratusResponse<StratusUser>();
            StratusUser stratusUser = _users.FirstOrDefault(user => user.Name == username);
            getStratusUserResp.Data = stratusUser;

            return getStratusUserResp;
        }

        public async Task<ActionResult<StratusResponse<List<StratusUser>>>> GetAllStratusUsers()
        {
            var getAllStratusUsersResp = new StratusResponse<List<StratusUser>>();
            var dbStratusUsers = await _dataContext.Users.ToListAsync();
            getAllStratusUsersResp.Data = dbStratusUsers;

            return getAllStratusUsersResp;
        }*/
    }
}
