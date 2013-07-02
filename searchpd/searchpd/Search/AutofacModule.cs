using Autofac;

namespace searchpd.Search
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Searcher>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
