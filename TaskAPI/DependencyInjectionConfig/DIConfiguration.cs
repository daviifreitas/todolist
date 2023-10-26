using Activity.API.Interfaces;
using Activity.API.Repository;

namespace Activity.API.DependencyInjectionConfig
{
    public static class DIConfiguration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IActivityTicketRepository, ActivityTicketTicketRepository>();
        }

    }
}
