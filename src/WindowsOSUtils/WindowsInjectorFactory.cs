﻿// Copyright 2012, 2013, 2014 Andrew C. Dvorak
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

using System.Collections.Generic;
using WindowsOSUtils.JobObjects;
using Ninject.Modules;
using OSUtils.JobObjects;

namespace WindowsOSUtils
{
    public static class WindowsInjectorFactory
    {
        public static IEnumerable<INinjectModule> CreateMainModules()
        {
            return new INinjectModule[]
                   {
                       new JobObjectModule()
                   };
        }
    }

    internal class JobObjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IJobObject>().To<JobObject>();
            Bind<IJobObjectManager>().To<JobObjectManager>();
        }
    }
}