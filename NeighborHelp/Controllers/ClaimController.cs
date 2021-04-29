using Microsoft.AspNetCore.Mvc;
using NeighborHelp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Controllers
{
    public class ClaimController : Controller
    {
        private IClaimDirectoryServise _claimDirectory;

        public ClaimController(IClaimDirectoryServise service)
        {
            _claimDirectory = service;
        }

        public IActionResult GetClaims()
        {
            return View();
        } 
    }
}
