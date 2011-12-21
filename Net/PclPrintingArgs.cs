using System;

namespace WebosPrinter.Net
{
	public class PclPrintingArgs : EventArgs
	{
		public string FileName{
			get;
			private set;
		}
		
		public PrintJobProperties Properties{
			get;
			private set;			
		}
		
		public PclPrintingArgs (string fileName,  PrintJobProperties properties)
		{
			this.FileName = fileName;
			this.Properties = properties;
		}
	}
}

