using AutoMapper;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_Back_SharedModels;
namespace STBEverywhere_back_APICompte
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<CreateCompteDto, Compte>();
            CreateMap<Compte, CreateCompteDto>();

        }
       
    }
}
