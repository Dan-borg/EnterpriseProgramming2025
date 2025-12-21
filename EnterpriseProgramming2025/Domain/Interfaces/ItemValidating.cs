using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface ItemValidating
    {
        List<string> GetValidators();
        string GetCardPartial();
    }
}
