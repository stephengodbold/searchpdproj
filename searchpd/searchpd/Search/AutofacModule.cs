using Autofac;

namespace searchpd.Search
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // These objects are solely dependent on the cache and on the Lucene index (a set of files).
            // So make them into singletons to reflect this.
            builder.RegisterType<SuggestionSearcher>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ProductSearcher>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<AutoupdateRefresher>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
