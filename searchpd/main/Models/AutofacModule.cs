using Autofac;
using main.Models;

namespace searchpd.Repositories
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Constants>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
