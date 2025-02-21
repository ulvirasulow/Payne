using Payne.Models.Common;

namespace Payne.Models;

public class Review : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }
    public string Comment { get; set; }
    public double Rating { get; set; }
    public DateTime Date { get; set; }
}