using Framework.Core;

namespace Domain.Write.Commands;

public class UpdateProductPriceCommand(Guid id, decimal price) : ICommand
{
    public Guid Id { get; } = id;
    public decimal Price { get; } = price;
}