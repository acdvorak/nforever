// Copyright 2012, 2013, 2014 Andrew C. Dvorak
//
// This file is part of BDHero.
//
// BDHero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// BDHero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with BDHero.  If not, see <http://www.gnu.org/licenses/>.

using System.ComponentModel;
using OSUtils.JobObjects;

namespace ProcessUtils
{
    /// <summary>
    /// Threaded version of <see cref="NonInteractiveProcess"/>.  Allows a process to run in the background
    /// on a separate thread while reporting its status and progress information to the UI.
    /// </summary>
    public class BackgroundProcessWorker : NonInteractiveProcess
    {
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly BackgroundWorker _worker = new BackgroundWorker
                                                        {
                                                            WorkerReportsProgress = true,
                                                            WorkerSupportsCancellation = true
                                                        };

        /// <summary>
        ///     Constructs a new <see cref="BackgroundProcessWorker"/> object that uses the given
        ///     <paramref name="jobObjectManager"/> to ensure that child processes are terminated
        ///     if the parent process exits prematurely.
        /// </summary>
        /// <param name="jobObjectManager"></param>
        public BackgroundProcessWorker(IJobObjectManager jobObjectManager)
            : base(jobObjectManager)
        {
            _worker.DoWork += (sender, args) => Start();
            _worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args)
                {
                    if (args.Error != null)
                    {
                        Logger.Error("Error occurred while running NonInteractiveProcess in BackgroundWorker", args.Error);
                    }
                };
        }

        public BackgroundProcessWorker StartAsync()
        {
            _worker.RunWorkerAsync();
            return this;
        }
    }
}
