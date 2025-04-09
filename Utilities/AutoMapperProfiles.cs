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
        }

        private void GenresMapConfig()
        {
            CreateMap<GenreCreationDTO, Genre>();
            CreateMap<Genre, GenreDTO>();
        }
    }
}
