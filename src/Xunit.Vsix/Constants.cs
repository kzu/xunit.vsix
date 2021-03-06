﻿using System.Diagnostics;

namespace Xunit
{
    internal static class Constants
    {
        /// <summary>
        /// This assembly name, for use in Xunit attributes that need to specify an
        /// assembly name.
        /// </summary>
        public const string ThisAssembly = "xunit.vsix";

        /// <summary>
        /// The root namespace of this assembly.
        /// </summary>
        public const string RootNamespace = "Xunit";

        public const string PipeNameEnvironmentVariable = "xunit.vsix.pipe";
        public const string BaseDirectoryEnvironmentVariable = "xunit.vsix.directory";
        public const string DebugEnvironmentVariable = "xunit.vsix.debug";

        public const string TracerName = "xunit.vsix";
        public const string ServerChannelName = "xunit.vsix.server";
        public const string ClientChannelName = "xunit.vsix.client-";

        public static readonly TraceSource Tracer = new TraceSource(TracerName);
    }
}
