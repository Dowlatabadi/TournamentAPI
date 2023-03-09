using Tournament.Application.Common.Interfaces;
namespace Tournament.Infrastructure.Services;

public class DateTimeService:IDateTime
{
    public DateTime Now => DateTime.Now;
}
