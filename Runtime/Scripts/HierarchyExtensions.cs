using Remotedebugger;
using System.Collections.Generic;
using UnityEngine;
using static Remotedebugger.Hierarchy.Types;
using static Remotedebugger.Hierarchy.Types.GameObj.Types;

public static class HierarchyExtensions
{
	public static void GetHierarchy(this Hierarchy hierarchy)
	{
		GameObject[] all = GameObjectExtensions.FindAllObjectsInScene();
		for (int i = 0; i < all.Length; i++)
		{
			GameObj gameObj = NewGameObj(i, all[i]);

			// Also loop through their sub children
			GameObject[] children = all[i].GetChildren();
			for (int j = 0; j < children.Length; j++)
			{
				GameObj child = NewGameObj(j, children[j]);
				gameObj.Children.Add(child);

				// TODO, MAKE RECURSIVE???
			}

			hierarchy.GameObjects.Add(gameObj);
		}
	}

	private static GameObj NewGameObj(int index, GameObject gameObject)
	{
		GameObj gameObj = new GameObj
		{
			Index = index,
			Name = gameObject.name,
			Transform = GetTransform(gameObject.transform)
		};
		gameObj.Components.AddRange(GetComponents(gameObject));

		return gameObj;
	}

	private static List<Compo> GetComponents(GameObject gameObject)
	{
		List<Compo> compos = new List<Compo>();
		Component[] components = gameObject.GetComponents<Component>();
		for (int i = 0; i < components.Length; i++)
		{
			Compo compo = new Compo
			{
				Index = i,
				Name = components[i].GetType().Name
			};
			compos.Add(compo);
		}
		return compos;
	}

	private static Transfo GetTransform(Transform transform)
	{
		return new Transfo()
		{
			Position = new Transfo.Types.Vector() { X = transform.localPosition.x, Y = transform.localPosition.y, Z = transform.localPosition.z },
			Rotation = new Transfo.Types.Vector() { X = transform.localEulerAngles.x, Y = transform.localEulerAngles.y, Z = transform.localEulerAngles.z },
			Scale = new Transfo.Types.Vector() { X = transform.localScale.x, Y = transform.localScale.y, Z = transform.localScale.z },
		};
	}

	public static void ApplyTransforms(this Transform transform, Transfo transfo)
	{
		Vector3 position = new Vector3(transfo.Position.X, transfo.Position.Y, transfo.Position.Z);
		Vector3 rotation = new Vector3(transfo.Rotation.X, transfo.Rotation.Y, transfo.Rotation.Z);
		Vector3 scale = new Vector3(transfo.Scale.X, transfo.Scale.Y, transfo.Scale.Z);

		transform.localPosition = position;
		transform.localEulerAngles = rotation;
		transform.localScale = scale;
	}
}
