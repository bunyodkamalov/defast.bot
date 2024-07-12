using Defast.Bot.Domain.Common.Entities;

namespace Defast.Bot.Domain.Entities.Common;

public class Сashbox : Entity
{
    public string AcctName { get; set; } = default!;

    public string ContraAct { get; set; } = default!;
    
    public decimal Credit { get; set; }
    
    public decimal Debit { get; set; }
    
    public string DueDate { get; set; }
    
    public string LineMemo { get; set; }
    
}