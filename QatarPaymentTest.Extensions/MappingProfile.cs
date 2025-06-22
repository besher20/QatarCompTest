using AutoMapper;
using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Contact, ContactDto>()
                .ForMember(dest => dest.Companies, opt => opt.MapFrom(src => src.Companies))
                .ForMember(dest => dest.CustomFieldValues, opt => opt.MapFrom(src => src.CustomFieldValues.ToDictionary(
                    cf => cf.CustomFieldId,
                    cf => cf.Value ?? string.Empty
                )));

            CreateMap<CreateContactDto, Contact>()
                .ForMember(dest => dest.Companies, opt => opt.Ignore())
                .ForMember(dest => dest.CustomFieldValues, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => System.DateTime.UtcNow));

            CreateMap<UpdateContactDto, Contact>()
                .ForMember(dest => dest.Companies, opt => opt.Ignore())
                .ForMember(dest => dest.CustomFieldValues, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => System.DateTime.UtcNow));

            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Contacts))
                .ForMember(dest => dest.CustomFields, opt => opt.MapFrom(src => src.CustomFieldValues.ToDictionary(
                    cf => cf.CustomFieldId,
                    cf => cf.Value ?? string.Empty
                )));

            CreateMap<Contact, ContactSummaryDto>();

            CreateMap<CreateCompanyDto, Company>()
                .ForMember(dest => dest.Contacts, opt => opt.Ignore())
                .ForMember(dest => dest.CustomFieldValues, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => System.DateTime.UtcNow));

            CreateMap<UpdateCompanyDto, Company>()
                .ForMember(dest => dest.Contacts, opt => opt.Ignore())
                .ForMember(dest => dest.CustomFieldValues, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => System.DateTime.UtcNow));

            CreateMap<CustomField, CustomFieldDto>();
            CreateMap<CreateCustomFieldDto, CustomField>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => System.DateTime.UtcNow));
            CreateMap<CustomFieldDto, CustomField>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => System.DateTime.UtcNow));
        }
    }
}
