using Domain.Read;
using Domain.Read.Queries;
using Domain.Write.Commands;
using Domain.Write.Commands.Handlers;
using Framework.Core;
using Microsoft.AspNetCore.Mvc;

namespace SampleApp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ICommandHandler<CreateProductCommand> createProductCommandHandler;
        private readonly ICommandHandler<UpdateProductPriceCommand> updateProductPriceCommandHandler;
        private readonly ICommandHandler<DeleteProductCommand> deleteProductCommandHandler;

        private readonly IQueryHandler<GetLowPricesProducts, IEnumerable<ProductReadModel>> queryHandler;

        public ProductController(
            ICommandHandler<CreateProductCommand> createProductCommandHandler,
            ICommandHandler<UpdateProductPriceCommand> updateProductPriceCommandHandler,
            ICommandHandler<DeleteProductCommand> deleteProductCommandHandler,
            IQueryHandler<GetLowPricesProducts, IEnumerable<ProductReadModel>> queryHandler)
        {
            this.createProductCommandHandler = createProductCommandHandler;
            this.updateProductPriceCommandHandler = updateProductPriceCommandHandler;
            this.deleteProductCommandHandler = deleteProductCommandHandler;
            this.queryHandler = queryHandler;
        }

        [HttpPost]
        public IActionResult CreateProduct() 
        {
            var productId = Guid.NewGuid();
            var command = new CreateProductCommand(productId, "product1", 100);

            createProductCommandHandler.Handle(command);

            return Ok(productId);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var query = new GetLowPricesProducts(100);
            var result = queryHandler.Handle(query);

            return Ok(result);
        }
    }
}
