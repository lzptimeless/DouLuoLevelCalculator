using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DouLuoLevelCalculator.Models
{
    /// <summary>
    /// 人物配置
    /// </summary>
    public class CharacterConfig
    {
        /// <summary>
        /// 初始日期
        /// </summary>
        public DateTime InitDate { get; set; }

        /// <summary>
        /// 初始年龄
        /// </summary>
        public int InitAge { get; set; }

        /// <summary>
        /// 初始等级
        /// </summary>
        public double InitLevel { get; set; }

        /// <summary>
        /// 初始先天魂力
        /// </summary>
        public double InitCongenitalPower { get; set; }

        /// <summary>
        /// 初始努力系数
        /// </summary>
        public double InitEffort { get; set; }

        /// <summary>
        /// 存在与某些等级的调整
        /// </summary>
        public List<CharacterConfigLevel> Levels { get; set; } = new List<CharacterConfigLevel>();

        public static CharacterConfig? Load(string filePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CharacterConfig));
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return xmlSerializer.Deserialize(fs) as CharacterConfig;
            }
        }

        public void Save(string filePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CharacterConfig));
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                xmlSerializer.Serialize(fs, this);
            }
        }
    }

    public class CharacterConfigLevel
    {
        /// <summary>
        /// 等级
        /// </summary>
        [XmlAttribute]
        public double Level { get; set; }
        /// <summary>
        /// 魂环等级
        /// </summary>
        [XmlAttribute]
        public double SoulCircle { get; set; }
        /// <summary>
        /// 额外附加等级，比如仙草附加的等级
        /// </summary>
        [XmlAttribute]
        public double ExLevel { get; set; }
        /// <summary>
        /// 努力系数变更
        /// </summary>
        [XmlAttribute]
        public double Effort { get; set; }
        /// <summary>
        /// 天赋变更，比如仙草影响
        /// </summary>
        [XmlAttribute]
        public double CongenitalPower { get; set; }
    }
}
