using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Animart.Portal.Supply.Dto;

namespace Animart.Portal.Supply
{
    public interface ISupplyService:IApplicationService
    {
        PagedResultOutput<SupplyItem> GetSupplyByName(GetSupplyByNameInput Input);

        Task Create(SupplyItemDto supplyItem);

        bool Update(SupplyItemDto supplyItem);

        bool Delete(Guid id);

        bool UpdateStatus(Guid id);

        bool UpdateInStock(Guid id, int stock);

        SupplyItem GetSupplyItemById(Guid id);

        SupplyItem GetSingleByName(string name);

    }
}
