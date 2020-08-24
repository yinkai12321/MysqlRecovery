using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
                int MAX = 37648265;
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
                            count++;

                            tempLine += linia + Environment.NewLine;
                            //每隔10000次写入
                            if (count % 10000 == 0)
                            {
                                File.AppendAllText("qhdhuifu.sql", tempLine);
                                tempLine = "";
                            }
                        }
                    
                        n++;
                        if (n >= MAX)
                        {
                            File.AppendAllText("qhdhuifu.sql", tempLine);
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
    }
}
