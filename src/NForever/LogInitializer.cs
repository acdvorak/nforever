using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using log4net;

namespace NForever
{
	public class LogInitializer
	{
		private static readonly ILog Logger =
			LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public LogInitializer Initialize(string logConfigFilePath, string defaultLogConfig)
		{
			var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
			var assemblyMeta = entryAssembly.GetName();

			GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;
			GlobalContext.Properties["cwd"] = Environment.CurrentDirectory;

			if (File.Exists(logConfigFilePath))
			{
				log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(logConfigFilePath));
			}
			else
			{
				log4net.Config.XmlConfigurator.Configure(new MemoryStream(Encoding.UTF8.GetBytes(defaultLogConfig), false));
			}

			Logger.InfoFormat("{0} v{1} starting up", assemblyMeta.Name, assemblyMeta.Version);

			return this;
		}
	}
}
