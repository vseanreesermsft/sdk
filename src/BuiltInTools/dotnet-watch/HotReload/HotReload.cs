﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Tools.Internal;

namespace Microsoft.DotNet.Watcher.Tools
{
    internal class HotReload : IDisposable
    {
        private readonly StaticFileHandler _staticFileHandler;
        private readonly CompilationHandler _compilationHandler;

        public HotReload(IReporter reporter)
        {
            _staticFileHandler = new StaticFileHandler(reporter);
            _compilationHandler = new CompilationHandler(reporter);
        }

        public async ValueTask InitializeAsync(DotNetWatchContext dotNetWatchContext, CancellationToken cancellationToken)
        {
            await _compilationHandler.InitializeAsync(dotNetWatchContext, cancellationToken);
        }

        public async ValueTask<bool> TryHandleFileChange(DotNetWatchContext context, FileItem file, CancellationToken cancellationToken)
        {
            if (await _staticFileHandler.TryHandleFileChange(context, file, cancellationToken))
            {
                return true;
            }

            if (await _compilationHandler.TryHandleFileChange(context, file, cancellationToken)) // This needs to be 6.0
            {
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            _compilationHandler.Dispose();
        }
    }
}
