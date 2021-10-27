using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class RemoteDebuggerEditor
{
    private const string MENU_NAME = "Remote Debugger/Test Locally";

    private static bool testingLocally;

    static RemoteDebuggerEditor()
	{
        testingLocally = EditorPrefs.GetBool(MENU_NAME, false);

        // Delaying until first editor tick so that the menu
        // will be populated before setting check state, and
        // re-apply correct action
        EditorApplication.delayCall += () => {
            ToggleTestingLocally(testingLocally);
        };
    }

    [MenuItem(MENU_NAME)]
    private static void TestLocally()
    {
        ToggleTestingLocally(!testingLocally);
    }

    public static void ToggleTestingLocally(bool enabled)
    {
        Menu.SetChecked(MENU_NAME, enabled);
        EditorPrefs.SetBool(MENU_NAME, enabled);
        testingLocally = enabled;

        Debug.Log("Testing locally is now " + (testingLocally ? "ON" : "OFF"));
    }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void OnPlay()
	{
		if (testingLocally)
		{
		    EditorSceneManager.LoadSceneAsyncInPlayMode(EditorBuildSettings.scenes[0].path, new LoadSceneParameters() { loadSceneMode = LoadSceneMode.Additive });
            // Start the server
            new Server();

			var ip = Server.GetLocalIPAddress();
			var remoteDebugger = Object.FindObjectOfType<RemoteDebugger>();

            remoteDebugger.Hostname = $"{ip}:{Server.PORT}";
            remoteDebugger.Connect();
		}
	}
}
