namespace Root_Dir_Ini
{
    public class RootDir
    {
        string IniFilePath;
        string ShowName;
        public RootDir() 
        {
            ShowName = string.Empty;
            IniFilePath = @"_dir.ini";
        }
        public RootDir(string? rootDirNameToShow, string? iniFilePath)
        {
            ShowName = rootDirNameToShow ?? string.Empty;
            IniFilePath = iniFilePath ?? @"_dir.ini";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>如果获取路径失败返回null</returns>
        public string? GetDir()
        {
            Console.Write($"input {ShowName} root directory or enter for default: ");
            var read = Console.ReadLine();
            if (read != null && Directory.Exists(read))
            {
                StreamWriter sw = new(new FileStream(IniFilePath, FileMode.Create));
                sw.WriteLine(read);
                sw.Close();
                return read;
            }
            else if (File.Exists(IniFilePath))
            {
                StreamReader sr = new(new FileStream(IniFilePath, FileMode.Open));
                read = sr.ReadToEnd().Trim();
                sr.Close();
                return read;
            }
            else
            {
                Console.WriteLine("there is no default path. input any to exit.");
                Console.ReadKey();
                return null;
            }
        }
    }
}