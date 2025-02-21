using Payne.Models.Common;

namespace Payne.Models
{
    public class TagProduct:BaseEntity
    {
        public int TagId { get; set; }
        public Tags Tag { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
