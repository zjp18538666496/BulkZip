#### 批量压缩文件

#### 配置项：

```c#
    /// <summary>
    /// 配置名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 文件名称
    /// </summary>
    public string fileName { get; set; }
    /// <summary>
    /// 源文件夹路径
    /// </summary>
    public string[] SourceFolderPaths { get; set; }
    /// <summary>
    /// 要压缩的文件格式
    /// </summary>
    public string[] FileFormats { get; set; }
    /// <summary>
    /// 压缩包文件夹路径
    /// </summary>
    public string ZipDestinationFolderPath { get; set; }
    /// <summary>
    /// 虚拟文件夹路径
    /// </summary>
    public string DestinationFolderPath { get; set; }
    /// <summary>
    /// 是否包含子文件夹
    /// </summary>
    public bool IncludeSubfolders { get; set; } = false;
```

