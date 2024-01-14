namespace Kinashy.ArrestSearchWebAPI.Data.Library
{
    public class Batch
    {
        public int Id { get; set; }
        public ICollection<Complect> Complects { get; set; } = new List<Complect>();
        public ICollection<BatchProperty> Properties { get; set; } = new List<BatchProperty>();
        public bool IsReleased { get; set; } = false;
        public DateTime DateUpload { get; set; } = DateTime.Now;
    }
}