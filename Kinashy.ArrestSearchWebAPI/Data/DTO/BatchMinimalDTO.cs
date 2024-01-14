namespace Kinashy.ArrestSearchWebAPI.Data.DTO
{
    public class BatchMinimalDTO
    {
        public int? Id { get; set; }
        public List<PropertyDTO> Properties { get; set; } = new();
        public int ComplectCount { get; set; } = new();
        public DateTime? DateCreated { get; set; }
    }
}
