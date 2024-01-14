namespace Kinashy.ArrestSearchWebAPI.Data.Library
{
    public class ComplectProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int? IdComplect { get; set; }
        public Complect Complect { get; set; }
    }
}