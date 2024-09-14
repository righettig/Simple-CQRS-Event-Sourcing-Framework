using Domain.Read.Queries;
using Domain.Write.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SampleApp.Web.Controllers;

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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var query = new GetLowPricesProducts(100);

        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
