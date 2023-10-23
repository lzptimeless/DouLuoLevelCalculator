using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DouLuoLevelCalculator.Models
{
    /// <summary>
    /// 修炼中的一个阶段
    /// </summary>
    public class LevelStatus : BindableBase
    {
        #region properties
        private DateTime _date;
        /// <summary>
        /// 当前日期
        /// </summary>
        public DateTime Date
        {
            get => _date; set => SetProperty(ref _date, value);
        }

        private int _age;
        /// <summary>
        /// 当前年龄
        /// </summary>
        public int Age
        {
            get => _age; set => SetProperty(ref _age, value);
        }

        private double _level;
        /// <summary>
        /// 当前等级
        /// </summary>
        public double Level
        {
            get => _level; set => SetProperty(ref _level, value);
        }

        private double _exLevel;
        /// <summary>
        /// 额外的等级加成
        /// </summary>
        public double ExLevel
        {
            get => _exLevel; set => SetProperty(ref _exLevel, value);
        }

        public double _soulCircle;
        /// <summary>
        /// 猎取的魂环数值
        /// </summary>
        public double SoulCircle
        {
            get => _soulCircle; set => SetProperty(ref _soulCircle, value);
        }

        public double _soulCircleAddLevel;
        /// <summary>
        /// 魂环附加的等级
        /// </summary>
        public double SoulCircleAddLevel
        {
            get => _soulCircleAddLevel; set => SetProperty(ref _soulCircleAddLevel, value);
        }

        public double _remainSoulCircle;
        /// <summary>
        /// 上次附加魂环提升等级后没有用完的能量
        /// </summary>
        public double RemainSoulCircle
        {
            get => _remainSoulCircle; set => SetProperty(ref _remainSoulCircle, value);
        }

        private double _trainingSpeed;
        /// <summary>
        /// 相对于正常努力修炼的速度的比率
        /// </summary>
        public double TrainingSpeed
        {
            get => _trainingSpeed; set => SetProperty(ref _trainingSpeed, value);
        }
        #endregion
    }
}
