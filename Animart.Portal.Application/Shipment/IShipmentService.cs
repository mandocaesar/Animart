using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Animart.Portal.Shipment.Dto;

namespace Animart.Portal.Shipment
{
    public interface IShipmentService:IApplicationService
    {
        Task Create(ShipmentCostDto shipmentItem);
        Task Update(ShipmentCostDto shipmentItem);
        Task Delete(Guid shipmentId);
        List<ShipmentCostDto> GetShipmentCosts();
        List<ShipmentCostDto> GetShipmentCostFilterByExpedition(string name);

        Task CreateCity(string name);
        Task UpdateCity(CityOptionsDto cityItem);
        bool DeleteCity(Guid id);
        List<CityOptionsDto> GetCities();
        List<CityOptionsDto> GetCityFilterByName(string name);
    }
}
