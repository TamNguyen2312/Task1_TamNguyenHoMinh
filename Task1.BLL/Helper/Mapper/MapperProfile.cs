﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.StoreDTOs;
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
        }

    }
}
