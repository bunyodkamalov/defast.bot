using Newtonsoft.Json;

namespace Defast.Bot.Domain.Common.Entities;

public abstract class Entity : IEntity
{
    [JsonIgnore]
    public Guid Id { get; set; }
}