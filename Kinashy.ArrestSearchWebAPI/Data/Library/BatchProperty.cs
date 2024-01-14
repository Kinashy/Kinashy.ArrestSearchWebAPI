namespace Kinashy.ArrestSearchWebAPI.Data.Library
{
    public class BatchProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int? IdBatch { get; set; }
        public Batch Batch { get; set; }
    }
}