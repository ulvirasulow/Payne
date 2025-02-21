using System.ComponentModel.DataAnnotations.Schema;
using Payne.Models.Common;

namespace Payne.Models;

public class Slider : BaseEntity
{
    public string Title { get; set; }
    public string ImgUrl { get; set; }
    public string SubTitle { get; set; }
    public string Description { get; set; }
    [NotMapped] public IFormFile File { get; set; }
}