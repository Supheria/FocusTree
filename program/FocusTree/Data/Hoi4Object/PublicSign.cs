namespace FocusTree.Data.Hoi4Object
{
    public static class PublicSign
    {
        /// <summary>
        /// 语义分割符
        /// </summary>
        public static char Splitter = '|';
        /// <summary>
        /// 执行动作
        /// </summary>
        [Flags]
        public enum Motions
        {
            #region ==== 动作时机 ====

            /// <summary>
            /// 国策完成后开始实施
            /// </summary>
            AfterDone = 0b1,
            /// <summary>
            /// 开启国策后立即实施
            /// </summary>
            Instantly = AfterDone << 1,

            #endregion

            #region ==== 基础 ====

            NoneButMayChange = Instantly << 1,
            /// <summary>
            /// 触发
            /// </summary>
            Trigger = NoneButMayChange << 1,
            /// <summary>
            /// 修改
            /// </summary>
            Modify = Trigger << 1,
            /// <summary>
            /// 创建
            /// </summary>
            Create = Modify << 1,
            /// <summary>
            /// 加入
            /// </summary>
            Join = Create << 1,
            /// <summary>
            /// 吞并
            /// </summary>
            Annexed = Join << 1,
            /// <summary>
            /// 开启
            /// </summary>
            Start = Annexed << 1,

            #endregion

            #region ==== 修改 ====

            /// <summary>
            /// 加
            /// </summary>
            Add = Modify | Join << 1,
            /// <summary>
            /// 减
            /// </summary>
            Sub = Modify | (Add ^ Modify) << 1,
            /// <summary>
            /// 固定
            /// </summary>
            Fixed = Modify | (Sub ^ Modify) << 1,
            /// <summary>
            /// 取消固定
            /// </summary>
            Unpin = Modify | (Fixed ^ Modify) << 1,
            /// <summary>
            /// 获得
            /// </summary>
            Gain = Modify | (Unpin ^ Modify) << 1,
            /// <summary>
            /// 移除
            /// </summary>
            Remove = Modify | (Gain ^ Modify) << 1,
            /// <summary>
            /// 加成
            /// </summary>
            Bonus = Modify | (Remove ^ Modify) << 1,
            /// <summary>
            /// 取代
            /// </summary>
            Replace = Modify | (Bonus ^ Modify) << 1

            #endregion
        };
        /// <summary>
        /// 受施对象类型
        /// </summary>
        [Flags]
        public enum Types
        {
            #region ==== 基础 ====

            /// <summary>
            /// 事件
            /// </summary>
            Event = 0b1,
            /// <summary>
            /// 可同意的事件
            /// </summary>
            RequestEvent = Event << 1,
            /// <summary>
            /// 标签
            /// </summary>
            Label = RequestEvent << 1,
            /// <summary>
            /// 变量
            /// </summary>
            Variable = Label << 1,
            /// <summary>
            /// 可用性
            /// </summary>
            Availability = Variable << 1,
            /// <summary>
            /// 战争目标
            /// </summary>
            WarGoal = Availability << 1,
            /// <summary>
            /// 决议
            /// </summary>
            Resolution = WarGoal << 1,
            /// <summary>
            /// 阵营
            /// </summary>
            Camp = Resolution << 1,
            /// <summary>
            /// 研究
            /// </summary>
            Reaserch = Camp << 1,
            /// <summary>
            /// 等级
            /// </summary>
            Grade = Reaserch << 1,
            /// <summary>
            /// 部队
            /// </summary>
            Troop = Grade << 1,
            /// <summary>
            /// 国家
            /// </summary>
            State = Troop << 1,
            /// <summary>
            /// 省份
            /// </summary>
            Province = State << 1,
            /// <summary>
            /// 区域
            /// </summary>
            Region = Province << 1,
            /// <summary>
            /// Ai修正
            /// </summary>
            AiModifyer = Region << 1,
            /// <summary>
            /// 区域核心
            /// </summary>
            RegionCore = AiModifyer << 1,

            #endregion

            #region ==== 可用性 ====

            /// <summary>
            /// 可以宣战
            /// </summary>
            AbleToDeclareWar = Availability | RegionCore << 1,
            /// <summary>
            /// 可以自动获取核心
            /// </summary>
            AbleToGainCoreAuto = Availability | (AbleToDeclareWar ^ Availability) << 1,
            /// <summary>
            /// 可以创建阵营
            /// </summary>
            AbleToCreateCamp = Availability | (AbleToGainCoreAuto ^ Availability) << 1,
            /// <summary>
            /// 可以加入阵营
            /// </summary>
            AbleToJoinCamp = Availability | (AbleToCreateCamp ^ Availability) << 1,
            /// <summary>
            /// 可以开启决议
            /// </summary>
            AbleToStartResolution = Availability | (AbleToJoinCamp ^ Availability) << 1,

            #endregion
        }

        /// <summary>
        /// 通过枚举名获取对应枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="name">枚举名</param>
        /// <returns>枚举名存在则以 object 返回枚举对象，否则返回null</returns>
        public static object GetEnumValue<T>(string name)
        {
            try
            {
                return Enum.Parse(typeof(T), name);
            }
            catch
            {
                return null;
            }
        }
    }
}
