using Payne.Models.Common;

namespace Payne.Models;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public int Weight { get; set; }
    public int Dimension { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public string SKU { get; set; }
    public List<TagProduct>? TagProducts { get; set; }
    public List<ProductImages> ProductImages { get; set; }
    public ICollection<Review> Reviews { get; set; }
}