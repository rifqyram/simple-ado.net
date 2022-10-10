namespace SimpleADONet.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } 
    public int Price { get; set; }
    public int Stock { get; set; }

    public override string ToString()
    {
        return $"{nameof(ProductId)}: {ProductId}, {nameof(Name)}: {Name}, {nameof(Price)}: {Price}, {nameof(Stock)}: {Stock}";
    }
}