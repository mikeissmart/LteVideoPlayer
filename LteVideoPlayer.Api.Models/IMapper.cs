using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LteVideoPlayer.Api.Models
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
