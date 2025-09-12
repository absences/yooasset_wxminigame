using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using WeChatWASM;
using YooAsset;
using YooAsset.Editor;
using static UnityEngine.GraphicsBuffer;

public class AutoBuild
{
    const string PackageName = "DefaultPackage";


    [MenuItem("Build/BuildAssets")]

    public static void BuildAssets()
    {
        //临时移除字体
        var target = EditorUserBuildSettings.activeBuildTarget;


        YooBuild(target);

    }

    static void YooBuild(BuildTarget buildTarget)
    {
        ScriptableBuildParameters buildParameters = new ScriptableBuildParameters();

        buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();

        buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();

        buildParameters.BuildPipeline = EBuildPipeline.ScriptableBuildPipeline.ToString();

        buildParameters.BuildTarget = buildTarget;

        buildParameters.EnableSharePackRule = true;

        buildParameters.VerifyBuildingResult = true;

        buildParameters.PackageVersion = GetDefaultPackageVersion();

        buildParameters.PackageName = PackageName;

        buildParameters.EncryptionServices = null;

        buildParameters.CompressOption = ECompressOption.LZ4;

        buildParameters.FileNameStyle = EFileNameStyle.HashName;

        buildParameters.ClearBuildCacheFiles = false;

        buildParameters.UseAssetDependencyDB = true;

        buildParameters.BuildBundleType = (int)EBuildBundleType.AssetBundle;

        buildParameters.BuiltinShadersBundleName = GetBuiltinShaderBundleName(PackageName);

        buildParameters.BuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyAll;

        ScriptableBuildPipeline pipeline = new ScriptableBuildPipeline();

        var buildResult = pipeline.Run(buildParameters, true);

        if (buildResult.Success)
            Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
        else
        {
            throw new Exception($"构建失败 : {buildResult.ErrorInfo}");
        }
    }

    /// <summary>
    /// 内置着色器资源包名称
    /// 注意：和自动收集的着色器资源包名保持一致！
    /// </summary>
    private static string GetBuiltinShaderBundleName(string PackageName)
    {
        var uniqueBundleName = AssetBundleCollectorSettingData.Setting.UniqueBundleName;
        var packRuleResult = DefaultPackRule.CreateShadersPackRuleResult();
        return packRuleResult.GetBundleName(PackageName, uniqueBundleName);
    }
    /// <summary>
    /// 获取默认版本
    /// </summary>
    protected static string GetDefaultPackageVersion()
    {
        int totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
        return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
    }
    [MenuItem("Build/BuildPlatform")]
    public static void BuildWXMiniGame()
    {
        //

        TMP_Settings.defaultFontAsset = null;

        var scene = EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);

        var gameEnter = GameObject.Find("GameEnter");

        const string CDN = "https://xxxx/webgl/";
        if (gameEnter != null)
        {
            var enter = gameEnter.GetComponent<GameEnter>();
            enter.CDN = CDN;
            EditorSceneManager.SaveScene(scene);
        }
        WXConvertCore.config.ProjectConf.Appid = "xxxx";
        WXConvertCore.config.ProjectConf.bundleExcludeExtensions = "json;version;";

        var path = string.Format("{0}/../AutoBuild/wx_export", Application.dataPath);

        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);

        WXConvertCore.config.ProjectConf.relativeDST = path;

        WXConvertCore.config.ProjectConf.CDN = CDN;

        EditorUtility.SetDirty(WXConvertCore.config);

        AssetDatabase.SaveAssets();


        AssetDatabase.Refresh();

        WXConvertCore.DoExport();
    }
}
