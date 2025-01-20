using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WeChatWASM;
using YooAsset;

public class GameEnter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InitResource().Forget();
    }

    private ResourcePackage DefaultPackage;

    async UniTask InitResource()
    {
        YooAssets.Initialize(null);

        YooAssets.SetOperationSystemMaxTimeSlice(1000);

        DefaultPackage = YooAssets.CreatePackage("DefaultPackage");
        YooAssets.SetDefaultPackage(DefaultPackage);

        InitializeParameters initializeParameters = null;

#if WEIXINMINIGAME
        initializeParameters = new WebPlayModeParameters()
        {
            WebRemoteFileSystemParameters = 
            WechatFileSystemCreater.CreateWechatFileSystemParameters(new RemoveServer(CDN)),
        };
#endif
        //initializeParameters = new EditorSimulateModeParameters()
        //{
        //    EditorFileSystemParameters =
        //     FileSystemParameters.CreateDefaultEditorFileSystemParameters(
        //         EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "DefaultPackage"))
        //};

        var init = DefaultPackage.InitializeAsync(initializeParameters);

        await init;


        var version = DefaultPackage.RequestPackageVersionAsync();

        await version;

        var update = DefaultPackage.UpdatePackageManifestAsync(version.PackageVersion);

        await update;

        var download = DefaultPackage.CreateResourceDownloader(3, 10);

        download.BeginDownload();

        await download;

        var loadHandle = DefaultPackage.LoadAssetAsync<GameObject>("Assets/GameObject.prefab");

        await loadHandle;

        var instant = loadHandle.InstantiateAsync();

        await instant;

        var request = UnityWebRequest.Get(CDN + "StreamingAssets/aa.png");

        await request.SendWebRequest();

        Debug.Log(request.result);


        var _wxFileSystemMgr = WX.GetFileSystemManager();

       // UniTaskCompletionSource<byte[]> task = new UniTaskCompletionSource<byte[]>();
        _wxFileSystemMgr.ReadFile(new ReadFileParam()
        {
            filePath = WX.env.USER_DATA_PATH + "/StreamingAssets/aa.png",
            success = (success) =>
            {
                Debug.Log("load success");
            },
            fail = (fail) =>
            {

            }
        });
    }

    public string CDN = "qq.com/";

    class RemoveServer : IRemoteServices
    {
        //注意CDN地址与Yoo远端加载地址需一致，才会触发缓存
        //https://wechat-miniprogram.github.io/minigame-unity-webgl-transform/Design/FileCache.html

        string CDN;
        public RemoveServer(string cdn)
        {
            CDN = cdn;
        }

        //如果不一致，需要修改缓存目录，_fileCacheRoot

        //远端目录结构为：
        //CDN:
        //webgl:
        //      StreamingAssets
        //      xxwebgl.wasm.code.unityweb.wasm.br

        //      xxx.version
        //      xxx.hash
        //      xx/bundle
        public string GetRemoteFallbackURL(string fileName)
        {
            return CDN + fileName;
        }

        public string GetRemoteMainURL(string fileName)
        {
            return CDN + fileName;
        }
    }
}
