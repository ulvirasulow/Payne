using Payne.Models.Common;

namespace Payne.Models
{
    public class Tags : BaseEntity
    {
        public string Name { get; set; }
        public List<TagProduct> TagProducts { get; set; }
    }
}