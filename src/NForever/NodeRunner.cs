using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProcessUtils;

namespace NForever
{
	class NodeRunner
	{
		private static readonly log4net.ILog Logger =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private static readonly log4net.ILog OutputLogger =
			log4net.LogManager.GetLogger("NodeOutputLogger");

		private static readonly Regex QuitRegex = new Regex(@"^(?:q|quit|exit|bye)$", RegexOptions.IgnoreCase);

		private readonly BackgroundProcessWorker _node;

		private readonly AutoResetEvent _barrier = new AutoResetEvent(false);

		public bool HasExited { get; private set; }

		public bool CleanExit { get; private set; }

		public NodeRunner(BackgroundProcessWorker node)
		{
			_node = node;
			_node.ExePath = "node.exe";
			_node.StdOut += NodeOnStdOut;
			_node.StdErr += NodeOnStdErr;
			_node.Exited += NodeOnExited;
		}

		/// <exception cref="InvalidOperationException">
		///		Thrown if this method is called more than once.
		/// </exception>
		public void Run(string[] args)
		{
			if (_node.State != NonInteractiveProcessState.Ready)
			{
				throw new InvalidOperationException("NodeRunner cannot be run more than once");
			}

			_node.Arguments = new ArgumentList(args);

			Logger.InfoFormat("Starting node.exe {0}", _node.Arguments);

			_node.StartAsync();

			Task.Factory.StartNew(WaitForUserInput);

			_barrier.WaitOne();

			_node.Kill();
		}

		private void WaitForUserInput()
		{
			while (KeepRunning(Console.In.ReadLine()))
			{
			}
			CleanExit = true;
			_barrier.Set();
		}

		private bool KeepRunning(string line)
		{
			if (HasExited)
				return false;
			if (line == @"\d")
				return false;
			if (QuitRegex.IsMatch(line))
				return false;
			return true;
		}

		private void NodeOnStdOut(string line)
		{
			// Process exited
			if (line == null) return;

			Logger.Info(line);
			OutputLogger.Info(line);
		}

		private void NodeOnStdErr(string line)
		{
			// Process exited
			if (line == null) return;

			Logger.Error(line);
			OutputLogger.Error(line);
		}

		private void NodeOnExited(NonInteractiveProcessState state, int exitCode, Exception exception, TimeSpan runTime)
		{
			Logger.Warn("node.exe exited");
			Logger.DebugFormat("state: {0}", state);
			Logger.DebugFormat("exitCode: {0}", exitCode);
			Logger.DebugFormat("exception: {0}", exception);

			HasExited = true;
			CleanExit = (state == NonInteractiveProcessState.Completed || state == NonInteractiveProcessState.Killed) &&
			            (exitCode == 0 || exitCode == -1) &&
			            exception == null;

			_barrier.Set();
		}
	}
}
