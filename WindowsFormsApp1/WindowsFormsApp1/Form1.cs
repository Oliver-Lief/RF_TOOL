using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

        }

        private const string DEFAULT_EPSILON_VALUE_1 = "2.2";
        private const string DEFAULT_MICROSTRIP_HEIGHT_VALUE_1 = "0.035";
        private const string DEFAULT_SUB_HEIGHT_VALUE_1 = "1.575";
        private const string DEFAULT_PIC_NUM_VALUE_1 = "1";
        private const string DEFAULT_HEAGER_DATA_NUM_VALUE_3 = "9";

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox_epsilon_1.Text = DEFAULT_EPSILON_VALUE_1;//设置默认值
            textBox_Height_1.Text = DEFAULT_MICROSTRIP_HEIGHT_VALUE_1;
            textBox_sub_Height_1.Text = DEFAULT_SUB_HEIGHT_VALUE_1;
            pictureBox_1.Image = WindowsFormsApp1.Properties.Resources.res1;
            textBox_pic_num_2.Text = DEFAULT_PIC_NUM_VALUE_1;
            textBox_header_start_3.Text = DEFAULT_HEAGER_DATA_NUM_VALUE_3;
            //第二页为主页
            tabControl1.SelectedIndex = 1;
        }



        // 微带线计算按键
        private void MicroStrip_Cal_Click(object sender, EventArgs e)
        {
            // 若输入为空，则弹出提示框
            if (textBox_epsilon_1.Text == "" || textBox_Width_1.Text == "" || textBox_Height_1.Text == "" || textBox_sub_Height_1.Text == "")
                MessageBox.Show("输入不能为空");
            // 不为空，则获取四个输入框的值
            else
            {
                double epsilon_1 = Convert.ToDouble(textBox_epsilon_1.Text.Trim());
                double Width_1 = Convert.ToDouble(textBox_Width_1.Text.Trim());
                double Height_1 = Convert.ToDouble(textBox_Height_1.Text.Trim());
                double sub_Height_1 = Convert.ToDouble(textBox_sub_Height_1.Text.Trim());
                double co_epsilon_1;
                double Z_1;
                //MessageBox.Show("提取成功:" + epsilon_1 + "," + Width_1 + "," + Height_1 + "," + sub_Height_1);
                if (Width_1 < sub_Height_1)
                {
                    co_epsilon_1 = (epsilon_1 + 1) / 2 + (epsilon_1 - 1) / 2 * (1 / Math.Sqrt(1 + 12 * sub_Height_1 / Width_1) + 0.04 * Math.Pow((1 - Width_1 / sub_Height_1), 2));
                    Z_1 = 376.8 / (2 * Math.PI * Math.Sqrt(co_epsilon_1)) * Math.Log(8 * sub_Height_1 / Width_1 + Width_1 / 4 / sub_Height_1);
                }
                else
                {
                    co_epsilon_1 = (epsilon_1 + 1) / 2 + (epsilon_1 - 1) / 2 * (1 / Math.Sqrt(1 + 12 * sub_Height_1 / Width_1));
                    Z_1 = 376.8 / Math.Sqrt(co_epsilon_1) / (1.393 + Width_1 / sub_Height_1 + 2 / 3 * Math.Log(Width_1 / sub_Height_1 + 1.444));
                }
                textBox_co_epsilon_1.Text = co_epsilon_1.ToString("f4");
                textBox_Z_1.Text = Z_1.ToString("f4");
            }


        }

        // 介电常数只允许输入数字
        private void textBox_epsilon_1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 退格键、数字、小数点可输入
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar) && e.KeyChar != 46)
            {
                e.Handled = true;
            }
        }
        // 微带线宽度只允许输入数字
        private void textBox_Width_1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar) && e.KeyChar != 46)
            {
                e.Handled = true;
            }
        }
        // 微带线高度只允许输入数字
        private void textBox_Height_1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar) && e.KeyChar != 46)
            {
                e.Handled = true;
            }
        }
        // 介质基板只允许输入数字
        private void textBox_sub_Height_1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar) && e.KeyChar != 46)
            {
                e.Handled = true;
            }
        }
        // 曲线条数只允许输入数字
        private void textBox_pic_num_2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        // 提取按钮
        private void button_extract_2_Click(object sender, EventArgs e)
        {
            // 清空表格
            dataGridView_show_2.Rows.Clear();
            // 清空所有列
            for (int i = dataGridView_show_2.ColumnCount - 1; i >= 0; i--)
            {
                dataGridView_show_2.Columns.RemoveAt(i);
            }
            // 获取几条曲线
            int pic_num_2 = Convert.ToInt16(textBox_pic_num_2.Text.Trim());
            // 在窗体加载时添加表格列
            dataGridView_show_2.Columns.Add("Column1", "Column 1");
            dataGridView_show_2.Columns.Add("Column2", "Column 2");



            if (textBox_pic_num_2.Text == "" || textBox_filepath_2.Text == "")
                MessageBox.Show("输入不能为空");
            else
            {
                string path = textBox_filepath_2.Text;
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        // 如果是空行或包含字母或是短划线的行，则跳过
                        if (string.IsNullOrWhiteSpace(line) || ContainsLetter(line) || line.Trim() == "-")
                        {
                            continue;
                        }
                        // 否则打印该行内容
                        // 使用正则表达式匹配所有的数字，包括正数、负数和小数
                        MatchCollection matches = Regex.Matches(line, @"-?\d+(\.\d+)?");
                        if (matches.Count >= 2)
                        {
                            for (int i = 0; i < matches.Count; i += 2)
                            {
                                string number1 = matches[i].Value;
                                string number2 = i + 1 < matches.Count ? matches[i + 1].Value : "";

                                dataGridView_show_2.Rows.Add(number1, number2);
                            }

                        }

                    }
                }
            }
            // 禁止排序
            for (int i = 0; i < this.dataGridView_show_2.Columns.Count; i++)
            {
                this.dataGridView_show_2.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            // 行号宽度
            dataGridView_show_2.RowHeadersWidth = 100;

            // 表格行数
            int rowCount = dataGridView_show_2.Rows.Count;
            //MessageBox.Show(rowCount.ToString());

            if (pic_num_2 >= 2)
                table_change(rowCount, pic_num_2);


            chart_plot(pic_num_2);

        }
        // 表格变换
        private void table_change(int n, int num_p)
        {

            for (int i = 0; i < num_p * 2 - 2; i++)
            {
                // 输入每一列的标题
                string newColumnHeader = "Column " + (i + 3);

                // 创建新列并添加到 DataGridView 中
                DataGridViewTextBoxColumn newColumn = new DataGridViewTextBoxColumn();
                newColumn.HeaderText = newColumnHeader;
                dataGridView_show_2.Columns.Add(newColumn);
            }

            // 将后半部分数据移动
            for (int i = 0; i < num_p - 1; i++)
            {
                // 获取后半部分数据的行索引
                for (int j = n * (i + 1) / num_p; j <= n * (i + 2) / num_p - 1; j++)
                {
                    // 获取后半部分数据的值
                    var value1 = dataGridView_show_2.Rows[j].Cells[0].Value;
                    var value2 = dataGridView_show_2.Rows[j].Cells[1].Value;
                    // 移动
                    dataGridView_show_2.Rows[j - n * (i + 1) / num_p].Cells[i * 2 + 2].Value = value1;
                    dataGridView_show_2.Rows[j - n * (i + 1) / num_p].Cells[i * 2 + 3].Value = value2;
                }
            }

            // 清空后半部分数据
            for (int i = dataGridView_show_2.Rows.Count - 1; i >= n / num_p; i--)
            {
                dataGridView_show_2.Rows.RemoveAt(i);
            }
        }
        // 曲线绘制
        private void chart_plot(int num)
        {
            // 清空图表中的数据
            chart_showSpar_2.Series.Clear();


            //添加数据系列到图表中
            List<string> seriesName = new List<string>();
            for (int col = 0; col < dataGridView_show_2.Columns.Count / 2; col++)
            {
                seriesName.Add("Series" + (col + 1)); // 生成数据系列的名称
                chart_showSpar_2.Series.Add(seriesName[col]); // 添加数据系列
            }

            // 将表格中的数据添加到图表的数据系列中
            for (int row = 0; row < dataGridView_show_2.Rows.Count; row++)
            {
                // 获取每一行的数据
                DataGridViewRow dataRow = dataGridView_show_2.Rows[row];

                // 将每一列的数据添加到对应的数据系列中
                for (int col = 0; col < dataGridView_show_2.Columns.Count / 2; col++)
                {
                    //string seriesName = "Series" + (col + 1); // 获取数据系列的名称
                    //chart_showSpar_2.Series.Add(seriesName); // 添加数据系列
                    chart_showSpar_2.Series[col].Points.AddXY(dataRow.Cells[col * 2].Value, dataRow.Cells[col * 2 + 1].Value);
                }
            }

            // 设置图表的类型为线图
            foreach (var series in chart_showSpar_2.Series)
            {
                series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            }

            // 设置图表的坐标轴标题
            chart_showSpar_2.ChartAreas[0].AxisX.Title = "f";
            chart_showSpar_2.ChartAreas[0].AxisY.Title = "Y";



        }

        // 判断是否存在字符
        private bool ContainsLetter(string s)
        {
            foreach (char c in s)
            {
                if (char.IsLetter(c))
                {
                    return true;
                }
            }
            return false;
        }

        private void textBox_filepath_2_DragDrop(object sender, DragEventArgs e)
        {
            textBox_filepath_2.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }

        private void textBox_filepath_2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        // 选择文件
        private void button_openfile_2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // 设置打开文件对话框的标题
            openFileDialog1.Title = "选择要打开的文件";

            // 设置打开文件对话框的初始目录
            // 桌面绝对路径
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog1.InitialDirectory = desktopPath;

            // 设置打开文件对话框的文件类型
            openFileDialog1.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";

            // 如果用户选择了文件并点击了“确定”按钮
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // 将选中的文件路径显示在TextBox中
                textBox_filepath_2.Text = openFileDialog1.FileName;
            }
        }
        // 点击复制按钮
        private void button_copy_2_Click(object sender, EventArgs e)
        {
            int pic_num_2 = Convert.ToInt16(textBox_pic_num_2.Text.Trim());
            dataGridView_show_2.ClearSelection();
            dataGridView_show_2.SelectAll();
            // 将选中单元格的内容复制到剪贴板
            DataObject dataObject = dataGridView_show_2.GetClipboardContent();

            Clipboard.SetDataObject(dataObject);
            MessageBox.Show("数据已复制", "复制成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // 显示行号
        private void dataGridView_show_2_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }
        // 关于跳转
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //点击第一个按钮时跳转到二个
            //this.Hide();
            about frm2 = new about();
            frm2.Show();
        }

        private void textBox_filepath_3_DragDrop(object sender, DragEventArgs e)
        {
            textBox_filepath_3.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }

        private void textBox_filepath_3_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void button_show_3_Click(object sender, EventArgs e)
        {
            int header_start_3 = Convert.ToInt16(textBox_header_start_3.Text.Trim());
            // 清空图表中的数据
            chart_polar_3.Series.Clear();

            string path = textBox_filepath_3.Text;

            // 路径是文件夹
            if (Directory.Exists(path))
            {
                // 清空表格
                dataGridView_show_path_3.Rows.Clear();
                // 清空所有列
                for (int i = dataGridView_show_path_3.ColumnCount - 1; i >= 0; i--)
                {
                    dataGridView_show_path_3.Columns.RemoveAt(i);
                }
                // 文件夹
                List<string> datFilePaths = GetDatFiles(path);

                // 输出所有 .dat 文件的路径
                List<string> dat_file_path = new List<string>();
                foreach (string filePath in datFilePaths)
                {
                    dat_file_path.Add(filePath);
                }
                dataGridView_show_path_3.Columns.Add("Column1", "path");
                dataGridView_show_path_3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView_show_path_3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView_show_path_3.MultiSelect = false; // 确保只能选中一行
                // 行号宽度
                dataGridView_show_path_3.RowHeadersWidth = 100;
                for (int i = 0; i < dat_file_path.Count; i++)
                {
                    dataGridView_show_path_3.Rows.Add(dat_file_path[i]);
                }

            }
            // 路径是文件
            else
            {
                //MessageBox.Show("是文件");
                // 文件
                dataGridView_show_path_3.Rows.Clear();
                // 读取dat文件
                string[] lines = File.ReadAllLines(path);

                // 使用正则表达式匹配每行数据
                string pattern = @"-?\d+(\.\d+)?";

                // 处理数据
                List<double> angles = new List<double>();
                List<double> radii = new List<double>();
                //MessageBox.Show(lines[0]);
                foreach (string line in lines)
                {
                    //MessageBox.Show(line);
                    MatchCollection matches = Regex.Matches(line, pattern);

                    if (matches.Count >= 2)
                    {

                        //MessageBox.Show(line + "," + matches[0].Value + matches[1].Value);
                        string number1 = matches[0].Value;
                        string number2 = matches[1].Value;

                        angles.Add(double.Parse(number1));
                        radii.Add(double.Parse(number2));

                    }
                }

                string seriesName = "Series" + 1; // 获取数据系列的名称
                chart_polar_3.Series.Add(seriesName); // 添加数据系列
                double maxRadius = radii.Max();
                chart_polar_3.ChartAreas[0].AxisY.Maximum = Math.Ceiling(maxRadius); // 设置半径轴的最小值
                // 绘制极坐标图
                for (int i = 0; i < angles.Count; i++)
                {

                    chart_polar_3.Series[seriesName].Points.AddXY(angles[i], radii[i]);
                }
                // 设置图表的类型为极坐标
                foreach (var series in chart_polar_3.Series)
                {
                    series.ChartType = SeriesChartType.Polar;
                }
                // 设置极坐标图表的标签字体大小
                chart_polar_3.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Palatino Linotype", 14f);
                chart_polar_3.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Palatino Linotype", 14f);
                
                // 颜色
                chart_polar_3.Series[0].Color = Color.FromArgb( 255, 0, 0);
                // 设置极坐标图表中线条的粗细
                chart_polar_3.Series[0].BorderWidth = 4; // 以像素为单位
            }
        }
        // 获取文件夹内的文件路径
        private List<string> GetDatFiles(string folderPath)
        {
            List<string> datFilePaths = new List<string>();

            try
            {
                // 获取文件夹中所有的 .dat 文件
                string[] files = Directory.GetFiles(folderPath, "*.dat");

                // 将 .dat 文件的路径添加到列表中
                foreach (string file in files)
                {
                    datFilePaths.Add(file);
                }

                // 递归获取子文件夹中的 .dat 文件
                string[] subdirectories = Directory.GetDirectories(folderPath);
                foreach (string subdirectory in subdirectories)
                {
                    datFilePaths.AddRange(GetDatFiles(subdirectory));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return datFilePaths;
        }

        // 只允许输入数字
        private void textBox_header_start_3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        // 显示行号
        private void dataGridView_show_path_3_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }
        // 选中某一行后绘图
        private void dataGridView_show_path_3_SelectionChanged(object sender, EventArgs e)
        {
            

            //// 重置 Chart 控件的属性和设置到默认状态
            //chart_polar_3.ResetAutoValues();
            //chart_polar_3.ChartAreas.Add(new ChartArea());
            // 读取dat文件
            if (dataGridView_show_path_3.Rows.Count != 0 && dataGridView_show_path_3.Columns.Count != 0 && dataGridView_show_path_3.SelectedRows.Count>0)
            {
                
                string[] lines = File.ReadAllLines(dataGridView_show_path_3.SelectedRows[0].Cells[0].Value.ToString());

                // 使用正则表达式匹配每行数据
                string pattern = @"-?\d+(\.\d+)?";

                // 处理数据
                List<double> angles = new List<double>();
                List<double> radii = new List<double>();
                //MessageBox.Show(lines[0]);
                foreach (string line in lines)
                {
                    //MessageBox.Show(line);
                    MatchCollection matches = Regex.Matches(line, pattern);

                    if (matches.Count >= 2)
                    {

                        //MessageBox.Show(line + "," + matches[0].Value + matches[1].Value);
                        string number1 = matches[0].Value;
                        string number2 = matches[1].Value;

                        angles.Add(double.Parse(number1));
                        radii.Add(double.Parse(number2));

                    }
                }
                //dataGridView_show_path_3.ClearSelection();

                // 清除 Chart 控件中的数据和图形
                chart_polar_3.Series.Clear();
                chart_polar_3.ChartAreas.Clear();
                string seriesName = "Series" + 1; // 获取数据系列的名称
                chart_polar_3.Series.Add(seriesName); // 添加数据系列
                double maxRadius = radii.Max();
                // 创建 ChartArea
                chart_polar_3.ChartAreas.Add(new ChartArea("Polar"));
                // 设置半径轴的最大值
                chart_polar_3.ChartAreas["Polar"].AxisY.Maximum = Math.Ceiling(maxRadius); 
                // 绘制极坐标图
                for (int i = 0; i < angles.Count; i++)
                {
                    chart_polar_3.Series[seriesName].Points.AddXY(angles[i], radii[i]);
                }
                // 设置图表的类型为极坐标
                foreach (var series in chart_polar_3.Series)
                {
                    series.ChartType = SeriesChartType.Polar;
                }
                // 设置极坐标图表的标签字体大小
                chart_polar_3.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Palatino Linotype", 14f);
                chart_polar_3.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Palatino Linotype", 14f);
                // 颜色
                chart_polar_3.Series[0].Color = Color.FromArgb(255, 0, 0);
                // 设置极坐标图表中线条的粗细
                chart_polar_3.Series[0].BorderWidth = 4; // 以像素为单位
            }
        }

    }
}
