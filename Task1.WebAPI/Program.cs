
using Microsoft.EntityFrameworkCore;
using Task1.BLL.Helper.Mapper;
using Task1.BLL.Services.Implements;
using Task1.BLL.Services.Interfaces;
using Task1.DAL;
using Task1.DAL.Repositories;
using Task1.Util.Queries;

namespace Task1.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
				options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
			}); ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            //set up policy
            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("corspolicy", build =>
                {
                    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
                });
            });

            //set up DB
            builder.Services.AddDbContext<PubsContext>(options => {
                options.UseSqlServer(builder.Configuration.GetConnectionString("pubs"));
            });

            //AutoMapper
            builder.Services.AddAutoMapper(typeof(MapperProfile));

            //set repo base
            builder.Services.AddScoped(typeof(IRepoBase<>), typeof(RepoBase<>));

            //set Unit Of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Service
            builder.Services.AddScoped<IStoreService, StoreService>();
            builder.Services.AddScoped<ITitleService, TitleService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("corspolicy");
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
