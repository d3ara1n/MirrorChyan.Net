# MirrorCHyan.Net

[Mirror 酱](https://mirrorchyan.com/) 的 .Net Api 封装。

## Getting Started

这个库在 Nuget 上发布，可以通过以下命令安装：

```bash
dotnet add package MirrorChyan.Net
```

### 使用依赖注入

```csharp
using MirrorChyan.Net;
using MirrorChyan.Net.Models;
using MirrorChyan.Net.Services;

// 注册服务
services.AddMirrorChyan(productId: "maa", clientName: "MaaWpfGui", currentVersion: "v1.0.0");
var provider = services.BuildServiceProvider();

// 使用服务
var service = provider.GetRequiredService<IMirrorChyanService>();
// 当没有提供 CDK 时只能进行版本查询，得到的 `VersionModel.Artifact` 为空，不可进一步下载安装
var version = await service.GetLatestVersionAsync(cdk: null, ChannelKind.Stable);
```

### 直接使用裸接口

```csharp
using MirrorChyan.Net;
using MirrorChyan.Net.Models;
using MirrorChyan.Net.Services;

// 创建服务
var service = IMirrorChyanService.Create(new Uri("https://mirrorchyan.com"), new MirrorChyanOptions
{
    ProductId = "maa",
    ClientName = "MaaWpfGui",
    VersionString = "v1.0.0"
});

// 使用客户端
var version = await service.GetLatestVersionAsync(cdk: null, ChannelKind.Stable);
```

## Deep Dive

没有深入。Mirror酱只提供了一个接口，所以这个库也只封装了一个接口。如果需要进一步的用法例如打包与安装，请使用扩展库。

## Extensions

- Velopack 更新扩展: [VelopackExtension.MirrorChyan](https://github.com/d3ara1n/VelopackExtension.MirrorChyan)
