using Microsoft.AspNetCore.Mvc;
using StratusApp.Models.Responses;
using StratusApp.Models;
using Utils.DTO;

namespace StratusApp.Services
{
    public interface IStratusService
    {
        Task<StratusUser?> GetUserByEmail(string email);
        Task<bool> UpdateUserDetails(string userEmail, StratusUser user);
    }
}
