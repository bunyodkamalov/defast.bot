using Newtonsoft.Json;

namespace Defast.Bot.Domain.Entities.Common;

public class Response<T> where T : class
{
    [JsonProperty("@odata.context")]
    public string ODataContext { get; set; }
    
    [JsonProperty("value")]
    public List<T>? Value { get; set; }

}
