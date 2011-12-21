using System;

namespace WebosPrinter
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length == 0 || args.Length >1)
				new ArgumentException(@"
Command: WebosPrinterServer <script-file>
script-file is a shell script that work with pcl-file from WebOS.
It will be called if a printjob received

Parameter of the scriptfile call:
Parameter: pcl-file color-mode plex-mode 
");
			
			new WebOsPrinterServer(args[0])
				.Start();
		}
	}
}
