namespace Kinashy.ArrestSearchWebAPI.Data.Library
{
    public class Complect
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public bool IsReadyToDownload { get; set; } = false;
        public ICollection<ComplectProperty> Properties { get; set; } = new List<ComplectProperty>();
        public DateTime? DateUpload { get; set; } = DateTime.Now;
        public int? BatchId { get; set; }
        public Batch Batch { get; set; }
    }
}