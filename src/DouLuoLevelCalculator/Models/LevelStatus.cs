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
        /// 修炼的速度
        /// </summary>
        public double TrainingSpeed
        {
            get => _trainingSpeed; set => SetProperty(ref _trainingSpeed, value);
        }

        private double _effort;
        /// <summary>
        /// 努力程度系数，懈怠0.7，中规中矩0.8，努力0.9，非常努力1
        /// </summary>
        public double Effort
        {
            get => _effort; set => SetProperty(ref _effort, value);
        }

        private double _calculateEffort;
        /// <summary>
        /// 用以最终计算修炼时间的努力系数
        /// </summary>
        public double CalculateEffort
        {
            get => _calculateEffort; set => SetProperty(ref _calculateEffort, value);
        }

        private double _congenitalPower;
        /// <summary>
        /// 先天魂力
        /// </summary>
        public double CongenitalPower
        {
            get => _congenitalPower; set => SetProperty(ref _congenitalPower, value);
        }

        private double _calculateCongenitalPower;
        /// <summary>
        /// 用以最终计算的先天魂力
        /// </summary>
        public double CalculateCongenitalPower
        {
            get => _calculateCongenitalPower; set => SetProperty(ref _calculateCongenitalPower, value);
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Comments { get; set; }
        #endregion
    }
}
