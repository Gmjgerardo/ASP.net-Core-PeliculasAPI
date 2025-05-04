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
            MoviesMapConfig();
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

            CreateMap<Actor, MovieActorDTO>()
                .ForMember(mActor => mActor.Image, image => image.MapFrom(actor => actor.ProfileImage));
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

        private void MoviesMapConfig()
        {
            CreateMap<MovieCreationDTO, Movie>()
                .ForMember(movie => movie.Image, options => options.Ignore())
                .ForMember(movie => movie.MovieGenres, dataList => dataList.MapFrom(dto =>
                dto.GenresIds!.Select(id => new MovieGenre { GenreId = id})))
                .ForMember(movie => movie.MovieCinemas, dataList => dataList.MapFrom(dto =>
                dto.CinemasIds!.Select(id => new MovieCinema { CinemaId = id })))
                .ForMember(movie => movie.MovieActors, dataList => dataList.MapFrom(dto =>
                dto.Actors!.Select(actor => new MovieActor { ActorId = actor.Id, Character = actor.Character })));

            CreateMap<Movie, MovieDTO>();
        }
    }
}
