using AutoMapper;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;

namespace PeliculasAPI.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            GenresMapConfig();
            ActorsMapConfig();
        }

        private void GenresMapConfig()
        {
            CreateMap<GenreCreationDTO, Genre>();
            CreateMap<Genre, GenreDTO>();
        }

        private void ActorsMapConfig()
        {
            CreateMap<ActorCreationDTO, Actor>()
                .ForMember(actor => actor.ProfileImage, options => options.Ignore());
            CreateMap<Actor, ActorDTO>();
        }
    }
}
