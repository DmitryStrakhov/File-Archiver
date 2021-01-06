using System;
using Castle.Windsor;
using FileArchiver.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;

namespace FileArchiver.IOC {
    public class Ioc : IDisposable {
        readonly WindsorContainer ioc;

        public Ioc() {
            this.ioc = new WindsorContainer();
            this.ioc.Install(new DefaultInstaller());
        }
        public T Resolve<T>() {
            return ioc.Resolve<T>();
        }
        public void Dispose() {
            ioc.Dispose();
        }

        #region IWindsorInstaller

        class DefaultInstaller : IWindsorInstaller {
            public void Install(IWindsorContainer container, IConfigurationStore store) {
                container.Register(Classes.FromAssemblyContaining<MainWindow>()
                    .Pick()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient());
            }
        }

        #endregion
    }
}