using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceRESTfulAPI.Application.DTOs
{
    public class CategoryCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<CategoryLocalizationDto> Localizations { get; set; } = new List<CategoryLocalizationDto>();
    }

}
