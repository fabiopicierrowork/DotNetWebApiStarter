using System.ComponentModel.DataAnnotations;

namespace DotNetWebApiStarter.DTOs.Requests
{
    public class InsertProductRequestDTO
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Product price is mandatory.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }
    }
}
