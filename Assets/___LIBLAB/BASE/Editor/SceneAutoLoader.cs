using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using LibLabSystem;

[InitializeOnLoad]
public static class SceneAutoLoader
{
    static SceneAutoLoader()
	{
        if (EditorBuildSettings.scenes.Length > 0) return;

		IndexBuildScene();
	}

    static private List<SceneAsset> sceneAssets;

    static private void IndexBuildScene()
    {
        sceneAssets = new List<SceneAsset>();

        sceneAssets.Add((SceneAsset)AssetDatabase.LoadAssetAtPath("Assets/___LIBLAB/0___ LibLab Init.unity", typeof(SceneAsset)));
        sceneAssets.Add((SceneAsset)AssetDatabase.LoadAssetAtPath("Assets/NEW GAME/Scenes/NewGameScene.unity", typeof(SceneAsset)));

        if (sceneAssets[1] == null) return;

        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        foreach (var sceneAsset in sceneAssets)
        {
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            if (!string.IsNullOrEmpty(scenePath))
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }

        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

        LLLog.Log("SceneAutoLoader", "Builds scenes was initialized by LibLabSystem");
    }
}