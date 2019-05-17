﻿namespace Nancy.Tests.Fakes
{
    using System;

    public class FakeNancyModule : NancyModule
    {
        public FakeNancyModule()
        {
        }

        public FakeNancyModule(Action<FakeNancyModuleConfigurator> closure)
        {
            var configurator = 
                new FakeNancyModuleConfigurator(this);

            closure.Invoke(configurator);
        }

        public class FakeNancyModuleConfigurator
        {
            private readonly NancyModule module;

            public FakeNancyModuleConfigurator(NancyModule module)
            {
                this.module = module;
            }

            public FakeNancyModuleConfigurator AddDeleteRoute(string path)
            {
                this.module.Delete[path] = parameters => {
                    return HttpStatusCode.OK;
                };

                return this;
            }

            public FakeNancyModuleConfigurator AddGetRoute(string path)
            {
                return this.AddGetRoute(path, x => HttpStatusCode.OK);
            }

            public FakeNancyModuleConfigurator AddGetRoute(string path, Func<object, Response> action)
            {
                this.module.Get[path] = action;
                return this;
            }

            public FakeNancyModuleConfigurator AddPostRoute(string path)
            {
                this.module.Post[path] = parameters => {
                    return HttpStatusCode.OK;
                };

                return this;
            }

            public FakeNancyModuleConfigurator AddPutRoute(string path)
            {
                this.module.Put[path] = parameters => {
                    return HttpStatusCode.OK;
                };

                return this;
            }
        }
    }
}