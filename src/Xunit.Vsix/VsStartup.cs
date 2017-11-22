﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using Xunit.Properties;

namespace Xunit
{
    /// <summary>
    /// This is an internal class, and is not intended to be called from end-user code.
    /// </summary>
    /// <devdoc>
    /// This is the static entry point class invoked by the managed injector.
    /// It doesn't do anything other than spinning up a new instance of the
    /// <see cref="VsRemoteRunner"/> which does the actual work.
    /// </devdoc>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class VsStartup
    {
        private static readonly TraceSource s_tracer = Constants.Tracer;
        private static VsRemoteRunner s_runner;
        private static Dictionary<string, string> s_localAssemblyNames;

        /// <summary>
        /// This is an internal method, and is not intended to be called from end-user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool Start()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            // Determine if the current directory contains the assemblies to resolve, or if we
            // should locate the current assembly directory instead.
            var thisFileName = Path.GetFileName(typeof(VsStartup).Assembly.ManifestModule.FullyQualifiedName);
            var resolveDir = Directory.GetCurrentDirectory();
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), thisFileName)))
            {
                resolveDir = Path.GetDirectoryName(typeof(VsStartup).Assembly.ManifestModule.FullyQualifiedName);
                Directory.SetCurrentDirectory(resolveDir);
            }

            s_localAssemblyNames = GetLocalAssemblyNames(resolveDir);
            try
            {
                s_tracer.TraceEvent(TraceEventType.Verbose, 0, Strings.VsStartup.Starting);
                GlobalServices.Initialize();

                s_runner = new VsRemoteRunner();
                s_runner.Start();

                s_tracer.TraceInformation(Strings.VsStartup.Started);
                return true;
            }
            catch (Exception ex)
            {
                s_tracer.TraceEvent(TraceEventType.Error, 0, Strings.VsStartup.Failed + Environment.NewLine + ex.ToString());
                return false;
            }
        }

        private static Dictionary<string, string> GetLocalAssemblyNames(string localDirectory)
        {
            var names = new Dictionary<string, string>();
            foreach (var file in Directory.EnumerateFiles(localDirectory, "*.dll"))
            {
                try
                {
                    names.Add(AssemblyName.GetAssemblyName(file).FullName, file);
                }
                catch (SecurityException)
                {
                }
                catch (BadImageFormatException)
                {
                }
                catch (FileLoadException)
                {
                }
            }

            return names;
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            // NOTE: since we load our full names only in the local assembly set,
            // we will only return our assembly version if it matches exactly the
            // full name of the received arguments.
            if (s_localAssemblyNames.ContainsKey(args.Name))
                return Assembly.LoadFrom(s_localAssemblyNames[args.Name]);

            return null;
        }
    }
}
