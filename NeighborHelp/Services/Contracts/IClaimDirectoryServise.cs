using NeighborHelp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Services.Contracts
{
    public interface IClaimDirectoryServise
    {
        public bool TryAddClaim(Claim claim);
        public Claim GetClaim(int id);
        public IList<Claim> GetClaims(int userId);

        public IList<Claim> GetAllClaims();

        public bool TryPutClaim(Claim claim);
    }
}
