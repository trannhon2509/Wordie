using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.Common.Models;

public class LookupDto
{
    public int Id { get; init; }

    public string? Title { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            // Legacy Todo mappings removed.
        }
    }
}
