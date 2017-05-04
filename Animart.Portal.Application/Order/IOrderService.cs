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
        Task<Guid> Create(Dto.CreatePurchaseOrderDto purchaseOrderItem);
        Task<Guid> CreateInvoice(Dto.CreatePurchaseOrderDto purchaseOrderItem);

        bool Update(string id, Dto.OrderItemDto orderItem);
        bool UpdatePO(string poId, List<Dto.OrderItemDto> orderItems);

        bool Delete(string id);

        List<PurchaseOrderDto> GetAllPurchaseOrderByUserId(int type,int num);

        OrderDashboardDto GetDashboard();

        OrderDashboardDto GetDashboardAdmin();

        PurchaseOrderDto GetSinglePurchaseOrder(string id,int num);

        InvoicePODto GetSingleInvoice(string id);

        bool AddOrderItem(string id, List<Dto.OrderItemInputDto> orderItems);
        bool AddOrderItemToInvoice(string id, List<InvoiceInputDto> orderItems);

        bool UpdateOrderItem(string id, Dto.OrderItemInputDto orderItem);

        bool DeleteOrderItem(string id, string orderItemId);

        List<int> UpdateChart();

        bool CheckOrderItem(Dto.OrderItemInputDto orderItem);

        bool UpdatePurchaseOrderStatus(string id, string status);
        bool UpdateOrderItemStatus(string id, string status, List<InvoiceInputDto> orderItems);

        List<PurchaseOrderDto> GetAllPurchaseOrderForMarketing(int type,int num);

        List<PurchaseOrderDto> GetAllPurchaseOrderForAccounting(int type,int num);

        List<PurchaseOrderDto> GetAllPurchaseOrderForLogistic(int type,int num);

        bool InsertReceiptNumber(string id, string receipt);
        bool InsertInvoiceReceiptNumber(string id, string receipt, List<OrderItemDto> orderItems);

        bool InsertExpeditionAdjustment(string id, string name);
        bool UpdateExpeditionAdjustment(string id, string name, List<OrderItemDto> orderItems);
    }
}
