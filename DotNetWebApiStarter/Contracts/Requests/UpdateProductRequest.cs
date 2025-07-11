namespace DotNetWebApiStarter.Contracts.Requests
{
    public class UpdateProductRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
