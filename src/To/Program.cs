using CommandLine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Andtech.To
{

	public class Options
	{
		[Value(0)]
		public IList<string> Tokens { get; set; }

		[Option("verbose", HelpText = "Print verbose messages.")]
		public bool Verbose { get; set; }
		[Option('n', "dry-run", HelpText = "Dry run the command.")]
		public bool DryRun { get; set; }
		[Option("list", HelpText = "List the results.")]
		public bool List { get; set; }
	}

	class Program
	{

		static async Task Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<Options>(args);
			await result.WithParsedAsync(OnParse);
		}

		public static async Task OnParse(Options options)
		{
			try
			{
				var runner = new Runner(options);
				await runner.Run();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
				Environment.Exit(-1);
			}
		}
	}
}

