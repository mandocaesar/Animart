using Abp.Domain.Repositories;
using Abp.Domain.Services;


namespace Animart.Portal.Supply
{
    public class SupplyDomainService:DomainService
    {
        private readonly IRepository<SupplyItem> _supplyRepository;

        public SupplyDomainService(IRepository<SupplyItem> supplyRepository)
        {
            _supplyRepository = supplyRepository;
        }
    }
}
