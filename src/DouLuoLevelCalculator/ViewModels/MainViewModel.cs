using DouLuoLevelCalculator.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Ribbon;

namespace DouLuoLevelCalculator.ViewModels
{
    public class MainViewModel : BindableBase
    {
        #region properties
        private int _initAge = 6;
        /// <summary>
        /// 初始年龄，默认为6
        /// </summary>
        public int InitAge
        {
            get => _initAge; set => SetProperty(ref _initAge, value);
        }

        private double _initLevel = 10;
        /// <summary>
        /// 初始等级，默认为10
        /// </summary>
        public double InitLevel
        {
            get => _initLevel; set => SetProperty(ref _initLevel, value);
        }

        private DateTime _initDate = new DateTime(2637, 9, 1);
        /// <summary>
        /// 初始时间，默认为2637/9唐三6岁的哪一年
        /// </summary>
        public DateTime InitDate
        {
            get => _initDate; set => SetProperty(ref _initDate, value);
        }

        private double _naturalSp = 10;
        /// <summary>
        /// 先天魂力，默认为10
        /// </summary>
        public double NaturalSp
        {
            get => _naturalSp; set => SetProperty(ref _naturalSp, value);
        }

        private double _effort = 1;
        /// <summary>
        /// 相对于正常努力修炼的速度的比率，有些懈怠0.7，中规中矩0.8，努力0.9，非常努力1，默认1
        /// </summary>
        public double Effort
        {
            get => _effort; set => SetProperty(ref _effort, value);
        }

        /// <summary>
        /// 等级提升步骤
        /// </summary>
        public ObservableCollection<LevelStatus> LevelStatuses { get; set; } = new ObservableCollection<LevelStatus>();

        private string? _characterName;
        /// <summary>
        /// 人物名字
        /// </summary>
        public string? CharacterName
        {
            get => _characterName; set => SetProperty(ref _characterName, value);
        }
        #endregion

        #region commands
        private DelegateCommand? _computeCommand;
        /// <summary>
        /// 刷新等级提升步骤
        /// </summary>
        public DelegateCommand ComputeCommand
        {
            get
            {
                _computeCommand ??= new DelegateCommand(Compute);

                return _computeCommand;
            }
        }

        private DelegateCommand? _resetCommand;
        /// <summary>
        /// 重置数据
        /// </summary>
        public DelegateCommand ResetCommand
        {
            get
            {
                _resetCommand ??= new DelegateCommand(Reset);
                return _resetCommand;
            }
        }

        private DelegateCommand? _importCharacterConfigCommand;
        /// <summary>
        /// 导入人物配置
        /// </summary>
        public DelegateCommand ImportCharacterConfigCommand
        {
            get
            {
                _importCharacterConfigCommand ??= new DelegateCommand(Import);
                return _importCharacterConfigCommand;
            }
        }

        private DelegateCommand? _exportCharacterConfigCommand;
        /// <summary>
        /// 导出人物配置
        /// </summary>
        public DelegateCommand ExportCharacterConfigCommand
        {
            get
            {
                _exportCharacterConfigCommand ??= new DelegateCommand(Export);
                return _exportCharacterConfigCommand;
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// 刷新等级提升步骤
        /// </summary>
        /// <param name="initAge">初始年龄</param>
        /// <param name="initLevel">初始等级</param>
        /// <param name="initDate">初始日期</param>
        /// <param name="naturalSp">先天魂力</param>
        /// <param name="effort">相对于正常努力修炼速度的比率</param>
        public void Compute(int initAge, double initLevel, DateTime initDate, double naturalSp, double effort)
        {
            List<LevelStatus> oldStatuses = new List<LevelStatus>(LevelStatuses);
            LevelStatuses.Clear();

            DateTime currentDate = initDate;
            double currentLevel = initLevel;
            double currentEffort = GetEffort(oldStatuses, currentLevel) != 0 ? GetEffort(oldStatuses, currentLevel) : effort;
            double currentCongenitalPower = GetCongenitalPower(oldStatuses, currentLevel) != 0 ? GetCongenitalPower(oldStatuses, currentLevel) : naturalSp;
            double soulCircle = GetSoulCircle(oldStatuses, currentLevel);
            var addResult = GetSoulCircleAddLevel(soulCircle, currentLevel);
            double remainSoulCircle = addResult.RemainSoulCircleValue;
            string? comments = GetComments(oldStatuses, currentLevel);

            LevelStatuses.Add(new LevelStatus
            {
                Date = currentDate,
                Age = initAge,
                Level = currentLevel,
                Effort = GetEffortSetting(oldStatuses, currentLevel),
                CalculateEffort = currentEffort,
                CongenitalPower = GetCongenitalSetting(oldStatuses, currentLevel),
                CalculateCongenitalPower = currentCongenitalPower,
                TrainingSpeed = Math.Round(GetTrainingSpeedFromInitSoulPower(currentCongenitalPower, currentLevel, currentEffort), 4),
                ExLevel = GetExLevel(oldStatuses, currentLevel),
                SoulCircle = soulCircle,
                SoulCircleAddLevel = addResult.IncrementLevel,
                RemainSoulCircle = remainSoulCircle,
                Comments = comments
            });

            while (currentLevel < 100)
            {
                var lastLevelStatus = LevelStatuses.Last()!;
                currentEffort = GetEffort(oldStatuses, currentLevel) != 0 ? GetEffort(oldStatuses, currentLevel) : effort;
                currentCongenitalPower = GetCongenitalPower(oldStatuses, currentLevel) != 0 ? GetCongenitalPower(oldStatuses, currentLevel) : naturalSp;
                double realLevel = currentLevel + lastLevelStatus.SoulCircleAddLevel + lastLevelStatus.ExLevel;
                double nextLevel;
                DateTime nextDate;
                if (realLevel >= (Math.Floor(lastLevelStatus.Level / 10) + 1) * 10)
                {
                    // 吸收魂环提升的等级和附加等级超过了当前的大阶，这种情况下不用修炼直接吸收魂环
                    nextLevel = realLevel;
                    nextDate = currentDate;
                }
                else
                {
                    double incrementPerDay = GetTrainingSpeedFromInitSoulPower(currentCongenitalPower, realLevel, currentEffort) / 365;
                    if (incrementPerDay <= 0)
                    {
                        break;
                    }

                    // 计算将要提升到的下一个等级
                    if (realLevel < 99) nextLevel = realLevel + 1;
                    else if (realLevel < 99.8) nextLevel = Math.Min(100, realLevel + 0.4);
                    else if (realLevel < 100) nextLevel = 100;
                    else nextLevel = realLevel + 10;

                    // 计算提升到下一个等级需要的修炼时间
                    int toNextLevelDays = (int)Math.Round(Math.Max(0, nextLevel - realLevel) / incrementPerDay);
                    nextDate = currentDate.AddDays(toNextLevelDays);
                }

                double nextSoulCircle = GetSoulCircle(oldStatuses, nextLevel);
                var tmpAddResult = GetSoulCircleAddLevel(nextSoulCircle + lastLevelStatus.RemainSoulCircle, nextLevel);
                double nextExLevel = GetExLevel(oldStatuses, nextLevel);
                currentEffort = GetEffort(oldStatuses, nextLevel) != 0 ? GetEffort(oldStatuses, nextLevel) : effort;
                currentCongenitalPower = GetCongenitalPower(oldStatuses, nextLevel) != 0 ? GetCongenitalPower(oldStatuses, nextLevel) : naturalSp;
                double nextSpeed = GetTrainingSpeedFromInitSoulPower(currentCongenitalPower, nextLevel + tmpAddResult.IncrementLevel + nextExLevel, currentEffort);
                int nextAge = initAge + nextDate.Year - initDate.Year;
                var nextLevelStatus = new LevelStatus
                {
                    Date = nextDate,
                    Age = nextAge,
                    Level = Math.Round(nextLevel, 1),
                    Effort = GetEffortSetting(oldStatuses, nextLevel),
                    CalculateEffort = currentEffort,
                    CongenitalPower = GetCongenitalSetting(oldStatuses, nextLevel),
                    CalculateCongenitalPower = currentCongenitalPower,
                    TrainingSpeed = Math.Round(nextSpeed, 4),
                    ExLevel = nextExLevel,
                    SoulCircle = nextSoulCircle,
                    SoulCircleAddLevel = tmpAddResult.IncrementLevel,
                    RemainSoulCircle = tmpAddResult.RemainSoulCircleValue,
                    Comments = GetComments(oldStatuses, nextLevel)
                };
                LevelStatuses.Add(nextLevelStatus);
                currentDate = nextDate;
                currentLevel = nextLevel;
            }
        }

        /// <summary>
        /// 刷新等级提升步骤
        /// </summary>
        public void Compute()
        {
            Compute(InitAge, InitLevel, InitDate, NaturalSp, Effort);
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        public void Reset()
        {
            CharacterName = null;

            InitAge = 6;
            InitLevel = 10;
            InitDate = new DateTime(2631, 9, 1);
            NaturalSp = 10;
            Effort = 1;

            LevelStatuses.Clear();
        }

        /// <summary>
        /// 导入人物配置
        /// </summary>
        public void Import()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Characters");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Characters");
            dialog.Filter = "人物配置|*.xml";
            if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.FileName))
            {
                try
                {
                    var config = CharacterConfig.Load(dialog.FileName);
                    if (config == null) throw new ApplicationException("配置文件为空！");

                    Reset();

                    CharacterName = Path.GetFileNameWithoutExtension(dialog.FileName);
                    InitDate = config.InitDate;
                    InitAge = config.InitAge;
                    InitLevel = config.InitLevel;
                    NaturalSp = config.InitCongenitalPower;
                    Effort = config.InitEffort;

                    // 创建LevelStatuses
                    Compute();

                    foreach (var level in config.Levels)
                    {
                        var currentLevel = LevelStatuses.FirstOrDefault(x => x.Level == level.Level);
                        if (currentLevel != null)
                        {
                            currentLevel.SoulCircle = level.SoulCircle;
                            currentLevel.ExLevel = level.ExLevel;
                            currentLevel.Effort = level.Effort;
                            currentLevel.CongenitalPower = level.CongenitalPower;
                            currentLevel.Comments = level.Comments;
                        }
                    }

                    // 重新计算
                    Compute();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 导出人物配置
        /// </summary>
        public void Export()
        {
            var dialog = new SaveFileDialog();
            dialog.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Characters");
            dialog.Filter = "人物配置|*.xml";
            if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.FileName))
            {
                try
                {
                    CharacterName = Path.GetFileNameWithoutExtension(dialog.FileName);
                    var config = new CharacterConfig();
                    config.InitDate = InitDate;
                    config.InitAge = InitAge;
                    config.InitLevel = InitLevel;
                    config.InitCongenitalPower = NaturalSp;
                    config.InitEffort = Effort;

                    foreach (var level in LevelStatuses)
                    {
                        if (level.SoulCircle != 0 || level.ExLevel != 0 || level.Effort != 0 || level.CongenitalPower != 0 || !string.IsNullOrEmpty(level.Comments))
                        {
                            config.Levels.Add(new CharacterConfigLevel
                            {
                                Level = level.Level,
                                SoulCircle = level.SoulCircle,
                                ExLevel = level.ExLevel,
                                Effort = level.Effort,
                                CongenitalPower = level.CongenitalPower,
                                Comments = level.Comments
                            });
                        }
                    }

                    config.Save(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// 获取魂环数值，有旧的配置就用旧的，没有配置旧使用默认魂环数值
        /// </summary>
        /// <param name="items">旧的等级提升步骤</param>
        /// <param name="level">当前等级</param>
        /// <returns></returns>
        private static double GetSoulCircle(IEnumerable<LevelStatus> items, double level)
        {
            if (((int)level) % 10 != 0)
            {
                // 只有10的倍数时才有魂环
                return 0;
            }

            var item = items.FirstOrDefault(x => x.Level == level);
            if (item != null)
            {
                return item.SoulCircle;
            }
            else
            {
                // 正常吸收的魂环等级
                switch (level)
                {
                    case 10: return 400;// 大部分魂师第一魂环可以吸收四百二十三年以下
                    case 20: return 700;// 第二魂环可以吸收七百六十四年以
                    case 30: return 1500;// 大部分魂师第三魂环可以吸收一千七百六十年以下
                    case 40: return 4500;// 第四魂环可以吸收五千年以下，唐三第四环吸收万年魂环提升了一级
                    case 50: return 10000;// 大部分魂师第五魂环可以吸收一万两千年以下
                    case 60: return 20000;// 第六魂环可以吸收两万年以下
                    case 70: return 45000;// 第七魂环可以吸收五万年以下
                    case 80: return 60000;// 第八魂环可以吸收大部分万年魂环
                    case 90: return 80000;
                    default: return 0;
                }
            }
        }

        /// <summary>
        /// 获取魂环对魂师的附加等级
        /// </summary>
        /// <param name="soulCircle">魂环数值</param>
        /// <param name="level">当前等级</param>
        /// <returns></returns>
        private static (double IncrementLevel, double RemainSoulCircleValue) GetSoulCircleAddLevel(double soulCircle, double level)
        {
            double valuePerLevel;
            if (level < 20)
            {
                // 大部分魂师第一魂环可以吸收四百二十三年以下，推测此时魂力等级提升=魂环数值/300
                valuePerLevel = 300;
            }
            else if (level < 30)
            {
                // 第二魂环可以吸收七百六十四年以，推测此时魂力等级提升=魂环数值/500
                valuePerLevel = 500;
            }
            else if (level < 40)
            {
                // 大部分魂师第三魂环可以吸收一千七百六十年以下，推测此时魂力等级提升=魂环数值/1250
                valuePerLevel = 1250;
            }
            else if (level < 50)
            {
                // 第四魂环可以吸收五千年以下，唐三第四环吸收万年魂环提升了一级，推测此时魂力等级提升=魂环数值/4000
                valuePerLevel = 4000;
            }
            else if (level < 60)
            {
                // 大部分魂师第五魂环可以吸收一万两千年以下，推测此时魂力等级提升=魂环数值/10000
                valuePerLevel = 10000;
            }
            else if (level < 70)
            {
                // 第六魂环可以吸收两万年以下，推测此时魂力等级提升=魂环数值/15000
                valuePerLevel = 15000;
            }
            else if (level < 80)
            {
                // 第七魂环可以吸收五万年以下，推测此时魂力等级提升=魂环数值/30000
                valuePerLevel = 30000;
            }
            else if (level < 90)
            {
                // 第八魂环可以吸收大部分万年魂环，推测此时魂力等级提升=魂环数值/40000
                valuePerLevel = 40000;
            }
            else if (level < 95)
            {
                // 唐三吸收六万年暗魔邪神虎从93级升到94级多，得出5万年魂环可以使人在95级之前提升一级。
                valuePerLevel = 50000;
            }
            else if (level < 99)
            {
                // 双生武魂者可以凭借第二魂环附加万年魂环快速提升等级到99级，所以95到99级每吸收8万年魂环应该能够提升一级
                valuePerLevel = 80000;
            }
            else
            {
                // 合理推测99级之后每吸收10万年魂环提升0.1级
                valuePerLevel = 1000000;
            }

            double maxIncrementLevel = (Math.Floor(level / 10) + 1) * 10 - level;
            double incrementLevel = Math.Floor(Math.Min(maxIncrementLevel, soulCircle / valuePerLevel));
            double remainSoulCircleValue = soulCircle - incrementLevel * valuePerLevel;
            return (incrementLevel, remainSoulCircleValue);
        }

        /// <summary>
        /// 获得保存在items中的额外的等级加成，通常用以表示超限魂环附加等级
        /// </summary>
        /// <param name="items">旧的等级提升步骤</param>
        /// <param name="level">当前等级</param>
        /// <returns></returns>
        private static double GetExLevel(IEnumerable<LevelStatus> items, double level)
        {
            var item = items.FirstOrDefault(x => x.Level == level);
            if (item == null) return 0;
            else return item.ExLevel;
        }

        /// <summary>
        /// 获取保存在items中离指定等级最近的努力系数
        /// </summary>
        /// <param name="items">旧的等级提升步骤</param>
        /// <param name="level">当前等级</param>
        /// <returns></returns>
        private static double GetEffort(IEnumerable<LevelStatus> items, double level)
        {
            LevelStatus? lastItem = null;
            foreach (var item in items)
            {
                if (item.Level > level) break;
                if (item.Effort != 0) lastItem = item;
            }

            return lastItem != null ? lastItem.Effort : 0;
        }

        /// <summary>
        /// 获取指定等级的备注
        /// </summary>
        /// <param name="items">旧的等级提升步骤</param>
        /// <param name="level">当前等级</param>
        /// <returns></returns>
        private static string? GetComments(IEnumerable<LevelStatus> items, double level)
        {
            var item = items.FirstOrDefault(x => x.Level == level);
            if (item == null) return null;
            else return item.Comments;
        }

        /// <summary>
        /// 获取指定等级设置的努力系数
        /// </summary>
        /// <param name="items">旧的等级提升步骤</param>
        /// <param name="level">当前等级</param>
        /// <returns></returns>
        private static double GetEffortSetting(IEnumerable<LevelStatus> items, double level)
        {
            var item = items.FirstOrDefault(x => x.Level == level);
            if (item == null) return 0;
            else return item.Effort;
        }

        /// <summary>
        /// 获取保存在Items中离指定等级最近的先天魂力
        /// </summary>
        /// <param name="items">旧的等级提升步骤</param>
        /// <param name="level">当前等级</param>
        /// <returns></returns>
        private static double GetCongenitalPower(IEnumerable<LevelStatus> items, double level)
        {
            LevelStatus? lastItem = null;
            foreach (var item in items)
            {
                if (item.Level > level) break;
                if (item.CongenitalPower != 0) lastItem = item;
            }

            return lastItem != null ? lastItem.CongenitalPower : 0;
        }

        /// <summary>
        /// 获取指定等级设置的先天魂力
        /// </summary>
        /// <param name="items">旧的等级提升步骤</param>
        /// <param name="level">当前等级</param>
        /// <returns></returns>
        private static double GetCongenitalSetting(IEnumerable<LevelStatus> items, double level)
        {
            var item = items.FirstOrDefault(x => x.Level == level);
            if (item == null) return 0;
            else return item.CongenitalPower;
        }

        /// <summary>
        /// 根据先天魂力和当前等级获得当前修炼速度（级/年）
        /// </summary>
        /// <param name="naturalSp">先天魂力</param>
        /// <param name="level">当前等级</param>
        /// <param name="speedRate">相对于正常努力修炼速度的比率</param>
        /// <returns></returns>
        private double GetTrainingSpeedFromInitSoulPower(double naturalSp, double level, double speedRate)
        {
            if (naturalSp <= 0) return 0;

            // 计算不考虑潜力限制的极限修炼速度
            double speed;
            if (level < 30) speed = GetFullSpeed(naturalSp); // 30级之前修炼速度最快
            else if (level < 70) speed = GetFullSpeed(naturalSp) * 0.8f; // 30级之后修炼速度有所减低，大概为之前的0.8
            else if (level < 90) speed = GetFullSpeed(naturalSp) * 0.8f * 0.5f; // 70级之后修炼速度减半
            else if (level < 95) speed = GetFullSpeed(naturalSp) * 0.8f * 0.5f * 0.5f; // 90级之后修炼速度再减半
            else if (level < 99) speed = GetFullSpeed(naturalSp) * 0.8f * 0.5f * 0.5f * 0.1f; // 95级之后修炼速度为之前的十分之一
            else speed = GetFullSpeed(naturalSp) * 0.8f * 0.5f * 0.5f * 0.1f * 0.2f; // 99级到100级速度为之前的五分之一

            // 再加上潜力消耗的影响
            speed = speed * GetRemainPotentialRate(naturalSp, level);

            // 再加上个人努力系数
            speed = speed * speedRate;

            return speed;
        }

        /// <summary>
        /// 获取剩余潜力对修炼速度的影响系数
        /// </summary>
        /// <param name="naturalSp">先天魂力</param>
        /// <param name="level">当前等级</param>
        /// <returns></returns>
        private static double GetRemainPotentialRate(double naturalSp, double level)
        {
            // 根据小说潜力大概等于先天魂力x10
            double potentialValue = naturalSp * 10;
            if (potentialValue - 10 > level)
            {
                // 潜力剩余大于10级时修炼速度不受影响
                return 1;
            }
            else
            {
                // 潜力剩余小于10级时修炼速度逐渐变慢
                return Math.Max(0, (potentialValue + 5 - level) / 15);
            }
        }

        /// <summary>
        /// 根据先天魂力获得初始修炼速度（级/年），这个速度是先天魂力对应的极限修炼速度
        /// </summary>
        /// <param name="naturalSp">先天魂力</param>
        /// <returns></returns>
        private static double GetFullSpeed(double naturalSp)
        {
            if (naturalSp <= 1) return 0.25;
            else if (naturalSp <= 2) return 0.5;
            else if (naturalSp <= 3) return 1;
            else if (naturalSp <= 4) return 1.5;
            else if (naturalSp <= 5) return 2;
            else if (naturalSp <= 6) return 2.5;
            else if (naturalSp <= 7) return 3; // 朱竹青初次登场12岁27级，初始等级7，魂环增加2，修炼速度为3，以朱竹清的努力程度来看应该算极限了
            else if (naturalSp <= 8) return 3.5;
            else if (naturalSp <= 9) return 4;
            else return 4.5; // 唐三30级之前修炼速度慢是因为他需要上课、打铁、陪小舞
        }
        #endregion
    }
}
