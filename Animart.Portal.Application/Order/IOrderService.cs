using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Animart.Portal.Order.Dto;

namespace Animart.Portal.Order
{
    public interface IOrderService: IApplicationService
    {
        Task<Guid> Create(Dto.CreatePurchaseOrderDto purchaseOrderrderItem);

        bool Update(string id ,Dto.OrderItemInputDto orderItem);

        bool Delete(string id);

        List<PurchaseOrderDto> GetAllPurchaseOrderByUserId();

        OrderDashboardDto GetDashboard();

        OrderDashboardDto GetDashboardAdmin();

        PurchaseOrderDto GetSinglePurchaseOrder(string id);

        bool AddOrderItem(string id, List<Dto.OrderItemInputDto> orderItems);

        bool UpdateOrderItem(string id, Dto.OrderItemInputDto orderItem);

        bool DeleteOrderItem(string id, string orderItemId);

        List<int> UpdateChart();

        bool CheckOrderItem(Dto.OrderItemInputDto orderItem);

        bool UpdatePurchaseOrderStatus(string id, string status);

        List<PurchaseOrderDto> GetAllPurchaseOrderForMarketing();

        List<PurchaseOrderDto> GetAllPurchaseOrderForAccounting();

        List<PurchaseOrderDto> GetAllPurchaseOrderForLogistic();

        bool InsertReceiptNumber(string id, string receipt);
    }
}
