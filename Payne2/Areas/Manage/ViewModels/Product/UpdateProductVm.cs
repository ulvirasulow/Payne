namespace Payne.Areas.Manage.ViewModels.Product;

public class UpdateProductVm
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public string Description { get; set; }
    public int Weight { get; set; }
    public int Dimension { get; set; }
    public string SKU { get; set; }
    public List<int>? TagIds { get; set; }
    public IFormFile? Photo { get; set; }
    public List<IFormFile>? Images { get; set; }
    public List<ProductImageVm>? productImages { get; set; }
    public List<string>? ImagesUrls { get; set; }
}

public record ProductImageVm
{
    public bool Primary { get; set; }
    public string ImgUrl { get; set; }
}