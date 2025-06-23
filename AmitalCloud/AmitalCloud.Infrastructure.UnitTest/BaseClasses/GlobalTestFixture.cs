using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AmitalCloud.Infrastructure.Web.BaseClasses;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AmitalCloud.Infrastructure.UnitTest.BaseClasses
{
    public class GlobalTestFixture
    {
        public GlobalTestFixture()
        {
            Console.WriteLine("Global setup running...");

        }
    }

}
