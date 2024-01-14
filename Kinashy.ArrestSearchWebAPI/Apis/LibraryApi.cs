using AutoMapper;
using Kinashy.ArrestSearchWebAPI.Data;
using Kinashy.ArrestSearchWebAPI.Data.DTO;
using Kinashy.ArrestSearchWebAPI.Data.Library;
using Kinashy.ArrestSearchWebAPI.Data.RequiredProperties;
using Samba;
using System.Net;

namespace Kinashy.ArrestSearchWebAPI.Apis
{
    public class LibraryApi : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapGet("/Complect/Property/GetAll", GetAllComplectsPropertyNames);

            app.MapGet("/Batch/Minimal/GetAll/{countSkip}/{countTake}", GetAllBatches);
            app.MapPost("/Batch/Add", BatchAdd);
            app.MapPost("Complect/SearchByProperty", SearchComplectByProperty);
            app.MapPost("/Complect/{id}/AddRaw", [Authorize] async (int id, HttpRequest request, [FromServices] ILibraryRepository repository, [FromServices] IMapper mapper) =>
            {
                var complect = await repository.GetComplectAsync(id);
                if (complect == null)
                    return Results.NotFound($"/Complect/{id}/AddRaw");
                MemoryStream memoryStream = new();
                var stream = request.BodyReader.AsStream();
                await stream.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray();
                memoryStream.Close();
                await memoryStream.DisposeAsync();
                Samba.SambaConnection sambaConnection = new Samba.SambaConnection()
                {
                    Domain = app.Configuration.GetSection("Samba").GetSection("Domain").Value,
                    IP = IPAddress.Parse(app.Configuration.GetSection("Samba").GetSection("Server").Value),
                    Name = app.Configuration.GetSection("Samba").GetSection("Login").Value,
                    Password = app.Configuration.GetSection("Samba").GetSection("Password").Value
                };
                SambaHelper sambaHelper = new SambaHelper();
                await sambaHelper.WriteToFileAsync(sambaConnection, app.Configuration.GetSection("Samba").GetSection("Share").Value, complect.ImagePath, fileContent);
                complect.IsReadyToDownload = true;
                repository.UpdateComplect(complect);
                await repository.SaveAsync();
                Batch batch = await repository.GetBatchAsync(complect.BatchId.Value, false);
                Complect complectNotReady = batch.Complects.FirstOrDefault(c => !c.IsReadyToDownload);
                if (complectNotReady is not null)
                {
                    batch.IsReleased = false;
                }
                else
                {
                    batch.IsReleased = true;
                }
                repository.UpdateBatch(batch);
                await repository.SaveAsync();
                GC.Collect();
                return Results.Ok($"/Complect/{id}/AddRaw/");
            });
            app.MapGet("/Complect/{id}/GetRaw", [Authorize] async (int id, [FromServices] ILibraryRepository repository, [FromServices] IMapper mapper) =>
            {
                var complect = await repository.GetComplectAsync(id);
                if (complect == null)
                    return Results.NotFound($"/Complect/{id}/GetRaw");
                Samba.SambaConnection sambaConnection = new Samba.SambaConnection()
                {
                    Domain = app.Configuration.GetSection("Samba").GetSection("Domain").Value,
                    IP = IPAddress.Parse(app.Configuration.GetSection("Samba").GetSection("Server").Value),
                    Name = app.Configuration.GetSection("Samba").GetSection("Login").Value,
                    Password = app.Configuration.GetSection("Samba").GetSection("Password").Value
                };
                SambaHelper sambaHelper = new SambaHelper();
                var data = await sambaHelper.ReadFileAsync(sambaConnection, app.Configuration.GetSection("Samba").GetSection("Share").Value, complect.ImagePath);
                GC.Collect();
                return Results.Extensions.Pdf<byte[]>(data);
            });
            app.MapGet("/Batch/{id}/Get", GetBatchById);
            app.MapGet("/RequiredProperties/GetAll", GetRequiredProperties);
            app.MapPost("/RequiredProperties/Set", SetRequiredProperties);
        }
        [Authorize]
        private async Task<IResult> GetAllComplectsPropertyNames([FromServices] ILibraryRepository repository)
        {
            return Results.Json(await repository.GetComplectPropertyNamesAsync());
        }
        [Authorize]
        private async Task<IResult> GetAllBatches(int countSkip, int countTake, ILibraryRepository repository, [FromServices] IMapper mapper)
        {
            List<BatchMinimalDTO> batchDTOs = new();
            var batches = await repository.GetLightBatchesAsync(countSkip, countTake);
            if (batches != null)
                batchDTOs = mapper.Map<List<Batch>, List<BatchMinimalDTO>>(batches);
            return Results.Json(batchDTOs);
        }
        [Authorize]
        private async Task<IResult> BatchAdd([FromBody] BatchDTO document, [FromServices] ILibraryRepository repository, [FromServices] IMapper mapper)
        {
            Batch batch = new Batch();
            batch = mapper.Map<BatchDTO, Batch>(document);
            await repository.InsertBatchAsync(batch);
            await repository.SaveAsync();
            BatchDTO batchDTO = new BatchDTO();
            batchDTO = mapper.Map<Batch, BatchDTO>(batch);
            return Results.Created($"/Batch/{batch.Id}", batchDTO);
        }
        [Authorize]
        private async Task<IResult> SearchComplectByProperty(SearchByPropertyDTO sProp, HttpRequest request, [FromServices] ILibraryRepository repository, [FromServices] IMapper mapper)
        {
            var findedComplects = await repository.FindComplectsAsync(sProp.ComplectProperties, sProp.CountSkip, sProp.CountTake, sProp.IsSearchByOccurrence, sProp.IsSearchByDate, sProp.DateBegin, sProp.DateEnd);
            List<ComplectDTO> complectsDTO = new();
            if (findedComplects.Count > 0)
                complectsDTO = mapper.Map<List<Complect>, List<ComplectDTO>>(findedComplects);
            return Results.Json(complectsDTO);
        }
        [Authorize]
        private async Task<IResult> GetBatchById(int id, [FromServices] ILibraryRepository repository, [FromServices] IMapper mapper)
        {
            var b = await repository.GetBatchAsync(id, true);
            if (b == null)
                return Results.NotFound($"/Batch/{id}/Get");
            var batchDTO = mapper.Map<Batch, BatchDTO>(b);
            return Results.Json(batchDTO);
        }
        [Authorize]
        private async Task<IResult> GetRequiredProperties([FromServices] ILibraryRepository repository, [FromServices] IMapper mapper)
        {
            var rp = await repository.GetRequiredPropertiesAsync();
            var res = mapper.Map<List<RequiredProperty>, List<PropertyDTO>>(rp);
            return Results.Json(res);
        }
        [Authorize]
        private async Task<IResult> SetRequiredProperties([FromBody] SetRequiredDTO properties, [FromServices] ILibraryRepository repository, [FromServices] IMapper mapper)
        {
            List<RequiredProperty> requiredProperties = new List<RequiredProperty>();
            foreach (var property in properties.BatchProperties)
            {
                requiredProperties.Add(new RequiredProperty() { IsForComplect = false, Name = property.Name, Value = property.Value });
            }
            foreach (var property in properties.ComplectProperties)
            {
                requiredProperties.Add(new RequiredProperty() { IsForComplect = true, Name = property.Name, Value = property.Value });
            }
            repository.SetRequiredPropertiesAsync(requiredProperties);
            repository.SaveAsync();
            return Results.Ok("RequiredProperties/Set");
        }
    }
}