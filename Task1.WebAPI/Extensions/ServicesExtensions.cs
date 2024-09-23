using System.Reflection;
using Task1.BLL.Helper.Mapper;
using Task1.DAL.Repositories;

namespace Task1.WebAPI.Extensions
{
	public static class ServicesExtesions
	{
		//Unit Of Work
		public static void AddUnitOfWork(this IServiceCollection services)
		{
			services.AddScoped<IUnitOfWork, UnitOfWork>();
		}

		//RepoBase
		public static void AddRepoBase(this IServiceCollection services)
		{
			services.AddScoped(typeof(IRepoBase<>), typeof(RepoBase<>));
		}

		//BLL Services
		public static void AddBLLServices(this IServiceCollection services)
		{
			services.Scan(scan => scan
					.FromAssemblies(Assembly.Load("Task1.BLL"))
					.AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
					.AsImplementedInterfaces()
					.WithScopedLifetime());
		}

		public static void AddMapper(this IServiceCollection services)
		{
			services.AddAutoMapper(typeof(MapperProfile));
		}
	}
}
