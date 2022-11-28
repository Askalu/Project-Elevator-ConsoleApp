using AutoMapper;
using ElevatorConsoleApp.Models;

namespace ElevatorConsoleApp.Helpers.Profiles;

internal class AutoMapper : Profile
{
    public AutoMapper()
    {
        CreateMap<ElevatorResponse, Elevator>().ForMember(x => x.Id, y => y.Ignore());
        CreateMap<Elevator, ElevatorRequest>().ForMember(x => x.Id, y => y.MapFrom(x => x.ElevatorId));
        
    }
}