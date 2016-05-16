using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Animart.Portal.Shipment.Dto;

namespace Animart.Portal.Shipment
{
    public class ShipmentService : PortalAppServiceBase, IShipmentService
    {
        private readonly IRepository<ShipmentCost, Guid> _shipmentRepository;
        private readonly IRepository<City, Guid> _cityRepository;
        private readonly IRepository<Users.User, long> _userRepository;
        private readonly IRepository<Expedition, Guid> _expeditionRepository;

        public ShipmentService(IRepository<ShipmentCost, Guid> ShipmentRepository, IRepository<City, Guid> CityRepository,
            IRepository<Users.User, long> UserRepository, IRepository<Expedition, Guid> ExpeditionRepository)
        {
            _shipmentRepository = ShipmentRepository;
            _cityRepository = CityRepository;
            _userRepository = UserRepository;
            _expeditionRepository = ExpeditionRepository;
        }

        public async Task Create(ShipmentCostDto shipmentItem)
        {
            var id = Guid.Parse(shipmentItem.City);
            var city = _cityRepository.FirstOrDefault(e => e.Id == id);
            var item = new ShipmentCost
            {
                City = city ,
                CreationTime = DateTime.Now,
                CreatorUser = _userRepository.Get(AbpSession.GetUserId()),
                CreatorUserId = AbpSession.GetUserId(),
                Expedition = shipmentItem.Expedition,
                Type = shipmentItem.Type,
                First5Kilo = shipmentItem.First5Kilo,
                NextKilo = shipmentItem.NextKilo
            };

            await _shipmentRepository.InsertAsync(item);
        }

        public async Task Update(ShipmentCostDto shipmentItem)
        {
            var editItem = _shipmentRepository.Get(shipmentItem.Id);
            editItem.City = _cityRepository.FirstOrDefault(e => e.Name == shipmentItem.City);
            editItem.Expedition = shipmentItem.Expedition;
            editItem.Type = shipmentItem.Type;

            await _shipmentRepository.UpdateAsync(editItem);
        }

        public async Task Delete(Guid shipmentId)
        {
            await _shipmentRepository.DeleteAsync(shipmentId);
        }

        public List<ShipmentCostDto> GetShipmentCosts()
        {
            return _shipmentRepository.GetAllList().Select(e => new ShipmentCostDto()
            {
                City = e.City.Name,
                CreationTime = e.CreationTime,
                CreatorUserId = e.CreatorUserId,
                Expedition = e.Expedition,
                First5Kilo = e.First5Kilo,
                NextKilo = e.NextKilo,
                Type = e.Type,
                Id = e.Id
            }).ToList();
        }

        public List<ShipmentCostDto> GetShipmentCostFilterByExpedition(string name)
        {
            return _shipmentRepository.GetAllList().Where(e => e.Expedition == name).Select(e => new ShipmentCostDto()
            {
                City = e.City.Name,
                CreationTime = e.CreationTime,
                CreatorUserId = e.CreatorUserId,
                Expedition = e.Expedition,
                First5Kilo = e.First5Kilo,
                NextKilo = e.NextKilo,
                Type = e.Type,
                Id = e.Id
            }).ToList();
        }

        public List<ShipmentCostDto> GetShipmentCostFilterByExpeditionAndCity(string expeditionName, string city)
        {
            var _expedition = expeditionName.Trim();
            var _city = city.Trim();

            var result =  _shipmentRepository.GetAllList().Where(e => e.Expedition == _expedition && e.City.Name == _city).Select(e => new ShipmentCostDto()
            {
                City = e.City.Name,
                CreationTime = e.CreationTime,
                CreatorUserId = e.CreatorUserId,
                Expedition = e.Expedition,
                First5Kilo = e.First5Kilo,
                NextKilo = e.NextKilo,
                Type = e.Type,
                Id = e.Id

            }).ToList();

            return result;
        }

        public async Task CreateCity(string name)
        {
            await _cityRepository.InsertAsync(new City()
            {
                Name = name,
                CreationTime = DateTime.Now,
                CreatorUser = _userRepository.Get(AbpSession.GetUserId()),
                CreatorUserId = AbpSession.GetUserId()
            });
        }

        public async Task UpdateCity(CityOptionsDto cityItem)
        {
            var item = _cityRepository.GetAll().First(e=>e.Id == cityItem.Id);
            item.Name = cityItem.Name;
            await _cityRepository.UpdateAsync(item);
        }

        public bool DeleteCity(Guid id)
        {
            try
            {
                var item = _cityRepository.FirstOrDefault(e => e.Id == id);
                _cityRepository.Delete(item);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }

        public List<CityOptionsDto> GetCities()
        {
            var result = _cityRepository.GetAll().Select(e => new CityOptionsDto()
            {
                Id = e.Id,
                Name = e.Name
            }).ToList();

            return result;
        }

        public List<CityOptionsDto> GetCityFilterByName(string name)
        {
            return _cityRepository.GetAll().Where(e=>e.Name == name).Select(e => new CityOptionsDto()
            {
                Id = e.Id,
                Name = e.Name
            }).ToList();
        }
    }
}
