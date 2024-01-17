using AutoMapper;
using Final.DTOs;
using Final.Models;

namespace Final.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Db->client
            CreateMap<Meet, ResponseDTO>();
            //client->Db
            CreateMap<RequestDTO, Meet>();

        }
    }
}
