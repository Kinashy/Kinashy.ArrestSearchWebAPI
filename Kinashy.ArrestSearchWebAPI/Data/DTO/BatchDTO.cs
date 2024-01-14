namespace Kinashy.ArrestSearchWebAPI.Data.DTO
{
    public class BatchDTO
    {
        public int? Id { get; set; }
        public List<PropertyDTO> Properties { get; set; } = new();
        public List<ComplectDTO> Complects { get; set; } = new();
        public DateTime? DateCreated { get; set; }
    }
}