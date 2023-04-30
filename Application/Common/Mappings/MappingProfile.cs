using AutoMapper;
using System.Reflection;
using Tournament.Application.Accounts.Queries.GetAccountParticipations;
using Tournament.Application.Contests.Queries.GetContests;
using Tournament.Domain.Entities;

namespace Tournament.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Answer, AnswerDto>()
          .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Option.Question.Id));

        CreateMap<Contest, ContestBriefDto>()
            .ForMember(dest => dest.ParticipationsCount, opt => opt.MapFrom(src => src.Participations.Count()))
            .ForMember(dest => dest.ParticipationsTotalPoints, opt => opt.MapFrom(src => src.Participations.Sum(x => x.Spent)))
            .ForMember(dest => dest.ParticipationCapacity, opt => opt.MapFrom(src => src.ParticipationCapacity))
            .ForMember(dest => dest.ChannelId, opt => opt.MapFrom(src => src.Channel.Id))
            .ForMember(dest => dest.ChannelTitle, opt => opt.MapFrom(src => src.Channel.Title))
            .ForMember(dest => dest.QuestionsCount, opt => opt.MapFrom(src => src.Questions.Count()));



        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var mapFromType = typeof(IMapFrom<>);

        var mappingMethodName = nameof(IMapFrom<object>.Mapping);

        bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;

        var types = assembly.GetExportedTypes().Where(t => t.GetInterfaces().Any(HasInterface)).ToList();

        var argumentTypes = new Type[] { typeof(Profile) };

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var methodInfo = type.GetMethod(mappingMethodName);

            if (methodInfo != null)
            {
                methodInfo.Invoke(instance, new object[] { this });
            }
            else
            {
                var interfaces = type.GetInterfaces().Where(HasInterface).ToList();

                if (interfaces.Count > 0)
                {
                    foreach (var @interface in interfaces)
                    {
                        var interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);

                        interfaceMethodInfo?.Invoke(instance, new object[] { this });
                    }
                }
            }
        }
    }
}

