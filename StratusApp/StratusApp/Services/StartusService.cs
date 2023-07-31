using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StratusApp.Data;
using StratusApp.Models;
using StratusApp.Models.Responses;
using Utils.DTO;

namespace StratusApp.Services
{
    public class StratusService : IStratusService
    {
        public Task<ActionResult<StratusResponse<List<StratusUser>>>> GetAllStratusUsers()
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<StratusResponse<StratusUser>>> GetStratusUser(string username)
        {
            throw new NotImplementedException();
        }
    }
}
