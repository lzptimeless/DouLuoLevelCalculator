using DouLuoLevelCalculator.Models;
using DouLuoLevelCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DouLuoLevelCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

        private void DataGrid_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            if (e.IsColumnHeadersRow)
            {
                var oldHeaderRow = e.ClipboardRowContent.ToArray();
                e.ClipboardRowContent.Clear();
                foreach (var oldHeader in oldHeaderRow)
                {
                    e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, oldHeader.Column, $"{oldHeader.Content}|"));
                }
                return;
            }

            var levelStatus = (e.Item as LevelStatus)!;
            var oldRow = e.ClipboardRowContent.ToArray();
            e.ClipboardRowContent.Clear();

            foreach (var cell in oldRow)
            {
                if (cell.Column.Header as string == "等级")
                {
                    string s;
                    if (levelStatus.Level < 10) s = $"{levelStatus.Level}级  |";
                    else if (levelStatus.Level < 99.4) s = $"{levelStatus.Level}级 |";
                    else if (levelStatus.Level < 99.8) s = "半神 |";
                    else if (levelStatus.Level < 100) s = "准神 |";
                    else s = $"{levelStatus.Level}级|";

                    e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, cell.Column, s));
                }
                else if (cell.Column.Header as string == "日期")
                {
                    e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, cell.Column, $"{levelStatus.Date:yyyy年MM月}|"));
                }
                else if (cell.Column.Header as string == "年龄")
                {
                    string s;
                    if (levelStatus.Age < 10) s = $"{levelStatus.Age}岁  |";
                    else if (levelStatus.Age < 100) s = $"{levelStatus.Age}岁 |";
                    else s = $"{levelStatus.Age}岁|";

                    e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, cell.Column, s));
                }
                else if (cell.Column.Header as string == "魂环年限")
                {
                    string s;
                    if (levelStatus.SoulCircle < 10) s = $"{levelStatus.SoulCircle}年      |";
                    else if (levelStatus.SoulCircle < 100) s = $"{levelStatus.SoulCircle}年     |";
                    else if (levelStatus.SoulCircle < 1000) s = $"{levelStatus.SoulCircle}年    |";
                    else if (levelStatus.SoulCircle < 10000) s = $"{levelStatus.SoulCircle}年   |";
                    else if (levelStatus.SoulCircle < 100000) s = $"{levelStatus.SoulCircle}年  |";
                    else if (levelStatus.SoulCircle < 1000000) s = $"{levelStatus.SoulCircle}年 |";
                    else s = $"{levelStatus.SoulCircle}年|";

                    e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, cell.Column, s));
                }
                else if (cell.Column.Header as string == "魂环附加等级")
                {
                    string s;
                    if (levelStatus.SoulCircleAddLevel < 10) s = $"{levelStatus.SoulCircleAddLevel}级 |";
                    else s = $"{levelStatus.SoulCircleAddLevel}级|";

                    e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, cell.Column, s));
                }
                else if (cell.Column.Header as string == "额外附加等级")
                {
                    e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, cell.Column, $"{levelStatus.ExLevel}级|"));
                }
                else if (cell.Column.Header as string == "当前修炼速度（级/年）")
                {
                    e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, cell.Column, $"{levelStatus.TrainingSpeed}级/年|"));
                }
            }
        }
    }
}
