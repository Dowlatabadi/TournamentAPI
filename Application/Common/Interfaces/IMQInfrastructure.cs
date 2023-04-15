using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IMQInfrastructure
    {
        void ConfigureConsumer(Func<byte[],IServiceScopeFactory ,bool> consumeAction);
        void PublishMessage(string message);
    }
}
