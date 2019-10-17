using System;
using AutoMapper;
using MongoDB.Bson;
using MongoDBSync.WebAPI.Domain;
using MongoDBSync.WebAPI.ViewModels.Command;

namespace MongoDBSync.WebAPI.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CommandViewModel, Command>()
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => new BsonDateTime(src.Timestamp)))
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => new BsonBoolean(src.IsSynced)));
            CreateMap<Command, CommandViewModel>()
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.Parse(src.Timestamp.ToString())))
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => Boolean.Parse(src.IsSynced.Value.ToString())));
        }
    }
}
