using Kinashy.ArrestSearchWebAPI.Data.DTO;
using System.Net;
using System.Runtime.CompilerServices;

namespace Kinashy.ArrestSearchWebAPI.Data.Library
{
    public interface ILibraryRepository : IDisposable
    {
        //Task<List<BatchProperty>> GetBatchPropertiesAsync();
        //Task<List<Batch>> GetBatchesAsync(int countSkip, int countTake); // default order - desc.
        Task<List<string>> GetComplectPropertyNamesAsync();
        Task<List<Batch>> GetLightBatchesAsync(int countSkip, int countTake); // Light - without complects, default order - desc.
        Task InsertBatchAsync(Batch batch);
        Task<List<Complect>> FindComplectsAsync(List<PropertyDTO> complectProperties,
            int countSkip,
            int countTake,
            bool searchByOccurrences = false,
            bool searchByDate = false,
            string dateStart = null,
            string dateEnd = null);
        void UpdateComplect(Complect complect);
        void UpdateBatch(Batch batch);
        Task<Batch> GetBatchAsync(int batchId, bool onlyReleased);
        Task<Complect> GetComplectAsync (int complectId);
        Task<List<RequiredProperties.RequiredProperty>> GetRequiredPropertiesAsync();
        Task SetRequiredPropertiesAsync(List<RequiredProperties.RequiredProperty> requiredProperties);
        Task SaveAsync();
    }
}