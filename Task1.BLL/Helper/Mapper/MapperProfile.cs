using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //Store
            CreateMap<Store, StoreViewDTO>().ReverseMap();
            CreateMap<Store, StoreCreateRequestDTO>().ReverseMap();
            CreateMap<Store, StoreUpdateRequestDTO>().ReverseMap();
            CreateMap<Store, StoreDetailDTO>().ReverseMap();

            //Titles
            CreateMap<Title, TitleViewDTO>().ReverseMap();
            CreateMap<Title, TitleCreateRequestDTO>().ReverseMap();
            CreateMap<Title, TitleUpdateRequestDTO>().ReverseMap();

            //Employees
            CreateMap<Employee, EmpViewDTO>().ReverseMap();
            CreateMap<Employee, EmpCreateRequestDTO>().ReverseMap();
            CreateMap<Employee, EmpUpdateRequestDTO>().ReverseMap();
        }

    }
}
