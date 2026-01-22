using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface ItemValidating
    {
        public string? ImportId { get; set; }
        List<string> GetValidators();
        string GetCardPartial();
    }
}
