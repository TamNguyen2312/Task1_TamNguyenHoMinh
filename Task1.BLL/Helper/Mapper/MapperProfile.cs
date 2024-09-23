using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.EmployeeDTOs;
using Task1.BLL.DTOs.StoreDTOs;
using Task1.BLL.DTOs.TitleDTOs;
using Task1.DAL.Entities;

namespace Task1.BLL.Helper.Mapper
{
    public class MapperProfile : Profile
    {
		public MapperProfile()
		{
			var dalAssembly = Assembly.Load("Task1.DAL");
			var bllAssembly = Assembly.Load("Task1.BLL");


			var entityTypes = dalAssembly.GetTypes().Where(t => t.IsClass && t.Namespace == "Task1.DAL.Entities");


			foreach (var entityType in entityTypes)
			{
				var dtoTypes = bllAssembly.GetTypes()
					.Where(t => t.IsClass && t.Namespace == $"Task1.BLL.DTOs.{entityType.Name}DTOs" && t.Name.StartsWith(entityType.Name));

				foreach (var dtoType in dtoTypes)
				{
					CreateMap(entityType, dtoType).ReverseMap();
				}
			}
		}

	}
}
