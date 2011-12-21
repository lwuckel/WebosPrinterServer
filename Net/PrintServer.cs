using System;
using System.Threading;
using System.IO;
using WebosPrinter.IO;

namespace WebosPrinter.Net
{
	public class PrintServer :  IDisposable
	{		
		public event EventHandler<PclPrintingArgs> ReceivedJob;	
		
        private readonly CancellationToken cancelToken;
        private readonly PrintJobListener printJobReader;
        private readonly SnmpPrinterAgent snmpAgent;
        private readonly Action<string> logger;		
		
		public PrintServer (Action<string> logger, CancellationToken cancelToken)
		{
            this.cancelToken = cancelToken;
            this.logger = logger;
			
            this.printJobReader = new PrintJobListener(this.logger, this.cancelToken);
            this.snmpAgent = new SnmpPrinterAgent(this.logger);					
		}
		
        public void Start()
        {
            this.printJobReader.Start();
            this.snmpAgent.Start();
            var serviceThread = new Thread(this.runServer);
            serviceThread.IsBackground = true;
            serviceThread.Start();
        }

        public void Stop()
        {
            this.printJobReader.Stop();
            this.snmpAgent.Stop();
        }

        public void Dispose()
        {
            this.snmpAgent.Dispose();
        }

        private void runServer()
        {
            while (!this.cancelToken.IsCancellationRequested)
            {
                this.logger.Invoke("Waiting for incoming job...");
                string tempPclPath = this.printJobReader.ReadJobToFile();
                var jobProperties = new PclFileReader(tempPclPath).GetProperties();
				
				this.OnReceivedJob(tempPclPath, jobProperties);
				
				File.Delete(tempPclPath);
            }
        }

		void OnReceivedJob(string fileName, PrintJobProperties properties){
			if (ReceivedJob != null){
				lock(ReceivedJob){
					PclPrintingArgs args = new PclPrintingArgs(fileName, properties);
					ReceivedJob(this,args);
				}
			}							
		}
	}
}

