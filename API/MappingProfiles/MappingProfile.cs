using AutoMapper;
using Core.DTOs;
using Core.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
    }
}