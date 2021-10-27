using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestroyCommand : ICommand
{
	public string GetString(params string[] parameters)
	{
		List<string> destroyed = new List<string>();

		GameObject[] all = GameObjectExtensions.FindAllObjectsInScene();
		int[] indices = parameters.Select(x => int.Parse(x)).ToArray();
		foreach (var index in indices)
		{
			Object.Destroy(all[index]);
			destroyed.Add($"\"{all[index].name}\"");
		}

		return $"Destroyed {string.Join(", ", destroyed)}";
	}
}
