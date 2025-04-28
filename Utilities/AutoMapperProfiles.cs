using AutoMapper;
using NetTopologySuite.Geometries;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;

namespace PeliculasAPI.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            GenresMapConfig();
            ActorsMapConfig();
            CinemasMapConfig(geometryFactory);
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

        private void CinemasMapConfig(GeometryFactory geometryFactory)
        {
            CreateMap<CinemaCreationDTO, Cinema>()
                .ForMember(cinema => cinema.location, point => point.MapFrom(CCDTO =>
                geometryFactory.CreatePoint(new Coordinate(CCDTO.Longitude, CCDTO.Latitude))));

            CreateMap<Cinema, CinemaDTO>()
                .ForMember(cDTO => cDTO.Longitude, d => d.MapFrom(cinema => cinema.location.X))
                .ForMember(cDTO => cDTO.Latitude, d => d.MapFrom(cinema => cinema.location.Y));
        }
    }
}
