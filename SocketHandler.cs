using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace reverseproxy
{
	internal class SocketHandler
	{
		private Socket soc;

		private string requestHeaders;

		private string fwdDest;

		private string LOCAL_HOSTNAME = "";

		private int BUFFER_SIZE = 10240;

		public SocketHandler(Socket sock, string host)
		{
			soc = sock;
			LOCAL_HOSTNAME = host;
		}

		public void handleConnection()
		{
			NetworkStream networkStream = new NetworkStream(soc);
			byte[] numArray = new byte[BUFFER_SIZE];
			StringBuilder stringBuilder = new StringBuilder();
			int bufferSize = BUFFER_SIZE;
			while (bufferSize == BUFFER_SIZE)
			{
				bufferSize = networkStream.Read(numArray, 0, BUFFER_SIZE);
				string str = Encoding.UTF8.GetString(numArray);
				stringBuilder.Append(str);
			}
			string str1 = stringBuilder.ToString();
			str1 = str1.Replace("\0", "");
			if (str1.Length >= 2)
			{
				string[] strArrays = str1.Split('\n');
				for (int i = 0; i < strArrays.Length; i++)
				{
					string str2 = strArrays[i];
					string str3 = str2.Replace("\r", "");
					if (!str2.ToLower().Contains("accept-encoding"))
					{
						if (str3.Length >= 4 && str3.Trim().Substring(0, 4).ToLower() == "host")
						{
							Regex regex = new Regex(string.Concat("host: (.*)\\.", LOCAL_HOSTNAME.Replace(".", "\\.")));
							Match match = regex.Match(str3.ToLower());
							fwdDest = match.Groups[1].Value;
							str3 = string.Concat("Host: ", fwdDest);
						}
						SocketHandler socketHandler = this;
						socketHandler.requestHeaders = string.Concat(socketHandler.requestHeaders, str3, "\r\n");
					}
				}
				requestHeaders = string.Concat(requestHeaders.Trim(), "\r\n\r\n");
			    if (requestHeaders == "") return;
			    int num = 0;
			    try
			    {
			        NetworkStream stream = new TcpClient(fwdDest, 80).GetStream();
			        byte[] bytes = new byte[BUFFER_SIZE];
			        bytes = Encoding.UTF8.GetBytes(requestHeaders);
			        stream.Write(bytes, 0, bytes.Length);
			        bufferSize = BUFFER_SIZE;
			        while (bufferSize > 0)
			        {
			            bufferSize = stream.Read(numArray, 0, BUFFER_SIZE);
			            num = num + bufferSize;
			            networkStream.Write(numArray, 0, bufferSize);
			        }
			        networkStream.Close();
			        stream.Close();
			    }
			    catch (Exception ex)
			    {
			        if (num == 0)
			        {
			            byte[] buf = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nCache-Control: no-cache\r\n\r\n<head><title>prox error</title></head><body><pre>An error occurred: " + ex.Message + "\r\nCheck your address and try again.</pre></body>\r\n\r\n");
			            Console.WriteLine("hmm:");
			            Console.WriteLine(ex.Message);
			            soc.Send(buf);
			        }
			    }
					
			    soc.Close();
			}
		}
	}
}