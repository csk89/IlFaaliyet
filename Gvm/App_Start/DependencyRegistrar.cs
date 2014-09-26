using System;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Gvm.Infra;
using MemoryCacheT;
using Microsoft.AspNet.Identity;
using Serilog;
using Turkok.Core.Caching;
using Turkok.Core.Mappers;
using Turkok.Core.Repository;
using Turkok.Core.Service;
using Turkok.Core.Service.CrudService;
using Turkok.Core.Settings;
using Turkok.Model;

namespace Gvm
{
    public class DependencyRegistrar
    {

        public static void RegisterDependencies()
        {  
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType<ApplicationSettings>().As<IApplicationSettings>();

            builder.RegisterType<DataContext>().As<IDbContext>().InstancePerRequest();

            builder.RegisterType<ClaimAuthorisationService>().As<IClaimAuthorisationService>().InstancePerRequest();

            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();

            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IRepository<>)).InstancePerDependency();

            builder.RegisterGeneric(typeof(CacheManager<,>)).As(typeof(ICacheManager<,>)).SingleInstance(); 

            TimeSpan expiration = TimeSpan.FromHours(1);

            builder.RegisterGeneric(typeof(Cache<,>)).As(typeof(ICache<,>)).WithParameter("timerInterval", expiration).SingleInstance();
            
            builder.RegisterGeneric(typeof(Mapper<,>)).As(typeof(IMapper<,>)).InstancePerDependency();

            builder.RegisterGeneric(typeof(BaseCrudService<>)).As(typeof(ICrudService<>)).InstancePerDependency();

            builder.RegisterType<ApplicationUserStore<ApplicationUser>>().As<IUserStore<ApplicationUser>>().UsingConstructor(typeof(IDbContext));

            builder.RegisterType<UserManager<ApplicationUser>>().UsingConstructor(typeof(IUserStore<ApplicationUser>)).InstancePerDependency();

            builder.Register(c => new LoggerConfiguration().WriteTo.RollingFile("log-{Date}.txt").CreateLogger()).As<ILogger>().InstancePerRequest();

            builder.RegisterFilterProvider(); 

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}