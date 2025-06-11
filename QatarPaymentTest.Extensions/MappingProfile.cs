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

            CreateMap<CreateCustomFieldDto, CustomField>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CompanyValues, opt => opt.Ignore())
                .ForMember(dest => dest.ContactValues, opt => opt.Ignore());

            CreateMap<CustomField, CustomFieldDto>();


            CreateMap<ContactDto, Contact>()
                .ForMember(dest => dest.Companies, opt => opt.Ignore()) // Handle separately
                .ForMember(dest => dest.CustomFieldValues, opt => opt.Ignore()); // Handle separately

            CreateMap<Contact, ContactDto>()
                .ForMember(dest => dest.Companies, opt => opt.MapFrom(src => src.Companies))
                .ForMember(dest => dest.CustomFields, opt => opt.Ignore());

            CreateMap<CompanyDto, Company>()
          .ForMember(dest => dest.Contacts, opt => opt.Ignore()) // Handle many-to-many separately
          .ForMember(dest => dest.CustomFieldValues, opt => opt.Ignore()); // Handle custom fields separately

            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Contacts))
                .ForMember(dest => dest.CustomFields, opt => opt.Ignore());


        }
    }
}
