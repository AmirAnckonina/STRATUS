using Microsoft.AspNetCore.Mvc;
using StratusApp.Models.Responses;
using StratusApp.Models;

namespace StratusApp.Services
{
    public interface IStratusService
    {
        Task<ActionResult<StratusResponse<StratusUser>>> GetStratusUser(string username);

        Task<ActionResult<StratusResponse<List<StratusUser>>>> GetAllStratusUsers();
    }
}
