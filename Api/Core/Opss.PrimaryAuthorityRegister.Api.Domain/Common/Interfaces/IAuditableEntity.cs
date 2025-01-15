using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Common.Interfaces;

public interface IAuditableEntity : IEntity
{
    DateTime? CreatedDate { get; set; }
    DateTime? UpdatedDate { get; set; }
}
