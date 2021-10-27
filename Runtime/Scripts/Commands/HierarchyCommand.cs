using System.Collections.Generic;
using UnityEngine;

public class HierarchyCommand : ICommand
{
	private GameObject[] previousList;

	public string GetString(params string[] parameters)
	{
		if (parameters.Length != 0)
		{
			// If an index is specified, list the children instead
			// todo: doesnt work for sub children
			int index = int.Parse(parameters[0]);
			previousList = GameObjectExtensions.GetChildren(previousList[index]);
		}
		else
		{
			previousList = GameObjectExtensions.FindAllObjectsInScene();
		}

        List<string> list = new List<string>();
		for (int i = 0; i < previousList.Length; i++)
		{
            Component[] components = GetAllComponentsFromGameObject(previousList[i]);

            if (!previousList[i].activeSelf)
			{
                list.Add($"{i}: [DISABLED] {previousList[i].name}");
			}
            else
			{
                list.Add($"{i}: {previousList[i].name}");
			}

			int childCount = previousList[i].transform.childCount;
			if (childCount > 0)
			{
				list[list.Count - 1] += $" ({childCount})";
			}

			foreach (var component in components)
			{
                string name = component.GetType().Name;

                if (component.GetType() == typeof(Transform))
				{
				    list.Add($"\t{name} {(component as Transform).localPosition}");
				}
                else
				{
				    list.Add($"\t{name}");
				}
			}
		}

        return string.Join(",\n", list);
	}

    private Component[] GetAllComponentsFromGameObject(GameObject go)
	{
        return go.GetComponents<Component>();
	}
}
