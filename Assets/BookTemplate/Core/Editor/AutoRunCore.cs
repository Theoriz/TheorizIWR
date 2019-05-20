using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityEditor.SceneManagement;

class EditorScrips : EditorWindow {

	[MenuItem("Play/Execute starting scene _%h")]
	public static void RunMainScene() {
		string currentSceneName = EditorSceneManager.GetActiveScene().path;

		if (currentSceneName != "Assets/Core/Core.unity") {
			File.WriteAllText(".lastScene", currentSceneName);
		}

		EditorSceneManager.OpenScene("Assets/BookTemplate/Core/Core.unity");
		EditorApplication.isPlaying = true;
	}

	[MenuItem("Play/Reload editing scene _%g")]
	public static void ReturnToLastScene() {
		string lastScene = File.ReadAllText(".lastScene");
		EditorSceneManager.OpenScene(lastScene);
	}
}