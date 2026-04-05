using AutoMapper;
using Core.DTOs;
using Core.DTOs.Requests;
using Core.DTOs.Responses;
using Core.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id));

        CreateMap<CreateProductRequest, Product>();
        CreateMap<UpdateProductRequest, Product>();
    }
}