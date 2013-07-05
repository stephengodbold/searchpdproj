using Autofac;
using autocomplete.Models;

namespace searchpd.Repositories
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Constants>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
