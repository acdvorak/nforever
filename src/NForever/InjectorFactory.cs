using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Modules;
using WindowsOSUtils;
using log4net;

namespace NForever
{
	public static class InjectorFactory
	{
		public static IKernel CreateContainer()
		{
			var modules = new List<INinjectModule>();
			modules.Add(new LoggingModule());
			modules.AddRange(CreateOSMainModules());
			return new StandardKernel(modules.ToArray());
		}

		private static IEnumerable<INinjectModule> CreateOSMainModules()
		{
			return WindowsInjectorFactory.CreateMainModules();
		}
	}

	internal class LoggingModule : NinjectModule
	{
		public override void Load()
		{
			Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Type));
		}
	}
}
