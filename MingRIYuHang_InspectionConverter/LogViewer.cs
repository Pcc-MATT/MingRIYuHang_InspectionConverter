using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace MingRIYuHang_InspectionConverter
{
    public partial class LogViewer : Form
    {
        public LogViewer()
        {
            InitializeComponent();
        }
        string startDate;
        string endDate;
        string filePath;
        DataTable dataTable;
        private void button1_Click(object sender, EventArgs e)
        {
            startDate = dateTimePicker1.Value.Date.ToString("yyyyMMdd");
            endDate = dateTimePicker2.Value.Date.ToString("yyyyMMdd");
            DateTime sD = dateTimePicker1.Value.Date;
            DateTime endD = dateTimePicker2.Value.Date;
            int days = (endD - sD).Days;

            dataTable = new DataTable();
            dataTable.Columns.Add("日期", Type.GetType("System.String"));
            dataTable.Columns.Add("时间", Type.GetType("System.String"));
            dataTable.Columns.Add("类型", Type.GetType("System.String"));
            dataTable.Columns.Add("内容", Type.GetType("System.String"));

            for (int i = 0; i <= days; i++)
            {
                filePath = System.Environment.CurrentDirectory + "\\log\\";
                filePath += sD.AddDays(i).ToString("yyyyMMdd") + ".txt";
                if (File.Exists(filePath))
                {
                    readLogFile(filePath);
                    dataGridView1.DataSource = dataTable;
                }
                else
                {
                    //ShowMsg(filePath + "--不存在！");
                }
            }
            dataGridView1.Columns[0].Width = 120;
            dataGridView1.Columns[1].Width = 170;
            dataGridView1.Columns[2].Width = 70;
            dataGridView1.Columns[3].Width =850;
            //dataGridView1.DataSource = dataTable;
        }
        private void readLogFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            //StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);

            //StringBuilder sb = new StringBuilder();
            //while (!sr.EndOfStream)
            //{
            //    sb.AppendLine(sr.ReadLine() + "<br>");
            //}
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("GB18030"));
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                DataRow dataRow = dataTable.NewRow();
                string[] lineList = line.Split(' ');
                dataRow[0] = lineList[0];
                Regex reg = new Regex(@"^\d{4}-\d{1,2}-\d{1,2}");
                if (reg.IsMatch(lineList[0])) {
                    dataRow[1] = lineList[1];
                    dataRow[2] = lineList[4];
                    dataRow[3] = lineList[9];
                }
                else
                {
                    dataRow[3] += " " + line + "\r\n";
                }
                
                dataTable.Rows.Add(dataRow);
            }
        }
    }
}
