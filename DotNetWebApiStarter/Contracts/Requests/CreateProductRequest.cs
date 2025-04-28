using System.ComponentModel.DataAnnotations;

namespace DotNetWebApiStarter.Contracts.Requests
{
    public class CreateProductRequest
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
    }
}
