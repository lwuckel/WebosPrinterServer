using System;
using System.IO;
using WebosPrinter.Net;
using System.Text;

namespace WebosPrinter.IO
{
	public class PclFileReader
	{
		string pclFilePath;
        private const int DUPLEX_SETTING_OFFSET = 0x36;
        private const string DUPLEX_SETTING_STRING = "ON";
        private const int SEARCH_SIZE = 512;
        private const string BLACK_WHITE_STRING = "&b1M";		
		
		public PclFileReader (string pclFilePath){
			this.pclFilePath = pclFilePath;
		}
		
        public PrintJobProperties GetProperties()
        {
            byte[] initialBytes = new byte[SEARCH_SIZE];
            using (var stream = File.OpenRead(pclFilePath))
            {
                int totalRead = 0;
                do{
                    int read = stream.Read(initialBytes, totalRead, initialBytes.Length - totalRead);
                    
					if (read < 1)
                        throw new EndOfStreamException("PCL file is too small, cannot read job properties");

					totalRead += read;
					
                } while (totalRead < SEARCH_SIZE);
            }
			
            PlexMode plexMode = this.GetPlexMode(initialBytes);
            ColorMode colorMode = this.GetColorMode(initialBytes);
			            
            return new PrintJobProperties(colorMode, plexMode);
        }
		
		ColorMode GetColorMode(byte[] initialBytes){
            string searchString = Encoding.ASCII.GetString(initialBytes);
			
			return (searchString.Contains(BLACK_WHITE_STRING))
            	? ColorMode.BlackAndWhite
				: ColorMode.Color;			
		}

		PlexMode GetPlexMode(byte[] initialBytes){
			string ascii = Encoding.ASCII.GetString(initialBytes, DUPLEX_SETTING_OFFSET, DUPLEX_SETTING_STRING.Length);
			
			PlexMode plexMode = (String.Equals(DUPLEX_SETTING_STRING,ascii , StringComparison.Ordinal)) 
				? PlexMode.Duplex
				: PlexMode.Simplex;

			return plexMode;
		}
	}
}

