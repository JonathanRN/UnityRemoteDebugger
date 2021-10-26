using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// http://answers.unity.com/answers/1641475/view.html
    /// </summary>
    public static GameObject[] FindAllObjectsInScene(bool rootOnly = true)
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        GameObject[] rootObjects = activeScene.GetRootGameObjects();
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        List<GameObject> objectsInScene = new List<GameObject>();

        for (int i = 0; i < rootObjects.Length; i++)
        {
            objectsInScene.Add(rootObjects[i]);
        }

        if (!rootOnly)
        {
            for (int i = 0; i < allObjects.Length; i++)
            {
                if (allObjects[i].transform.root)
                {
                    for (int i2 = 0; i2 < rootObjects.Length; i2++)
                    {
                        if (allObjects[i].transform.root == rootObjects[i2].transform && allObjects[i] != rootObjects[i2])
                        {
                            objectsInScene.Add(allObjects[i]);
                            break;
                        }
                    }
                }
            }
        }
        return objectsInScene.ToArray();
    }

    public static GameObject[] GetChildren(GameObject gameObject)
	{
        List<GameObject> children = new List<GameObject>();
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
            children.Add(gameObject.transform.GetChild(i).gameObject);
		}
        return children.ToArray();
	}
}
