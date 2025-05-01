using System.ComponentModel.DataAnnotations;

namespace ClienteApi.Core.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; set; }

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

        [MaxLength(100)]
        public string Logradouro { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Bairro { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Cidade { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Estado { get; set; } = string.Empty;
    }
}