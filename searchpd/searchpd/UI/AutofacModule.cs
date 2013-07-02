using Autofac;

namespace searchpd.UI
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DisplayFormatter>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
