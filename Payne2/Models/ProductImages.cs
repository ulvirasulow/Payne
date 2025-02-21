using Payne.Models.Common;

namespace Payne.Models;

public class ProductImages : BaseEntity
{
    public string ImgUrl { get; set; }
    public bool Primary { get; set; }
    public Product Product { get; set; }
    public int ProductId { get; set; }
}