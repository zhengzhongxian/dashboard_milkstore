using Dashboard_MilkStore.Models.Common;

namespace Dashboard_MilkStore.Models.Parent
{
    public class ParentResponse
    {
        public List<Parent> Items { get; set; } = new List<Parent>();
        public PaginationMetadata? Metadata { get; set; }
    }
}
