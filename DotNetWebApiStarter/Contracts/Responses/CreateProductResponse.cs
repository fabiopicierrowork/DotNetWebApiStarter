﻿namespace DotNetWebApiStarter.Contracts.Responses
{
    public class CreateProductResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
    }
}
