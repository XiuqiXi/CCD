using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CCD上位机
{
    public partial class calibration : Form
    {
        Series series1;
        int[] data_temp = new int[3648];
        int y_max = 100;
        int y_min = 0;

        public calibration()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void calibration_Load(object sender, EventArgs e)
        {
            CreateChart();
            createSeries();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Read_Data();
        }

        private void CreateChart()
        {

            ChartArea chartArea = new ChartArea();
            chartArea.Name = "FirstArea";

            chartArea.CursorX.AutoScroll = true;            //是否滚动
            chartArea.AxisX.ScrollBar.Enabled = true;       //开启滚动条
            chartArea.CursorX.IsUserEnabled = true;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.CursorX.SelectionColor = Color.SkyBlue;

            chartArea.CursorY.IsUserEnabled = true;
            chartArea.CursorY.AutoScroll = true;
            chartArea.CursorY.IsUserSelectionEnabled = true;
            chartArea.CursorY.SelectionColor = Color.SkyBlue;

            chartArea.CursorX.IntervalType = DateTimeIntervalType.Auto;


            chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;//启用X轴滚动条按钮


            chartArea.BackColor = Color.White;                      //背景色
            //chartArea.BackSecondaryColor = Color.White;                 //渐变背景色
            chartArea.BackGradientStyle = GradientStyle.TopBottom;      //渐变方式
            chartArea.BackHatchStyle = ChartHatchStyle.None;            //背景阴影
            chartArea.BorderDashStyle = ChartDashStyle.NotSet;          //边框线样式
            chartArea.BorderWidth = 1;                                  //边框宽度
            chartArea.BorderColor = Color.Black;




            // Axis
            //chartArea.AxisY.Title = @"Value";
            //chartArea.AxisY.LabelAutoFitMinFontSize = 5;
            //chartArea.AxisY.LineWidth = 2;
            //chartArea.AxisY.LineColor = Color.Black;
            //chartArea.AxisY.Enabled = AxisEnabled.True;
            chartArea.AxisY.IsLabelAutoFit = true;
            chartArea.AxisY.LabelAutoFitMinFontSize = 5;

            //chartArea.AxisX.Title = @"Time";
            chartArea.AxisX.IsLabelAutoFit = true;
            chartArea.AxisX.LabelAutoFitMinFontSize = 5;
            //chartArea.AxisX.LabelStyle.Angle = -15;


            chartArea.AxisX.LabelStyle.IsEndLabelVisible = true;        //show the last label
            chartArea.AxisX.Interval = 600;
            chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chartArea.AxisX.IntervalType = DateTimeIntervalType.NotSet;
            chartArea.AxisX.TextOrientation = TextOrientation.Auto;
            chartArea.AxisX.LineWidth = 2;
            chartArea.AxisX.LineColor = Color.Black;
            chartArea.AxisX.Enabled = AxisEnabled.True;
            chartArea.AxisX.ScaleView.MinSizeType = DateTimeIntervalType.Months;
            chartArea.AxisX.Crossing = 0;
           

            chartArea.Position.Height = 90;
            chartArea.Position.Width = 90;
            chartArea.Position.X = 5;
            chartArea.Position.Y = 5;

            chart_ecg.ChartAreas.Add(chartArea);
            chart_ecg.BackGradientStyle = GradientStyle.TopBottom;
            //图表的边框颜色、
            chart_ecg.BorderlineColor = Color.FromArgb(26, 59, 105);
            //图表的边框线条样式
            chart_ecg.BorderlineDashStyle = ChartDashStyle.Solid;
            //图表边框线条的宽度
            chart_ecg.BorderlineWidth = 0;
            //图表边框的皮肤(有点丑)
            //chart_ecg.BorderSkin.SkinStyle = BorderSkinStyle.FrameThin3;

            chart_ecg.ChartAreas[0].AxisX.Interval = 200;        //设置横坐标的分辨率
            chart_ecg.ChartAreas[0].AxisX.ScaleView.Size = 3800; //设置横坐标长度

            chart_ecg.ChartAreas[0].AxisY.Interval = 5000;        //设置横坐标的分辨率
            chart_ecg.ChartAreas[0].AxisY.ScaleView.Size = 50000; //设置横坐标长度

            //chart_ecg.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            //chart_ecg.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;

            //设置网格线  
            chart_ecg.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightSkyBlue;
            chart_ecg.ChartAreas[0].AxisX.MajorGrid.Interval = 200;//网格间隔
            chart_ecg.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;///线的样式

            chart_ecg.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightSkyBlue;
            chart_ecg.ChartAreas[0].AxisY.MajorGrid.Interval = 500;
            chart_ecg.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;///线的样式


            //chartArea.AxisX.MajorGrid.Enabled = true;
            //chartArea.AxisY.MajorGrid.Enabled = true;




        }
        private void createSeries()
        {
            //Series1
            int i;
            series1 = new Series();
            series1.ChartArea = "FirstArea";
            chart_ecg.Series.Add(series1);

            //Series1 style
            series1.ToolTip = "#VALX,#VALY";    //鼠标停留在数据点上，显示XY值

            series1.Name = "series1";
            series1.ChartType = SeriesChartType.Line;  // type:折线
            series1.BorderWidth = 1;
            series1.Color = Color.MediumSeaGreen;
            series1.XValueType = ChartValueType.Int32;//x axis type
            series1.YValueType = ChartValueType.Int32;//y axis type

            //Marker
            series1.MarkerStyle = MarkerStyle.Square;
            series1.MarkerSize = 2;
            series1.MarkerColor = Color.MediumSeaGreen;

            this.chart_ecg.Legends.Clear();

            for (i = 1; i < 3648; i++)
            {
                series1.Points.AddXY(i, data_temp[i]);   //画点
            }
        }



        private void Read_Data()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;   //是否允许多选
            dialog.Title = "Choose the sprectrum file";  //窗口title
            dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";   //可选择的文件类型
            string path = "";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = dialog.FileName;  //获取文件路径
            }
            
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                string str = sr.ReadToEnd();
                sr.Close();
                string[] H_data = str.Split(new char[] { '\n', '\r' });
                System.Console.WriteLine(H_data[2].ToString());

                for (int i = 0; i < data_temp.Length; i += 2)
                {
                    data_temp[i] = int.Parse(H_data[i].ToString());
                    //System.Console.WriteLine(H_data[i].ToString());
                }

                data_temp[0] = data_temp[1];
                y_max = data_temp.Max();
                y_min = data_temp.Min();
                //System.Console.WriteLine(y_min.ToString());
                series1.Points.Clear();////清屏
                for (int i = 0; i < 3648; i = i + 2)
                {
                    series1.Points.AddXY(i, data_temp[i]);   //画点
                }
            }
            catch
            {
                MessageBox.Show("Invaid path, please re-check", "ERROR");
            }
            
        }

        private void chart_ecg_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (IsNumber(textBox1.Text.Trim()) == false || IsNumber(textBox2.Text.Trim()) == false || IsNumber(textBox3.Text.Trim()) == false
                || IsNumber(textBox4.Text.Trim()) == false || IsNumber(textBox5.Text.Trim()) == false || IsNumber(textBox6.Text.Trim()) == false
                || IsNumber(textBox7.Text.Trim()) == false || IsNumber(textBox8.Text.Trim()) == false )
            {
                MessageBox.Show("Invaid inputs, please have a check", "ERROR");
            }
            else
            {
                string[] CCD_file = new string[7];
                CCD_file[0] = textBox1.Text;
                CCD_file[1] = textBox7.Text;
                CCD_file[2] = textBox6.Text;
                CCD_file[3] = textBox5.Text;
                CCD_file[4] = textBox4.Text;
                CCD_file[5] = textBox3.Text;
                CCD_file[6] = textBox2.Text;

                string[] HgAr = new string[7];
                HgAr[0] = comboBox1.SelectedItem.ToString();
                HgAr[1] = comboBox7.SelectedItem.ToString();
                HgAr[2] = comboBox6.SelectedItem.ToString();
                HgAr[3] = comboBox5.SelectedItem.ToString();
                HgAr[4] = comboBox4.SelectedItem.ToString();
                HgAr[5] = comboBox3.SelectedItem.ToString();
                HgAr[6] = comboBox2.SelectedItem.ToString();

                double[] coefficients = Curve_fitting(CCD_file, HgAr);

                DialogResult dr = saveFileDialog1.ShowDialog();
                string fileName = saveFileDialog1.FileName;
                if (dr == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(fileName))
                {
                    StreamWriter sw = new StreamWriter(fileName, true, Encoding.UTF8);
                    sw.Write("The fitting coefficients are \n");
                    for (int j = 0; j < coefficients.Length; j++) { sw.Write(coefficients[j] + "\n"); }
                    sw.Write("CCD pixels" + "\t" + "Standard wavelngth\n");
                    for (int j = 0; j < CCD_file.Length; j++) { sw.Write(CCD_file[j] + "\t" + HgAr[j] + "\n"); }
                    sw.Close();
                    MessageBox.Show("Save Successfully", "Info");
                }


            }

        }

        private double[] Curve_fitting(string[] x, string[] y)
        {
            if (x.Length != y.Length)
            {
                MessageBox.Show("Invaid inputs, please have a check", "ERROR");
            }

            string str = textBox8.Text;
            
            int n = int.Parse(str);

            double[] x_double = Array.ConvertAll<string, double>(x, delegate (string s) { return double.Parse(s); });
            double[] y_double = Array.ConvertAll<string, double>(y, delegate (string s) { return double.Parse(s); });

            double[] coefficients = MultiLine(x_double, y_double, x.Length, n);

            return coefficients;
        }

        private bool IsNumber(string oText)
        {
            try
            {
                int var1 = Convert.ToInt32(oText);
                return true;
            }
            catch
            {
                return false;
            }
        }

        ///<summary>
        ///用最小二乘法拟合二元多次曲线
        ///例如y=ax+b
        ///其中MultiLine将返回a，b两个参数。
        ///a对应MultiLine[1]
        ///b对应MultiLine[0]
        ///</summary>
        ///<param name="arrX">已知点的x坐标集合</param>
        ///<param name="arrY">已知点的y坐标集合</param>
        ///<param name="length">已知点的个数</param>
        ///<param name="dimension">方程的最高次数</param>
        public static double[] MultiLine(double[] arrX, double[] arrY, int length, int dimension)//二元多次线性方程拟合曲线
        {
            int n = dimension + 1;                  //dimension次方程需要求 dimension+1个 系数
            double[,] Guass = new double[n, n + 1];      //高斯矩阵 例如：y=a0+a1*x+a2*x*x
            for (int i = 0; i < n; i++)
            {
                int j;
                for (j = 0; j < n; j++)
                {
                    Guass[i, j] = SumArr(arrX, j + i, length);
                }
                Guass[i, j] = SumArr(arrX, i, arrY, 1, length);
            }
            return ComputGauss(Guass, n);
        }
        private static double SumArr(double[] arr, int n, int length) //求数组的元素的n次方的和
        {
            double s = 0;
            for (int i = 0; i < length; i++)
            {
                if (arr[i] != 0 || n != 0)
                    s = s + Math.Pow(arr[i], n);
                else
                    s = s + 1;
            }
            return s;
        }
        private static double SumArr(double[] arr1, int n1, double[] arr2, int n2, int length)
        {
            double s = 0;
            for (int i = 0; i < length; i++)
            {
                if ((arr1[i] != 0 || n1 != 0) && (arr2[i] != 0 || n2 != 0))
                    s = s + Math.Pow(arr1[i], n1) * Math.Pow(arr2[i], n2);
                else
                    s = s + 1;
            }
            return s;
        }
        private static double[] ComputGauss(double[,] Guass, int n)
        {
            int i, j;
            int k, m;
            double temp;
            double max;
            double s;
            double[] x = new double[n];
            for (i = 0; i < n; i++) x[i] = 0.0;//初始化
            for (j = 0; j < n; j++)
            {
                max = 0;
                k = j;
                for (i = j; i < n; i++)
                {
                    if (Math.Abs(Guass[i, j]) > max)
                    {
                        max = Guass[i, j];
                        k = i;
                    }
                }
                if (k != j)
                {
                    for (m = j; m < n + 1; m++)
                    {
                        temp = Guass[j, m];
                        Guass[j, m] = Guass[k, m];
                        Guass[k, m] = temp;
                    }
                }
                if (0 == max)
                {
                    // "此线性方程为奇异线性方程" 
                    return x;
                }
                for (i = j + 1; i < n; i++)
                {
                    s = Guass[i, j];
                    for (m = j; m < n + 1; m++)
                    {
                        Guass[i, m] = Guass[i, m] - Guass[j, m] * s / (Guass[j, j]);
                    }
                }
            }
            for (i = n - 1; i >= 0; i--)
            {
                s = 0;
                for (j = i + 1; j < n; j++)
                {
                    s = s + Guass[i, j] * x[j];
                }
                x[i] = (Guass[i, n] - s) / Guass[i, i];
            }
            return x;
        }


    }
}
