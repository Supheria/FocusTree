using System.Drawing.Imaging;

public static class Program
{
    public static void Main()
    {
        ConvertImageToIcon("test.bmp", "test.ico", new Size(255, 255));
    }
    /// <summary>
    /// ICON图标文件头模板
    /// </summary>
    private static readonly byte[] ICON_HEAD_TEMPLATE = {
    0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x80,
    0x80, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00,
    0xC4, 0x6E, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00
};

    /// <summary>
    /// 图片转换为ico文件
    /// </summary>
    /// <param name="origin">原图片路径</param>
    /// <param name="destination">输出ico文件路径</param>
    /// <param name="iconSize">输出ico图标尺寸，不可大于255x255</param>
    /// <returns>是否转换成功</returns>
    public static bool ConvertImageToIcon(string origin, string destination, Size iconSize)
    {
        if (iconSize.Width > 255 || iconSize.Height > 255)
        {
            return false;
        }
        // 把原图并缩放到指定大小
        Image originResized = new Bitmap(Image.FromFile(origin), iconSize);
        // 存放缩放后的原图的内存流
        MemoryStream originImageStream = new MemoryStream();
        // 将缩放后的原图以png格式写入到内存流
        originResized.Save(originImageStream, ImageFormat.Png);
        // Icon的文件字节内容
        List<byte> iconBytes = new List<byte>();
        // 先加载Icon文件头
        iconBytes.AddRange(ICON_HEAD_TEMPLATE);
        // 文件头的第7和8位分别是图标的宽高，修改为设定值，不可大于255
        iconBytes[6] = (byte)iconSize.Width;
        iconBytes[7] = (byte)iconSize.Height;
        // 文件头的第15到第18位是原图片内容部分大小
        byte[] size = BitConverter.GetBytes((int)originImageStream.Length);
        iconBytes[14] = size[0];
        iconBytes[15] = size[1];
        iconBytes[16] = size[2];
        iconBytes[17] = size[3];
        // 追加缩放后原图字节内容
        iconBytes.AddRange(originImageStream.ToArray());
        // 利用文件流保存为Icon文件
        Stream iconFileStream = new FileStream(destination, FileMode.Create);
        iconFileStream.Write(iconBytes.ToArray(), 0, iconBytes.Count);
        // 关闭所有流并释放内存
        iconFileStream.Close();
        originImageStream.Close();
        originResized.Dispose();
        return File.Exists(destination);
    }

}