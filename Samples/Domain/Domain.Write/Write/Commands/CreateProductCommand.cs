using Framework.Core;

namespace Domain.Write.Commands;

public class CreateProductCommand(Guid id, string name, int price) : ICommand
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public int Price { get; } = price;
}