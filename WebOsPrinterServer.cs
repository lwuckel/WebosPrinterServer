using System;
using WebosPrinter.Net;
using System.Threading;
using System.Diagnostics;

namespace WebosPrinter
{
	public class WebOsPrinterServer
	{
		PrintServer currentServer;
		string execute;
		Action<string> logger;
		
		public WebOsPrinterServer (string execute)
		{
			this.execute = execute;
			
            logger = (message) =>{
#if DEBUG
				Console.Out.WriteLine(message);
#endif
			};
			
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
			
            this.currentServer = new PrintServer(logger, cancelTokenSource.Token);			
			this.currentServer.ReceivedJob += ReceivedJob;
		}
		
		void ReceivedJob(object sender, PclPrintingArgs e) {
			logger.Invoke(e.FileName);
            var p = new Process();
            var si = p.StartInfo;
            si.FileName = execute;
            si.UseShellExecute = false;
            si.CreateNoWindow = true;
            si.Arguments = string.Format("{0} {1} {2}", e.FileName, e.Properties.ColorMode, e.Properties.PlexMode);
			
			logger.Invoke(si.FileName);
			logger.Invoke(si.Arguments);
			
            p.Start();
            p.WaitForExit();
            
			if (p.ExitCode != 0)
                throw new Exception(string.Format("returned with abnormal return code {0}", p.ExitCode));
		}
		
		public void Start(){
            this.currentServer.Start();			
			
			while(true)
				Thread.Sleep(1000);			
		}
	}
}

