using System.Diagnostics;
using UnityEditor;
using UnityEngine;

// Taken from: https://www.appsfresh.com/blog/multiplayer/
public static class MultiplayersBuildAndRun
{
    [MenuItem("File/Run Multiplayer/1 Player")]
    static void PerformWin64Build1()
    {
        PerformWin64Build(1);
    }
    
    [MenuItem("File/Run Multiplayer/2 Players")]
    static void PerformWin64Build2()
    {
        PerformWin64Build(2);
    }

    [MenuItem("File/Run Multiplayer/3 Players")]
    static void PerformWin64Build3()
    {
        PerformWin64Build(3);
    }

    [MenuItem("File/Run Multiplayer/4 Players")]
    static void PerformWin64Build4()
    {
        PerformWin64Build(4);
    }

    static void PerformWin64Build(int playerCount)
    {
        string path = Application.dataPath;
        path = path.Substring(0, path.LastIndexOf("/")) + "/Builds/" + GetProjectName() + ".exe";
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        BuildPlayerOptions bpo = new BuildPlayerOptions
        {
            scenes = GetScenePaths(),
            target = BuildTarget.StandaloneWindows64,
            locationPathName = path
        };
        BuildPipeline.BuildPlayer(bpo);
        for (int i = 1; i <= playerCount; i++)
        {
            Process.Start(path);
        }
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }

}