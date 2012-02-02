﻿using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using Pretzel.Commands;

namespace Pretzel
{
    class Program
    {
        [Import]
        private CommandCollection Commands { get; set; }

        static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            Compose();

            if (!args.Any())
            {
                Commands.WriteHelp();
                return;
            }

            var commandName = args[0];
            var commandArgs = args.Skip(1).ToArray();
            Commands[commandName].Execute(commandArgs);
            WaitForClose();
        }

        [Conditional("DEBUG")]
        public void WaitForClose()
        {
            Console.ReadLine();
        }

        public void Compose()
        {
            var first = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(first);

            var batch = new CompositionBatch();
            batch.AddExportedValue<IFileSystem>(new FileSystem());
            batch.AddPart(this);
            container.Compose(batch);
        }
    }
}
