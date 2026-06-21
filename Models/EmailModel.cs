using System.ComponentModel.DataAnnotations;

namespace Trabalho.Models
{
    public class EmailModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Para")]
        public string Para { get; set; }

        [Required]
        [Display(Name = "Assunto")]
        public string Assunto { get; set; }

        [Required]
        [Display(Name = "Mensagem")]
        public string Mensagem { get; set; }
    }
}