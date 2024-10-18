using Stickers.Common.DataAccess;
using Stickers.DataAccess.InMemory;

namespace Stickers.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers(options =>
        {
            options.InputFormatters.Insert(0, Utils.GetJsonPatchInputFormatter());
            options.SuppressAsyncSuffixInActionNames = false;
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<ISimplifiedDataAccessor, InMemoryDataAccessor>();
        builder.Services.AddRouting(options =>
        {
            options.LowercaseUrls = true;//url全小写
            options.LowercaseQueryStrings = true;//查询字串全小写
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}