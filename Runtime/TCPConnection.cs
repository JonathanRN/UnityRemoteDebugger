using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TCPConnection
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void RuntimeInitialize()
	{
		// Start the listener after the scene loads
		new TCPConnection();
	}

	private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>()
	{
		{ "ls", new HierarchyCommand() },
		{ "destroy", new DestroyCommand() },
	};

	private readonly TcpListener tcpListener;
	private const int PORT = 5555;

	public TCPConnection()
	{
		// Adding the help command manually to fetch the others
		commands.Add("help", new HelpCommand(commands.Keys.ToArray()));

		IPAddress localIp = GetLocalIPAddress();
		tcpListener = new TcpListener(localIp, PORT);
		tcpListener.Start();
		Debug.Log($"[UnityRemoteDebugger] Started listening on {localIp}:{PORT}");
		WaitForTcpClientAsync();
	}

	private async void WaitForTcpClientAsync()
	{
		TcpClient client = await tcpListener.AcceptTcpClientAsync();
		Debug.Log($"[UnityRemoteDebugger] New connection established with {client.Client.LocalEndPoint}");
		NetworkStream stream = client.GetStream();

		byte[] buffer = new byte[256];
		while (Application.isPlaying)
		{
			int recv;
			try
			{
				recv = await stream.ReadAsync(buffer, 0, buffer.Length);
				if (recv == 0)
				{
					throw new SocketException();
				}
			}
			catch (SocketException)
			{
				Debug.Log($"[UnityRemoteDebugger] Connection lost with {client.Client.LocalEndPoint}");

				if (Application.isPlaying)
				{
					stream.Close();
					client.Close();
					// Restart the loop
					WaitForTcpClientAsync();
				}
				break;
			}

			string input = Encoding.UTF8.GetString(buffer.Take(recv).ToArray()).Trim('\r', '\n');
			var (command, parameters) = GetCommand(input);

			if (command != null && commands.ContainsKey(command))
			{
				string response = commands[command].GetString(parameters);
				byte[] bytes = Encoding.UTF8.GetBytes(response);
				Debug.Log($"[UnityRemoteDebugger] Sending command {command}");
				await stream.WriteAsync(bytes, 0, bytes.Length);
			}
			else
			{
				Debug.Log($"[UnityRemoteDebugger] Received unknown command {command}");
				string response = $"Invalid command {command}";
				byte[] bytes = Encoding.UTF8.GetBytes(response);
				await stream.WriteAsync(bytes, 0, bytes.Length);
			}

			buffer = new byte[256];
		}

		if (!Application.isPlaying)
		{
			tcpListener.Stop();
			stream.Close();
			client.Close();
		}
	}

	private (string, string[]) GetCommand(string input)
	{
		string[] split = input.Split(' ');
		if (commands.ContainsKey(split[0]))
		{
			return (split[0], split.Skip(1).ToArray());
		}
		return (null, null);
	}

	private IPAddress GetLocalIPAddress()
	{
		return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
	}
}
