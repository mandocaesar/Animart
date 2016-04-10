using Abp.Domain.Repositories;
using Abp.Domain.Services;


namespace Animart.Portal.Supply
{
    public class SupplyDomainService:DomainService
    {
        public IRepository<SupplyItem> SupplyRepository { get; }
    
        public IRepository<SupplyImage> SupplyImageRepository { get; }

        public SupplyDomainService(IRepository<SupplyItem> supplyRepository, IRepository<SupplyImage> supplyImageRepository)
        {
            SupplyRepository = supplyRepository;
            SupplyImageRepository = supplyImageRepository;
        }


    }
}
