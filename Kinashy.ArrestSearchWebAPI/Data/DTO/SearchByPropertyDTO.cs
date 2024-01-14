namespace Kinashy.ArrestSearchWebAPI.Data.DTO
{
    public class SearchByPropertyDTO
    {
        public List<PropertyDTO> BatchProperties { get; set; }
        public List<PropertyDTO> ComplectProperties { get; set; }
        public bool IsSearchByDate { get; set; }
        public bool IsSearchByOccurrence { get; set; }
        public string? DateBegin { get; set; }
        public string? DateEnd { get; set; }
        public int CountSkip {get; set;}
        public int CountTake { get; set; }
    }
}
