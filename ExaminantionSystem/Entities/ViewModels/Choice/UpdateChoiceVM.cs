using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.ViewModels.Choice
{
    public class UpdateChoiceVM
    {
        [Required]
        public int ChoiceId { get; set; }

        [Required]
        public string Text { get; set; }

        public bool IsCorrect { get; set; }
    }
}
