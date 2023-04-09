namespace FocusTree.Data
{
    public struct FormattedData
    {
        public string[] Items { get { return items; } }
        string[] items = Array.Empty<string>();
        public FormattedData(params string[] data)
        {
            items = data ?? Array.Empty<string>();
        }
        public FormattedData()
        {
        }
        /// <summary>
        /// 比较两个格式化字符串数组是否相同
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(FormattedData other)
        {
            if (Items.Length != other.Items.Length)
            {
                return false;
            }
            for (int i = 0; i < Items.Length; i++)
            {
                if (string.Equals(Items[i], other.Items[i]) == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
