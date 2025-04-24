using System.ComponentModel.DataAnnotations;

namespace DotNetWebApiStarter.DTOs.Requests
{
    public class InsertProductRequestDTO
    {
        [Required(ErrorMessage = "Il nome del prodotto è obbligatorio.")]
        [StringLength(100, ErrorMessage = "Il nome del prodotto non può superare i 100 caratteri.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Il prezzo del prodotto è obbligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Il prezzo deve essere maggiore di zero.")]
        public decimal Price { get; set; }
    }
}
