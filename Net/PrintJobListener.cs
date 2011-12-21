/* Copyright 2011 Corey Bonnell and Sandro Lange

   This file is part of Touch2PcPrinter for Windows.

    Touch2PcPrinter for Windows is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Touch2PcPrinter for Windows is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Touch2PcPrinter for Windows.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace WebosPrinter.Net
{
    internal class PrintJobListener : TcpListener
    {
        private const int JOB_LISTEN_PORT = 9100;

        private readonly Action<string> logger;
        private readonly CancellationToken cancelToken;

        public PrintJobListener(Action<string> logger, CancellationToken cancelToken) 
			: base(IPAddress.Any, JOB_LISTEN_PORT){
			
            this.logger = logger;
            this.cancelToken = cancelToken;
        }
		
		void WaitToReceiveJob() {
            do{
                Thread.Sleep(250);
                this.cancelToken.ThrowIfCancellationRequested();
				
            } while (!this.Pending());			
		}

        public string ReadJobToFile()
        {
			WaitToReceiveJob();
            string filePath;
			
            using (var client = this.AcceptTcpClient()){
				
                filePath = Path.GetTempFileName();
                using (var stream = File.OpenWrite(filePath)){
					
                    var remoteIpData = (IPEndPoint)client.Client.RemoteEndPoint;
                    this.logger.Invoke(String.Format("Incoming print job from {0}:{1}", remoteIpData.Address, remoteIpData.Port));
					
					this.WriteData(client.GetStream(), stream);
           			this.logger.Invoke(String.Format("Wrote print job file \"{0}\"", filePath));			
                }
            }
            return filePath;
        }
		
		void WriteData(Stream instream, FileStream stream){
            byte[] buffer = new byte[4096];
            int readCount = 0;
            do {
                int read = instream.Read(buffer, 0, buffer.Length);
                
				if (read < 1)                        
                    break;
                
                readCount += read;
                stream.Write(buffer, 0, read);
            } while (true);
		}
    }
}
