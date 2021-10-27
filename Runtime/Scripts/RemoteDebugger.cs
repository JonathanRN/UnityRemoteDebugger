using System.Net.Sockets;
using UnityEngine;
using Google.Protobuf;
using Remotedebugger;
using System.Collections.Generic;

public class RemoteDebugger : MonoBehaviour
{
	private TcpClient tcpClient;
	private NetworkStream stream;
	private List<GameObject> hierarchy;

	public string Hostname { get; set; }

	private void Awake()
	{
		hierarchy = new List<GameObject>();
	}

	public async void Connect()
	{
		string[] split = Hostname.Split(':');
		string host = split[0];
		int port = int.Parse(split[1]);

		tcpClient = new TcpClient();
		await tcpClient.ConnectAsync(host, port);
		stream = tcpClient.GetStream();

		Debug.Log("Connected.");
	}

	public void FetchHierarchy()
	{
		if (!tcpClient.Connected || stream == null)
		{
			Debug.LogError("Client isn't connected.");
			return;
		}

		Command request = new Command
		{
			CommandType = Command.Types.CommandType.Hierarchy
		};

		request.WriteDelimitedTo(stream);

		Hierarchy response = Hierarchy.Parser.ParseDelimitedFrom(stream);

		DestroyAllChildren();

		foreach (var gameObj in response.GameObjects)
		{
			GameObject gameObject = new GameObject(gameObj.Name);
			gameObject.SetActive(gameObj.Enabled);
			gameObject.transform.ApplyTransforms(gameObj.Transform);

			hierarchy.Add(gameObject);

			foreach (var child in gameObj.Children)
			{
				GameObject childObject = new GameObject(child.Name);
				childObject.SetActive(child.Enabled);
				childObject.transform.SetParent(gameObject.transform);
				childObject.transform.ApplyTransforms(child.Transform);

				hierarchy.Add(childObject);
			}

			// todo list components
		}
	}

	public void Disconnect()
	{
		stream?.Close();
		tcpClient?.Close();
	}

	private void DestroyAllChildren()
	{
		for (int i = 0; i < hierarchy.Count; i++)
		{
			Object.DestroyImmediate(hierarchy[i]);
		}
		hierarchy.Clear();
	}

	private void OnApplicationQuit()
	{
		Disconnect();
	}
}
