﻿namespace DotNetWebApiStarter.Contracts.Responses
{
    public class GetProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
