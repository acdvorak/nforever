using System;
using NForever.Properties;
using Ninject;
using OSUtils.JobObjects;
using log4net;

namespace NForever
{
	class Program
	{
		private static readonly ILog Logger =
			LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		static void Main(string[] args)
		{
			new LogInitializer().Initialize("log4net.config", Resources.log4net);

			if (args.Length == 0)
			{
				args = new[] { "app.js" };
			}

            var kernel = CreateInjector();
            var manager = kernel.Get<IJobObjectManager>();

			if (manager.TryBypassPCA(args))
				return;

			var keepRunning = true;
			var fastCrash = false;

			while (keepRunning)
			{
				var runner = kernel.Get<NodeRunner>();
				var startTime = DateTime.Now;
				runner.Run(args);
				var runTime = DateTime.Now - startTime;
				fastCrash = runTime < TimeSpan.FromSeconds(2);
				keepRunning = !runner.CleanExit && !fastCrash;
			}

			if (fastCrash)
			{
				Logger.Fatal("Server crashed immediately on startup");
				Console.WriteLine("Press <ENTER> twice to exit");
				Console.In.ReadLine();
			}
		}

		private static IKernel CreateInjector()
		{
			return InjectorFactory.CreateContainer();
		}
	}
}
