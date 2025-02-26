using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

public class GameEnter : MonoBehaviour
{
    public string CDN = "http://192.168.1.94/";
    public TextMeshProUGUI tip;

    public Button clickBtn;
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
#if UNITY_EDITOR
        var buildResult = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
        var packageRoot = buildResult.PackageRootDirectory;

        initializeParameters = new EditorSimulateModeParameters()
        {
            EditorFileSystemParameters =
                FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot)
        };


#elif WEIXINMINIGAME

        string packageRoot = $"{WeChatWASM.WX.env.USER_DATA_PATH}/__GAME_FILE_CACHE";

        initializeParameters = new WebPlayModeParameters()
        {
            WebRemoteFileSystemParameters =
            WechatFileSystemCreater.CreateFileSystemParameters(packageRoot, new RemoveServer(CDN)),
        };
#endif

        var init = DefaultPackage.InitializeAsync(initializeParameters);

        await init;

        var version = DefaultPackage.RequestPackageVersionAsync();

        await version;
        var update = DefaultPackage.UpdatePackageManifestAsync(version.PackageVersion);

        await update;
        //can load assets
        await InitTmpAsset();
#if WEIXINMINIGAME
        
        //更新字体 事先隐藏的就不需要
        // tip.UpdateFontAsset();
        // tip.SetAllDirty();

        tip.text = "1、修改转换工具cdn地址 、appid、设置导出路径\n2、修改GameEnter CDN\n3、添加TMP_SDF-Mobile着色器到内置shaders清单";
        tip.gameObject.SetActive(true);
        var download = DefaultPackage.CreateResourceDownloader(3, 10);

        var a = 0;
        clickBtn.gameObject.SetActive(true);
        clickBtn.onClick.AddListener(() =>
        {
            a++;
            tip.text = a.ToString();
        });

        download.BeginDownload();

        await download;

        var loadHandle = DefaultPackage.LoadAssetAsync<GameObject>("GameObject");

        await loadHandle;

        var instant = loadHandle.InstantiateAsync();

        await instant;

        var _wxFileSystemMgr = WeChatWASM.WX.GetFileSystemManager();

        //测试读取 sa 资源
        _wxFileSystemMgr.ReadFile(new WeChatWASM.ReadFileParam()
        {
            filePath = WeChatWASM.WX.env.USER_DATA_PATH + "/StreamingAssets/aa.png",
            success = (success) =>
            {
                Debug.Log("load success");
            },
            fail = (fail) =>
            {

            }
        });
#else
        tip.text = "这是编辑器模式";
        tip.gameObject.SetActive(true);
#endif
    }
#if WEIXINMINIGAME
    async UniTask InitTmpAsset()
    {
        var fallbackFont = CDN + "AlibabaPuHuiTi-2-65-Medium.ttf";

        UniTaskCompletionSource<bool> source = new UniTaskCompletionSource<bool>();

        WeChatWASM.WX.GetWXFont(fallbackFont, (code, font) =>
        {
            Debug.Log("get font: code:" + code + " font:" + font);

            if (font != null)
            {
                //注意：需要将shader: TMP_SDF-Mobile 添加到editor Graphics included Shaders内置着色器列表
                var tmp_font = TMP_FontAsset.CreateFontAsset(font);

                TMP_Text.OnFontAssetRequest += (hashcode, asset) =>
                {
                    return tmp_font;
                };

                TMP_Settings.defaultFontAsset = tmp_font;

                Debug.Log("load font success");
            }
            source.TrySetResult(font != null);
        });

        await source.Task;
    }
#else
    async UniTask InitTmpAsset()
    {
        //出小游戏包，可以不打包字体，使用微信字体，减少包体大小
        var handle = DefaultPackage.LoadAssetAsync<TMP_FontAsset>("alibaba");

        await handle;
        var tmp_font = handle.AssetObject as TMP_FontAsset;
        TMP_Text.OnFontAssetRequest += (hashcode, asset) =>
        {
            return tmp_font;
        };

        TMP_Settings.defaultFontAsset = tmp_font;
    }
#endif

    class RemoveServer : IRemoteServices
    {
        //注意微信CDN地址与Yoo远端加载地址需一致，才会触发缓存
        //https://wechat-miniprogram.github.io/minigame-unity-webgl-transform/Design/FileCache.html

        string CDN;
        public RemoveServer(string cdn)
        {
            CDN = cdn;
        }

        //远端目录结构为：
        //CDN:
        //    StreamingAssets
        //    xxwebgl.wasm.code.unityweb.wasm.br

        //    xxx.version
        //    xxx.hash
        //    xx/bundle

        //    xx.ttf 备用字体
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
