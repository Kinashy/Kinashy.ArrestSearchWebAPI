namespace Kinashy.ArrestSearchWebAPI.Data.DTO
{
    public class ComplectDTO
    {
        public int? Id { get; set; }
        public string DocumentPath { get; set; }
        public List<PropertyDTO> Properties { get; set; } = new();
        public DateTime? DateCreated { get; set; }
    }
}
