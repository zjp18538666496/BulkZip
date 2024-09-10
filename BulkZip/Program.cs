using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

class Program
{
    static void Main()
    {
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // 从 JSON 文件加载配置
            string jsonConfigPath = "config.json";
            List<Config> configs = LoadConfigs(jsonConfigPath);

            // 遍历每个配置项
            foreach (var config in configs)
            {
                string[] sourceFolderPaths = config.SourceFolderPaths;
                string[] fileFormats = config.FileFormats;
                string zipDestinationFolderPath = config.ZipDestinationFolderPath;
                string destinationFolderPath = config.DestinationFolderPath;
                // 压缩包名称
                string zipFileName = getFileName(config.fileName);
                // 压缩包完整路径
                string zipFilePath = Path.Combine(zipDestinationFolderPath, zipFileName);
                Console.WriteLine($"处理配置: {config.Name}");
                // 输出配置信息
                Console.WriteLine("要压缩的文件夹: " + string.Join(", ", sourceFolderPaths));
                Console.WriteLine("要压缩的文件格式: " + string.Join(", ", fileFormats));
                Console.WriteLine("压缩包文件夹: " + zipDestinationFolderPath);
                Console.WriteLine("压缩包名称: " + zipFileName);

                Console.WriteLine("开始创建虚拟文件夹...");
                // 创建目标文件夹
                if (Directory.Exists(destinationFolderPath))
                {
                    Directory.Delete(destinationFolderPath, true);
                    Directory.CreateDirectory(destinationFolderPath);
                }
                else
                {
                    Directory.CreateDirectory(destinationFolderPath);
                }
                Console.WriteLine("创建虚拟文件夹完成");

                Console.WriteLine("开始复制文件到虚拟文件夹...");
                // 遍历源文件夹
                foreach (string sourceFolderPath in sourceFolderPaths)
                {
                    // 判断源文件夹是否存在
                    if (!Directory.Exists(sourceFolderPath))
                    {
                        Console.WriteLine("源文件夹不存在: " + sourceFolderPath);
                        continue; // 跳过当前循环，继续下一个文件夹路径
                    }

                    // 设置是否包含子文件夹
                    SearchOption searchOption = config.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                    // 遍历文件格式
                    foreach (string format in fileFormats)
                    {
                        // 获取当前文件格式的文件列表（根据是否包含子文件夹）
                        string[] files = Directory.GetFiles(sourceFolderPath, format, searchOption);
                        foreach (string file in files)
                        {
                            // 判断文件是否存在
                            if (!File.Exists(file))
                            {
                                Console.WriteLine("文件不存在: " + file);
                                continue; // 跳过当前循环，继续下一个文件
                            }

                            // 计算文件的相对路径（相对于源文件夹）
                            string relativePath = GetRelativePath(sourceFolderPath, file);

                            // 确定文件在目标文件夹中的完整路径
                            string destFile = Path.Combine(destinationFolderPath, relativePath);

                            // 获取文件所在的目录路径
                            string destDir = Path.GetDirectoryName(destFile);

                            // 如果目标目录不存在，则创建
                            if (!Directory.Exists(destDir))
                            {
                                Directory.CreateDirectory(destDir);
                            }

                            // 复制文件到目标目录
                            File.Copy(file, destFile, true);
                        }
                    }
                }

                Console.WriteLine("复制文件到虚拟文件夹完成");

                Console.WriteLine("开始压缩文件...");
                // 判断压缩文件是否已存在
                if (File.Exists(zipFilePath))
                {
                    Console.WriteLine("压缩文件已存在，将覆盖现有文件: " + zipFilePath);
                    File.Delete(zipFilePath);
                }

                // 将目标文件夹中的文件压缩成一个新的压缩包
                using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                    {
                        foreach (string filePath in Directory.GetFiles(destinationFolderPath, "*", SearchOption.AllDirectories))
                        {
                            // 计算文件在虚拟文件夹中的相对路径
                            string relativePathInZip = GetRelativePath(destinationFolderPath, filePath);

                            // 将文件添加到压缩包中，并保持其相对路径结构
                            archive.CreateEntryFromFile(filePath, relativePathInZip);
                        }
                    }
                }
                Console.WriteLine("压缩文件完成");

                Console.WriteLine("开始删除虚拟文件夹...");
                // 删除指定的文件夹
                Directory.Delete(destinationFolderPath, true);
                Console.WriteLine("删除虚拟文件夹完成");
                Console.WriteLine("---------------------------");
            }
            Console.WriteLine("");
            stopwatch.Stop();
            Console.WriteLine($"本次操作共计 {stopwatch.ElapsedMilliseconds / 1000} 秒");
            Console.WriteLine("");

            // 提示用户输入
            Console.Write("输入‘1’并回车打开压缩文件路径并退出，或回车直接退出: ");
            string userInput = Console.ReadLine();

            // 如果用户输入为1，则打开压缩包路径
            if (userInput == "1")
            {
                // 获取所有不同的压缩文件夹路径
                var distinctPaths = configs.Select(c => c.ZipDestinationFolderPath).Distinct();

                // 遍历所有不同的路径并打开
                foreach (var path in distinctPaths)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c start " + path,
                        Verb = "runas", // 以管理员身份运行
                        CreateNoWindow = true,  // 隐藏cmd窗口
                        UseShellExecute = false // 确保不显示命令行窗口
                    };
                    Process.Start(startInfo);
                }
            }

            // 如果用户输入为其他值，直接关闭
            Console.WriteLine("关闭窗口...");
            System.Threading.Thread.Sleep(1000);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            Console.Write("按任何其他键退出: ");
            Console.ReadLine();
        }
    }

    // 从 JSON 文件加载配置列表
    static List<Config> LoadConfigs(string filePath)
    {
        using (StreamReader file = File.OpenText(filePath))
        {
            JsonSerializer serializer = new JsonSerializer();
            return (List<Config>)serializer.Deserialize(file, typeof(List<Config>));
        }
    }

    static string getFileName(string str)
    {
        string fileName;
        Match match = Regex.Match(str, @"\[(.*?)\]");
        if (match.Success)
        {
            string dateFormat = match.Groups[1].Value;
            try
            {
                string formattedDate = DateTime.Now.ToString(dateFormat);
                fileName = Regex.Replace(str, @"\[(.*?)\]", formattedDate);
            }
            catch (FormatException)
            {
                fileName = str;
            }
        }
        else
        {
            fileName = str;
        }
        return fileName;
    }
    // 手动计算相对路径的方法
    static string GetRelativePath(string basePath, string fullPath)
    {
        Uri baseUri = new Uri(basePath.EndsWith("\\") ? basePath : basePath + "\\");
        Uri fullUri = new Uri(fullPath);
        return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar));
    }
}

class Config
{
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
}
