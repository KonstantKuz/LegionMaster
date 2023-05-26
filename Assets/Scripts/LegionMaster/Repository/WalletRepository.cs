using System.Collections.Generic;

namespace LegionMaster.Repository
{
    public class WalletRepository: LocalPrefsSingleRepository<Dictionary<string, int>>
    {
        protected WalletRepository() : base("wallet")
        {
        }
    }
}