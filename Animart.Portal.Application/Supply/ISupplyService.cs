using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Web.Models;
using Animart.Portal.Supply.Dto;

namespace Animart.Portal.Supply
{
    public interface ISupplyService:IApplicationService
    {
        [DontWrapResult]
        List<SupplyItemDto> GetSupplies();
        [DontWrapResult]
        List<SupplyItemDto> GetSuppliesByCategoryId(Guid id);
        [DontWrapResult]
        SuppliesDTO GetSuppliesRetailer();
        [DontWrapResult]
        SuppliesDTO GetSuppliesRetailerByCategoryId(Guid id);
        [DontWrapResult]
        PagedResultOutput<SupplyItem> GetSupplyByName(GetSupplyByNameInput Input);
        [DontWrapResult]
        Task Create(SupplyItemDto supplyItem);
        [DontWrapResult]
        bool Update(SupplyItemDto supplyItem);
        [DontWrapResult]
        bool Delete(Guid id);
        [DontWrapResult]
        bool UpdateStatus(Guid id);
        [DontWrapResult]
        bool UpdateInStock(Guid id, int stock);

        [DontWrapResult]
        ItemAvailableDto IsAvailable(Guid id,int idx);

        [DontWrapResult]
        SupplyItemDto Supply(Guid id);
        [DontWrapResult]
        SupplyItem GetSingleByName(string name);

        [DontWrapResult]
        bool SaveImage(Guid supplyItemId);

    }
}
