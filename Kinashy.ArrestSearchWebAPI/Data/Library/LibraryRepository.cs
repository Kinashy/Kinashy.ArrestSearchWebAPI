using Kinashy.ArrestSearchWebAPI.Data.DTO;
using Kinashy.ArrestSearchWebAPI.Data.RequiredProperties;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Numerics;
using System.Text;

namespace Kinashy.ArrestSearchWebAPI.Data.Library
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly LibraryDB _library;
        public LibraryRepository(LibraryDB library)
        {
            _library = library;
        }

        public async Task<List<Complect>> FindComplectsAsync(List<PropertyDTO> complectProperties, 
            int countSkip,
            int countTake,
            bool searchByOccurrences = false,
            bool searchByDate = false, 
            string dateStart = null,
            string dateEnd = null)
        {
            List<Complect> findedComplects = new();
            if (complectProperties == null)
                return null;
            if (complectProperties.Count < 1)
                return null;
            StringBuilder sqlBuilder = new();
            sqlBuilder.Append("SELECT * FROM DOLEG.\"ComplectsProperty\" WHERE");
                    List<OracleParameter> complectParams = new List<OracleParameter>();
                    for (int i = 0; i < complectProperties.Count; i++)
                    {
                        complectParams.Add(new OracleParameter($"name{i + 1}", complectProperties[i].Name));
                        complectParams.Add(new OracleParameter($"val{i + 1}", searchByOccurrences ? $"%{complectProperties[i].Value}%" : $"{complectProperties[i].Value}"));
                        sqlBuilder.Append($"(\"Name\" = :name{i + 1} AND lower(\"Value\") like lower(:val{i + 1}))");
                        if (i < complectProperties.Count - 1)
                        {
                            sqlBuilder.Append(" OR ");
                        }
                    }
                    var qc = _library.ComplectsProperty.FromSqlRaw(sqlBuilder.ToString(), parameters: complectParams.ToArray());
                    IQueryable<Complect> queryComplect;
                    if (searchByDate && dateStart is not null && dateEnd is not null)
                    {
                        queryComplect = qc
                       .Where(t => t.Complect.Properties.FirstOrDefault(t => (t.Name.ToLower().IndexOf("дата") > -1) ? 
                       (LibraryDB.ToDate(t.Value, "dd.mm.yyyy") <= LibraryDB.ToDate(dateEnd, "dd.mm.yyyy") && 
                       LibraryDB.ToDate(t.Value, "dd.mm.yyyy") >= LibraryDB.ToDate(dateStart, "dd.mm.yyyy")) : false) != null)
                       .Include(t => t.Complect).ThenInclude(t => t.Properties)
                       .Select(t => t.Complect)
                       .OrderBy(c => c.Id).Distinct().Skip(countSkip).Take(countTake);
                    }
                    else
                    {
                        queryComplect = qc.Include(t => t.Complect).ThenInclude(t => t.Properties).Select(t => t.Complect).OrderBy(c => c.Id).Distinct().
                            Skip(countSkip).Take(countTake);
                    }
                    sqlBuilder.Clear();
            return await queryComplect.ToListAsync();
        }

        public async Task<Batch> GetBatchAsync(int batchId, bool onlyReleased) => 
            await _library.Batches.Include(b => b.Properties).Include(b => b.Complects).ThenInclude(c => c.Properties).FirstOrDefaultAsync(b => b.Id == batchId && (b.IsReleased || !onlyReleased));

        public async Task<List<string>> GetComplectPropertyNamesAsync() => 
            await _library.ComplectsProperty.Where(p => p.Complect.IsReadyToDownload).Select(p => p.Name).Distinct().ToListAsync();

        public async Task<List<Batch>> GetLightBatchesAsync(int countSkip, int countTake) => await _library.Batches.Where(b => b.IsReleased)
            .OrderBy(o => o.Id).Skip(countSkip).Take(countTake) 
            .Include(p => p.Properties)
            .Include(c => c.Complects).ThenInclude(p => p.Properties).ToListAsync();

        public async Task InsertBatchAsync(Batch batch) => 
            await _library.Batches.AddAsync(batch);
        public async Task SaveAsync() =>
            await _library.SaveChangesAsync();

        public void UpdateBatch(Batch batch) =>
            _library.Batches.Update(batch);

        public void UpdateComplect(Complect complect) =>
             _library.Complects.Update(complect);

        public async Task<Complect> GetComplectAsync(int complectId) => await _library.Complects.FirstOrDefaultAsync(c => c.Id == complectId);
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _library.Dispose();
                }
            }
            _disposed = true;
        }
        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<List<RequiredProperty>> GetRequiredPropertiesAsync()
        {
            return await _library.RequiredProperties.ToListAsync();
        }

        public async Task SetRequiredPropertiesAsync(List<RequiredProperty> requiredProperties)
        {
            await _library.RequiredProperties.ExecuteDeleteAsync();
            await _library.RequiredProperties.AddRangeAsync(requiredProperties);
        }
    }
}
