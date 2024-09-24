using Domain.Write.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ProductController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateProduct() 
    {
        var productId = Guid.NewGuid();
        var command = new CreateProductCommand(productId, "product1", 100);

        await _mediator.Send(command);

        return Ok(productId);
    }
}
