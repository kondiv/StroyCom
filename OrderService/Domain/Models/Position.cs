namespace OrderService.Domain.Models;

public class Position
{
    public required string Item { get; set; }
    
    public required int Quantity { get; set; }
}
