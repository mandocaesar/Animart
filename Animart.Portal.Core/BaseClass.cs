using System;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Users;

namespace Animart.Portal
{
    public abstract class BaseClass:CreationAuditedEntity<int, User>
    {

        public DateTime? CreatedOn { get; set; }
        public User CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public User ModifiedBy { get; set; }
    }
}
