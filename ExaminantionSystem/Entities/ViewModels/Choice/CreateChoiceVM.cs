using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.ViewModels.Choice
{
    public class CreateChoiceVM
    {
        [Required] 
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}
