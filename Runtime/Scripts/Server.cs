using Remotedebugger;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Google.Protobuf;

public class Server
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void RuntimeInitialize()
	{
		// Start the listener after the scene loads
		if (!SceneManager.GetActiveScene().name.Equals("RemoteDebugger"))
		{
			new Server();
		}
	}

	private readonly TcpListener tcpListener;
	private TcpClient client;
	private NetworkStream stream;
	private const int PORT = 5555;

	public Server()
	{
		IPAddress localIp = GetLocalIPAddress();
		tcpListener = new TcpListener(localIp, PORT);
		tcpListener.Start();
		Debug.Log($"[UnityRemoteDebugger] Started listening on {localIp}:{PORT}");
		WaitForTcpClientAsync();
	}

	~Server()
	{
		client?.Close();
		stream?.Close();
	}

	private async void WaitForTcpClientAsync()
	{
		client = await tcpListener.AcceptTcpClientAsync();
		Debug.Log($"[UnityRemoteDebugger] New connection established with {client.Client.LocalEndPoint}");
		stream = client.GetStream();

		Receive();
	}

	private async void Receive()
	{
		while (stream != null)
		{
			Command command = Command.Parser.ParseDelimitedFrom(stream);
			ParseCommand(command);
			await Task.Delay(500);
		}
	}

	private void ParseCommand(Command command)
	{
		switch (command.CommandType)
		{
			case Command.Types.CommandType.Hierarchy:
				Hierarchy hierarchyCommand = new Hierarchy();
				hierarchyCommand.GetHierarchy();
				hierarchyCommand.WriteDelimitedTo(stream);
				break;
			case Command.Types.CommandType.Add:
				break;
			case Command.Types.CommandType.Delete:
				break;
			default:
				break;
		}
	}

	private IPAddress GetLocalIPAddress()
	{
		return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
	}
}
