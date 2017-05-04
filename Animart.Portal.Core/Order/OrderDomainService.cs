using Abp.Domain.Repositories;
using Abp.Domain.Services;

namespace Animart.Portal.Order
{
    public class OrderDomainService : DomainService
    {
        private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;

        public OrderDomainService(IRepository<PurchaseOrder> purchaseOrderRepository)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
        }
    }
}
