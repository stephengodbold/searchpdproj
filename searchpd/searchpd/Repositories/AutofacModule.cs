﻿using Autofac;

namespace searchpd.Repositories
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CategoryRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ProductRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
