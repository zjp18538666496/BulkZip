# 文件批量压缩工具

### 功能说明

该工具的主要功能是根据用户指定的配置，从多个源文件夹中批量压缩特定格式的文件，并将生成的压缩包保存到目标路径。支持是否包含子文件夹的选项，适用于不同的压缩需求。

## 环境要求

- **开发环境**： .NET Framework 4.5
- **使用环境**：
  - .NET Framework 4.5 及以上版本
  - Windows 操作系统

此工具专为 .NET Framework 4.5 环境开发，旨在批量压缩文件夹中的特定文件格式，并按用户指定的路径保存。

### 使用示例

在 `config.json` 文件中配置批量压缩任务，该文件与应用程序放置在同一目录下。

```json
[
  {
    "name": "ApplicationServer",
    "fileName": "dll.[yyyyMMdd].update.zip",
    "sourceFolderPaths": [
      "D:\\wis\\WisCode\\Wis.ApplicationServer\\Wis.ApplicationServer\\bin\\Debug"
    ],
    "fileFormats": [
      "*.pdb",
      "*.dll",
      "*.xml",
      "*.config"
    ],
    "zipDestinationFolderPath": "D:\\data\\wis\\",
    "destinationFolderPath": "D:\\data\\wis\\dll",
    "includeSubfolders": false
  },
  {
    "name": "ServiceInterface",
    "fileName": "bin.[yyyyMMdd].update.zip",
    "sourceFolderPaths": [
      "D:\\wis\\WisCode\\Wis.ServiceInterface\\Wis.ServiceInterface\\bin"
    ],
    "fileFormats": [
      "*.pdb",
      "*.dll",
      "*.xml",
      "*.config"
    ],
    "zipDestinationFolderPath": "D:\\data\\wis\\",
    "destinationFolderPath": "D:\\data\\wis\\dll",
    "includeSubfolders": false
  }
]
```

## 配置说明

| 字段名                       | 说明                                                         | 类型       | 默认值  |
| ---------------------------- | ------------------------------------------------------------ | ---------- | ------- |
| **Name**                     | 压缩任务的名称，用于区分不同任务。                           | `string`   | 无      |
| **FileName**                 | 生成的压缩包名称，支持日期格式 `[yyyyMMdd]`。                | `string`   | 无      |
| **SourceFolderPaths**        | 源文件夹路径的数组，支持同时从多个文件夹中选择文件。         | `string[]` | 无      |
| **FileFormats**              | 需要压缩的文件格式，例如 `["*.dll", "*.xml"]`，支持使用通配符。 | `string[]` | 无      |
| **ZipDestinationFolderPath** | 压缩包保存的目标路径。                                       | `string`   | 无      |
| **DestinationFolderPath**    | 虚拟文件夹路径或目标路径，压缩包文件的最终保存位置。         | `string`   | 无      |
| **IncludeSubfolders**        | 是否包含源文件夹的子文件夹，默认不包含。                     | `bool`     | `false` |

### 项目贡献

如果你想为该项目做出贡献，请提交 Pull Request，或在 GitHub 上报告问题。
