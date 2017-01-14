using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace IlViewer
{
    public class Program
    {
        public static void Main(string[] args)
        {
	        var argsList = new List<string>(args);
	        if (argsList.Contains("--debug"))
	        {
		        argsList.Remove("--debug");
		        Console.WriteLine($"2Attach debugger to process {Process.GetCurrentProcess().Id} to continue. ..");


		        Thread.Sleep(8000);
		        Console.WriteLine("8 seconds later...");
		        while (!Debugger.IsAttached)
		        {
			        Thread.Sleep(100);
		        }
	        }

	        using (var stdin = Console.OpenStandardInput())
	        using (var stdout = Console.OpenStandardOutput())
	        {
		        Console.WriteLine(stdin.ToString());

		        var buffer = new byte[2048];
		        int bytes;
		        while ((bytes = stdin.Read(buffer, 0, buffer.Length)) > 0) {
			        stdout.Write(buffer, 0, bytes);
		        }
	        }
        }
    }
}
