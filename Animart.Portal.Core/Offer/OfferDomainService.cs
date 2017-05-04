using Abp.Domain.Repositories;
using Abp.Domain.Services;

namespace Animart.Portal.Offer
{
    public class OfferDomainService:DomainService
    {
        private readonly IRepository<Offer> _offerRepository;

        public OfferDomainService(IRepository<Offer> offerRepository)
        {
            _offerRepository = offerRepository;
        }
    }
}
