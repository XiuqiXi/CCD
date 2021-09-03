using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.IO.Ports;



using System.IO;


namespace CCD上位机
{
    public partial class Form1 : Form
    {
        private char[] data;
        private Thread t;
        private Thread t1;
        Socket clientSocket;
        bool server_open_flag = false;
        bool server_open_change = false;
        bool tcp_sta_change = false;
        bool main_form_close_flag = false;
        Series series1;
        int osc_x = 1;  //波形横坐标
        private List<byte> data_buffer = new List<byte>(7296);//默认分配1页内存，并始终限制不允许超过
        private List<byte> ecg_buffer = new List<byte>(4096);//默认分配1页内存，并始终限制不允许超过
        bool ReceiveData_Flag;    //置位接收标志，防止在接收过程中关闭串口
        int[] data_temp = new int[3648];


        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            createSeries();
            CreateChart();
            comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());    //获取可用串口号
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
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
            chartArea.AxisX.Interval = 10;
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
            chart_ecg.BorderlineWidth = 1;
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
            chart_ecg.ChartAreas[0].AxisX.MajorGrid.LineDashStyle= ChartDashStyle.Dash;///线的样式

            chart_ecg.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightSkyBlue;
            chart_ecg.ChartAreas[0].AxisY.MajorGrid.Interval = 500;
            chart_ecg.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;///线的样式


            chartArea.AxisX.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.Enabled = true;




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
            series1.Color = Color.Red;
            series1.XValueType = ChartValueType.Int32;//x axis type
            series1.YValueType = ChartValueType.Int32;//y axis type

            //Marker
            series1.MarkerStyle = MarkerStyle.Square;
            series1.MarkerSize = 2;
            series1.MarkerColor = Color.Red;

            this.chart_ecg.Legends.Clear();

            for (i = 1; i < 3648; i++)
            {
                series1.Points.AddXY(i, data_temp[i]);   //画点
            }
        }

        public void Write()
        {
            FileStream fs = new FileStream("E:\\ak.txt", FileMode.Create);
            //获得字节数组
            byte[] data = System.Text.Encoding.Default.GetBytes("Hello World!");
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }

        //处理TCP透传信息
        private void deal_data()
        {
            int i;            
            while (true)
            {
                int buffer_num = data_buffer.Count;    //提前存好，防止处理过程中Count变化
                if(comboBox3.SelectedIndex == 0)////AD采样位数设置设置
                {
                    if (buffer_num >= 7296)   //至少4字节
                    {
                        for (i = 1; i < 3648; i++) data_temp[i] = data_buffer[2 * i + 1] * 256 + data_buffer[2 * i];
                        Action act = delegate ()
                        {
                            series1.Points.Clear();////清屏
                            for (i = 1; i < 3648; i++)
                            {
                                
                                series1.Points.AddXY(i, data_temp[i]);   //画点
                            }
                        };
                        this.Invoke(act);
                        data_buffer.Clear();
                    }

                }
                else if (comboBox3.SelectedIndex == 1)////AD采样位数设置设置
                {
                    if (buffer_num >= 3648)   //至少4字节
                    {
                        for (i = 1; i < 3648; i++) data_temp[i] = data_buffer[i] * 16;
                        Action act = delegate ()
                        {
                            series1.Points.Clear();////清屏
                            for (i = 1; i < 3648; i++)
                            {                                
                                series1.Points.AddXY(i, data_temp[i]);   //画点
                            }
                        };
                        this.Invoke(act);
                        data_buffer.Clear();
                    }
                }
                else if (comboBox3.SelectedIndex == 2)////AD采样位数设置设置
                {
                    if (buffer_num >= 12)   //至少4字节
                    {
                        if (data_buffer[0] == 0XFE)
                        {
                            textBox3.Text = Convert.ToString(data_buffer[1] * 100 + data_buffer[2]);   //最大坐标
                            textBox4.Text = Convert.ToString(data_buffer[3] * 100 + data_buffer[4]);   //最小坐标
                            textBox5.Text = Convert.ToString(data_buffer[5] * 100 + data_buffer[6]);   //平均值
                            textBox6.Text = Convert.ToString(data_buffer[7] * 100 + data_buffer[8]);   //最大值
                            textBox7.Text = Convert.ToString(data_buffer[9] * 100 + data_buffer[10]);   //最小值
                            //series1.Points.AddXY(Convert.ToInt16(textBox3.Text), Convert.ToInt16(textBox6.Text));   //画最大点
                            //series1.Points.AddXY(Convert.ToInt16(textBox4.Text), Convert.ToInt16(textBox7.Text));   //画最大点
                            
                        }
                        data_buffer.Clear();
                    }
                    if (main_form_close_flag == true)
                    {
                        t1.Abort();      //结束本线程
                    }
                }

                if (main_form_close_flag == true)
                {
                    t1.Abort();      //结束本线程
                }
            }
        }
        UInt64 receive_tick = 0;

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)//接收处理事件
        {
            this.BeginInvoke((EventHandler)(delegate    //开一个单独线程做数据处理
            {
                ReceiveData_Flag = true;    //置位接收标志，防止在接收过程中关闭串口
                try
                {
                    int num = serialPort1.BytesToRead;  //获取接收缓冲区的字节数
                    if (num != 0)                       //若数据不为空
                    {
                        receive_tick = time_tick;   //记录当前时间
                        byte[] received_buf = new byte[num];    //创建一个大小为num的字节数据用于存放
                        serialPort1.Read(received_buf, 0, num); //读取接收缓冲区中num个字节到数组中
                        data_buffer.AddRange(received_buf); //存入缓存中
                    }
                }
                catch (Exception ex)
                {
                    //System.Media.SystemSounds.Beep.Play();
                    //MessageBox.Show("错误:" + ex.Message, "提示");
                }
                ReceiveData_Flag = false;//清空接收标志

            }
            ));
        }




        public void OpenPort()//打开串口，供委托调用
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = 256000;////921600
                serialPort1.DataBits = 8;
                serialPort1.Parity = System.IO.Ports.Parity.None;
                serialPort1.StopBits = System.IO.Ports.StopBits.One;
                serialPort1.Open();
            }
            catch
            {
                MessageBox.Show("Fail to open the serial port, please re - check", "ERROR");
            }
        }
        public void ClosePort()//关闭串口，供委托调用
        {
            try
            {
                while (ReceiveData_Flag == true) ;  //等待本次接收处理完成
                serialPort1.Close();
            }
            catch (Exception ex) { MessageBox.Show("ERROR:" + ex.Message, "INFO"); }
        }

        UInt64 time_tick = 5000;
        bool tcp_flag = false;

       


       
        //连接串口按键
        private void button_start_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == false)
            {
                OpenPort();
                if (serialPort1.IsOpen == true)
                {
                    t1 = new Thread(deal_data);     //开启线程执行循环处理消息
                    t1.Start();
                    button_start.Text = "断开连接";
                    comboBox2.SelectedIndex = 0;////积分时间设置
                    comboBox3.SelectedIndex = 0;////AD采样位数设置设置
                }
            }
            else
            {
                main_form_close_flag = true;
                button_start.Text = "连接";
                ClosePort();
            }
        }

        //向串口发送数据
        public void SendCommand(byte[] writeBytes)
        {
            //byte[] WriteBuffer = Convert.ToByte(CommandString,32);
            serialPort1.Write(writeBytes, 0, writeBytes.Length);
        }

        /// <summary>
        /// 发送字节
        /// </summary>
        /// <param name="port_DataWrite">要发送的字节</param>
        /// <returns></returns>
        private void port_DataWrite(char[] writeChars)
        {
            System.Console.WriteLine(writeChars);
            string str = new string(writeChars);
            byte[] writeBytes;
            writeBytes = Encoding.Default.GetBytes(str);
            serialPort1.Write(writeBytes, 0, writeBytes.Length);
        }

        private byte[] FromStrtoBytes(string str)
        {
            byte[] btyes = new byte[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                if (str != null)
                {
                    if (str.Length > 0)
                    {
                        btyes[i] = Convert.ToByte(str[i]);
                    }
                }
            }
            return btyes;
        }

        private void start_DataWrite()//开始采集按键
        {
            char[] data;
            string str;
            str = "@c0080#@";
            data = str.ToCharArray(); 
            /*
            if (comboBox3.SelectedIndex == 0)////AD采样位数设置设置
                data[0] = sender[0];
            else if (comboBox3.SelectedIndex == 1)////AD采样位数设置设置
                data[0] = 0xA2;
            else if (comboBox3.SelectedIndex == 2)////AD采样位数设置设置
                data[0] = 0xA3;
            */
            if (serialPort1.IsOpen == true)
            {
                port_DataWrite(data);
                data_buffer.Clear();////清理串口的所有数据
            }
            else
            {
                timer2.Stop();//启动定时采集按钮
                MessageBox.Show("Fail to open the serial port, please re-check", "ERROR");
                //timer2.Stop();//启动定时采集按钮
            }
        }

        //开始采集按键
        private void button1_Click(object sender, EventArgs e)
        {
            start_DataWrite();//开始采集按键


        }
       


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            main_form_close_flag = true;
            System.Windows.Forms.Application.Exit();
            System.Environment.Exit(0);//结束进程时，关闭所有线程  这个很重要，如果没有这个代码，页面关闭了，线程还在开启着
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void checkbox_chart_mode_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chart_ecg_Click(object sender, EventArgs e)
        {

        }

        //组合框中选项改变的事件
        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //当组合框中选择的值发生变化时弹出消息框显示当前组合框中选择的值
            
            char[] data;
            string str = "@c0080#@";
            data = str.ToCharArray();

            string str_0 = "@c0000#@";
            string str_1 = "@c0001#@";
            string str_2 = "@c0002#@";
            string str_3 = "@c0003#@";
            string str_4 = "@c0004#@";
            string str_5 = "@c0005#@";
            string str_6 = "@c0006#@";
            string str_7 = "@c0007#@";
            string str_8 = "@c0008#@";
            string str_9 = "@c0009#@";
            string str_10 = "@c00010#@";
            string str_11 = "@c00011#@";
            string str_12 = "@c00012#@";
            string str_13 = "@c00013#@";
            string str_14 = "@c00014#@";
            string str_15 = "@c00015#@";
            string str_16 = "@c00016#@";

            if (comboBox2.SelectedIndex == 0) data = str_0.ToCharArray();
            else if (comboBox2.SelectedIndex == 1) data = str_1.ToCharArray();
            else if (comboBox2.SelectedIndex == 2) data = str_2.ToCharArray();
            else if (comboBox2.SelectedIndex == 3) data = str_3.ToCharArray();
            else if (comboBox2.SelectedIndex == 4) data = str_4.ToCharArray();
            else if (comboBox2.SelectedIndex == 5) data = str_5.ToCharArray();
            else if (comboBox2.SelectedIndex == 6) data = str_6.ToCharArray();
            else if (comboBox2.SelectedIndex == 7) data = str_7.ToCharArray();
            else if (comboBox2.SelectedIndex == 8) data = str_8.ToCharArray();
            else if (comboBox2.SelectedIndex == 9) data = str_9.ToCharArray();
            else if (comboBox2.SelectedIndex == 10) data = str_10.ToCharArray();
            else if (comboBox2.SelectedIndex == 11) data = str_11.ToCharArray();
            else if (comboBox2.SelectedIndex == 12) data = str_12.ToCharArray();
            else if (comboBox2.SelectedIndex == 13) data = str_13.ToCharArray();
            else if (comboBox2.SelectedIndex == 14) data = str_14.ToCharArray();
            else if (comboBox2.SelectedIndex == 15) data = str_15.ToCharArray();
            else if (comboBox2.SelectedIndex == 16) data = str_16.ToCharArray();

            if (serialPort1.IsOpen == true)
            {
                port_DataWrite(data);
                data_buffer.Clear();////清理串口的所有数据
            }
            else
            {
                MessageBox.Show("Fail to open the serial port, please re-check", "ERROR");
                ///this.comboBox2.SelectedIndex = 1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            //FileStream fs = new FileStream("E:\\dat.txt", FileMode.Create);
            FileStream fs = new FileStream(textBox1.Text, FileMode.Create);
            //获得字节数组
            string result = string.Join("\r\n", data_temp);
            byte[] data = System.Text.Encoding.Default.GetBytes(result);
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {

                int num11 = Convert.ToInt32(textBox2.Text);
                //设置每隔1秒调用一次定时器Tick事件
                timer2.Interval = num11;
                
                timer2.Start();//启动定时采集按钮
            }
            else
            {
                timer2.Stop();//启动定时采集按钮
            }
                
        }


        //触发定时器的事件，在该事件中切换图片
        private void timer2_Tick(object sender, EventArgs e)
        {
            start_DataWrite();//开始采集按键
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int num11 = Convert.ToInt32(textBox2.Text);
            //设置每隔1秒调用一次定时器Tick事件
            timer2.Interval = num11;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void chart_ecg_Click_1(object sender, EventArgs e)
        {

        }
    }
}
