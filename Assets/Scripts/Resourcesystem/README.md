> 先简单写一份能看的readme吧，等后面我再统一重写一下QAQ

# 打包 AssetBundle
在 Unity 编辑器中，选择要打包的资源（如预制体、纹理等）

在 Inspector 窗口底部设置 AssetBundle 名称和变体（可选）

通过菜单栏 Tools/AssetBundle/Packing 执行打包操作

打包完成后，控制台会显示输出路径（默认：项目根目录/Bundles/output）

# 加载 AssetBundle 资源
在游戏运行时使用以下代码加载 AssetBundle 中的资源：

'''csharp
// 加载 GameObject
GameObject playerPrefab = ABundleManager.LoadGameObjectFromBundle<GameObject>("characters", "Player");

// 加载纹理
Texture2D iconTexture = ABundleManager.LoadGameObjectFromBundle<Texture2D>("ui", "Icon");

// 加载音频
AudioClip soundEffect = ABundleManager.LoadGameObjectFromBundle<AudioClip>("sounds", "Explosion");
'''
# API 参考
BundleEditor 类
Pack() - 打包所有设置了 AssetBundle 名称的资源到指定目录

ABundleManager 类
LoadGameObjectFromBundle<T>(string bundlePath, string name) - 从指定 AssetBundle 加载指定名称的资源

bundlePath: AssetBundle 文件名（不需要扩展名）

name: 资源在 AssetBundle 中的名称

T: 资源类型（如 GameObject, Texture2D, AudioClip 等）