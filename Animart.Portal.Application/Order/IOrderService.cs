﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Animart.Portal.Order.Dto;

namespace Animart.Portal.Order
{
    public interface IOrderService: IApplicationService
    {
        Task Create(Dto.CreatePurchaseOrderDto purchaseOrderrderItem);

        bool Update(Dto.OrderItemInputDto orderItem);

        bool Delete(string id);

        IEnumerable<PurchaseOrder> GetAllPurchaseOrderByUserId(int id);
        OrderDashboardDto GetDashboard();

        PurchaseOrder GetSinglePurchaseOrder(string id);

        bool AddOrderItem(string id, Dto.OrderItemInputDto orderItem);

        bool UpdateOrderItem(string id, Dto.OrderItemInputDto orderItem);

        bool DeleteOrderItem(string id, string orderItemId);



    }
}
