using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace PocketBlocks.Editor {
[InitializeOnLoad]
public static class ProjectSetup {
    const string ScenePath="Assets/Scenes/Main.unity";
    static ProjectSetup(){EditorApplication.delayCall+=()=>{if(!File.Exists(ScenePath)){Configure();CreateScene();}};}
    [MenuItem("Pocket Blocks/Configure Android")]
    public static void Configure(){
        PlayerSettings.companyName="Pocket Studio";PlayerSettings.productName="Pocket Blocks";
        PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android,"com.pocketstudio.pocketblocks");
        PlayerSettings.bundleVersion="1.0.0";PlayerSettings.Android.bundleVersionCode=1;
        PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToLandscapeLeft=false;PlayerSettings.allowedAutorotateToLandscapeRight=false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
        PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion=AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android,false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,new[]{GraphicsDeviceType.OpenGLES3});
        PlayerSettings.Android.applicationEntry=AndroidApplicationEntry.Activity;
        PlayerSettings.Android.forceInternetPermission=false;
        var settings=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
        var input=settings.FindProperty("activeInputHandler");if(input!=null){input.intValue=0;settings.ApplyModifiedPropertiesWithoutUndo();}
        PlayerSettings.Android.renderOutsideSafeArea=true;
        PlayerSettings.runInBackground=false;
        PlayerSettings.Android.useCustomKeystore=false;
        QualitySettings.vSyncCount=0;QualitySettings.antiAliasing=0;
        AssetDatabase.SaveAssets();
    }
    [MenuItem("Pocket Blocks/Create Main Scene")]
    public static void CreateScene(){Directory.CreateDirectory("Assets/Scenes");var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorSceneManager.SaveScene(scene,ScenePath);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};AssetDatabase.Refresh();}
    [MenuItem("Pocket Blocks/Build Android APK")]
    public static void BuildApk(){Configure();if(!File.Exists(ScenePath))CreateScene();Directory.CreateDirectory("Builds");EditorUserBuildSettings.buildAppBundle=false;var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="Builds/PocketBlocks.apk",target=BuildTarget.Android,options=BuildOptions.None});if(result.summary.result!=BuildResult.Succeeded)throw new Exception("APK build failed: "+result.summary.result);Debug.Log("APK ready: "+Path.GetFullPath("Builds/PocketBlocks.apk"));}
}
}
