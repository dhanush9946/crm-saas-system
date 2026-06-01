using CRM.Domain.Common;

namespace CRM.Application.Common.Interfaces;

public interface IConcurrencyService
{
    void SetOriginalRowVersion(
        BaseEntity entity,
        byte[] rowVersion);
}