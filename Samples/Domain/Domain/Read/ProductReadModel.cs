﻿using Framework.Core;

namespace Domain.Read;

public class ProductReadModel : IReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public override string? ToString()
    {
        return $"Id: {Id}, Name: {Name}, Price {Price}";
    }
}