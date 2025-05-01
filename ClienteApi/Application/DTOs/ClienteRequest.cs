using System.ComponentModel.DataAnnotations;

namespace ClienteApi.Application.DTOs
{
    public class ClienteRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        [MaxLength(9)]
        public string Cep { get; set; } = string.Empty;
    }
}