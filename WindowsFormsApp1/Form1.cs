using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var filename = dialog.FileName;

                var file = new StreamReader(filename);
                int ini = 0;
                int MAX = requestMethod(filename);
                int MIN = 0;
                int n = MIN;
                var count = 1;

                progressBar1.Minimum = MIN;
                progressBar1.Maximum = MAX;

                //缓存行
                var tempLine = "";

                Task.Factory.StartNew(() =>
                {
                    while (!file.EndOfStream)
                    {
                        string linia = file.ReadLine();
                        ini++;
                        if (ini < MIN)
                            continue;

                        if (linia.Contains("###"))
                        {

                            linia = linia.Replace("###", "");
                            linia = linia.Replace("WHERE", "select");
                            linia = linia.Replace("DELETE FROM", ";insert into");

                            if (Regex.Match(linia, @"@\d+\=").Value == "@1=")
                            {
                                linia = Regex.Replace(linia, @"@\d+\=", "");
                            }
                            else
                            {
                                linia = Regex.Replace(linia, @"@\d+\=", ",");
                            }

                            if (linia.EndsWith("?"))
                            {
                                linia = linia.Replace("?", "'");
                            }

                            if (count == 1)
                            {
                                linia = linia.Replace(";", "");
                            }

                            //如果出现 单引号 的次数大于3 
                            if (linia.Split('\'').Length - 1 == 3)
                            {
                                linia = "   ,''";
                            }

                            count++;

                            tempLine += linia + Environment.NewLine;
                            //每隔10000次写入
                            if (count % 10000 == 0)
                            {
                                File.AppendAllText("恢复数据.sql", tempLine);
                                tempLine = "";
                            }
                        }
                    
                        n++;
                        if (n >= MAX)
                        {
                            File.AppendAllText("恢复数据.sql", tempLine);
                            tempLine = "";
                            break;
                        }
                        if (n % 10000 == 0)
                        {
                            this.Invoke(new Action(delegate {
                                progressBar1.Value = n;
                                label1.Text = n + "";
                            }));
                        }

                    }
                    file.Close();

                    this.Invoke(new Action(delegate { 
                        MessageBox.Show("成功");

                    }));
                });
                
            }

        }

        /// <summary>
        /// 乱码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var filename = dialog.FileName;

                var file = new StreamReader(filename);
                int ini = 0;
                int MAX = requestMethod(filename);
                int MIN = 0;
                int n = MIN;
                var count = 1;

                progressBar1.Minimum = MIN;
                progressBar1.Maximum = MAX;


                //缓存行
                var tempLine = "";

                Task.Factory.StartNew(() =>
                {
                    while (!file.EndOfStream)
                    {
                        string linia = file.ReadLine();
                        ini++;
                        if (ini < MIN)
                            continue;


                        //如果出现 单引号 的次数大于3 
                        if (linia.Split('\'').Length - 1 >= 3)
                        {
                            linia = "   ,''";
                        }

                        count++;

                        //每隔10000次写入
                        if (count % 10000 == 0)
                        {
                            File.AppendAllText("乱码恢复.sql", tempLine);
                            tempLine = "";
                        }

                        tempLine += linia + Environment.NewLine;
                        n++;
                        if (n >= MAX)
                        {
                            File.AppendAllText("乱码恢复.sql", tempLine);
                            tempLine = "";
                            break;
                        }
                        if (n % 10000 == 0)
                        {
                            this.Invoke(new Action(delegate {
                                progressBar1.Value = n;
                                label1.Text = n + "";
                            }));
                        }

                    }
                    file.Close();

                    this.Invoke(new Action(delegate {
                        MessageBox.Show("成功");

                    }));
                });

            }
        }


        //读取txt文件中总行数的方法
        public static int requestMethod(String _fileName)
        {
            Stopwatch sw = new Stopwatch();
            var path = _fileName;
            int lines = 0;

            //按行读取
            sw.Restart();
            using (var sr = new StreamReader(path))
            {
                var ls = "";
                while ((ls = sr.ReadLine()) != null)
                {
                    lines++;
                }
            }
            sw.Stop();
            return lines;
        }
    }
}
