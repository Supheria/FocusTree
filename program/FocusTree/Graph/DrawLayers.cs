namespace FocusTree.Graph
{
    /// <summary>
    /// 绘制委托类型
    /// </summary>
    /// <param name="g">传入栅格的 GDI</param>
    public delegate void Drawer(Bitmap image);
    /// <summary>
    /// 绘制层级
    /// </summary>
    public class DrawLayers
    {
        Drawer[] Layers;
        public int LayerNumber => Layers.Length;
        /// <summary>
        /// 默认构造函数：1层
        /// </summary>
        public DrawLayers() => Layers = new Drawer[1];
        /// <summary>
        /// 创建给定层级数的 Drawer 数组
        /// </summary>
        /// <param name="layerNumber"></param>
        public DrawLayers(uint layerNumber) => Layers = new Drawer[layerNumber];
        /// <summary>
        /// 按层级序号顺序激发所有层级的委托方法
        /// </summary>
        /// <param name="image"></param>
        public void Invoke(Bitmap image) => Layers.ToList().ForEach(x => x?.Invoke(image));
        /// <summary>
        /// 激发指定层级的委托
        /// </summary>
        /// <param name="image"></param>
        /// <param name="layerIndex"></param>
        public void Invoke(Bitmap image, uint layerIndex) => Layers[layerIndex].Invoke(image);
        /// <summary>
        /// 清空所有层级的委托
        /// </summary>
        public void Clear() => Layers = new Drawer[Layers.Length];
        /// <summary>
        /// 清空指定层级的委托
        /// </summary>
        /// <param name="layerIndex"></param>
        public void Clear(uint layerIndex) => Layers[layerIndex] = null;
        /// <summary>
        /// 所有层级的委托方法个数
        /// </summary>
        public int MethodNumber() => Layers.Sum(x => x == null ? 0 : x.GetInvocationList().Length);
        /// <summary>
        /// 指定层级的委托方法个数
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public int MethodNumber(uint layerIndex) => Layers[layerIndex] == null ? 0 : Layers[layerIndex].GetInvocationList().Length;
        /// <summary>
        /// 添加 drawer 到指定层级
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="drawer"></param>
        /// <returns></returns>
        public static DrawLayers operator +(DrawLayers layer, LayerDrawer drawer)
        {
            layer.Layers[drawer.LayerIndex] += drawer.Method;
            return layer;
        }
        /// <summary>
        /// 删减指定层级的 drawer
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="drawer"></param>
        /// <returns></returns>
        public static DrawLayers operator -(DrawLayers layer, LayerDrawer drawer)
        {
            layer.Layers[drawer.LayerIndex] -= drawer.Method;
            return layer;
        }
    }
    public struct LayerDrawer
    {
        /// <summary>
        /// 层级序号
        /// </summary>
        public uint LayerIndex { get; init; }
        /// <summary>
        /// 委托方法
        /// </summary>
        public Drawer Method { get; init; }
        /// <summary>
        /// 默认构造函数：层级序号0，委托为 null
        /// </summary>
        public LayerDrawer()
        {
            LayerIndex = 0;
            Method = null;
        }
        /// <summary>
        /// 根据给定层级序号和委托方法创建
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="method"></param>
        public LayerDrawer(uint layer, Drawer method)
        {
            LayerIndex = layer;
            Method = method;
        }
    }
}
