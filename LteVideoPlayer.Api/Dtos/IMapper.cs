using AutoMapper;

namespace LteVideoPlayer.Api.Dtos
{
	public interface IMapFrom<T>
	{
		void MapFrom(Profile profile) =>
			profile.CreateMap(typeof(T), GetType());
	}

	public interface IMapTo<T>
	{
		void MapTo(Profile profile) =>
			profile.CreateMap(GetType(), typeof(T));
	}
}
