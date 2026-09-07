using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.ViewModels.PlanViewModels
{
    public class UpdatePlanViewModel
    {
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Duration is required!")]
        [Range(1,365, ErrorMessage = "Duration must be between 1 and 365 days!")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Price is required!")]
        [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0!")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 200 characters!")]
        public string Description { get; set; } = default!;
    }
}
