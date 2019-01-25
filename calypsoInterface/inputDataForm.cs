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

namespace calypsoInterface
{
    public partial class inputDataForm : Form
    {
        public inputDataForm()
        {
            InitializeComponent();
        }
        public string pointFilePath;
        public InputPoint inputPoint;
        public bool confirmFlag;
       
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";
            dialog.Filter = "所有文件(*.txt*)|*.txt;*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                textBox1.Text = file;
                pointFilePath = file;
                //showMsg("选择点文件：" + file);
                getPointFile();//
            }
        }
        public void getPointFile()
        {
            if (readInputPointFile(pointFilePath) != null)//读取点文件
            {
                //显示点的信息到点datagridview中
                addInfo2PointDGV(inputPoint);
            }
        }
        public InputPoint readInputPointFile(string pointFilePath)
        {
            try
            {
                //showMsg("读取点数据文件 " + pointFilePath + " 开始");
                inputPoint = new InputPoint();
                StreamReader sr = new StreamReader(pointFilePath);
                string line;
                List<string> pointFile = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    pointFile.Add(line);
                }
                sr.Close();
                //inputPoint.model = pointFile[0].Split(':')[1];
                //inputPoint.partNo = pointFile[1].Split(':')[1];
                //inputPoint.unit = pointFile[2].Split(':')[1];
                //inputPoint.fileName = pointFile[3].Split(':')[1];
                inputPoint.fileName = pointFilePath.Split('\\')[pointFilePath.Split('\\').Count() - 1].Substring(0, pointFilePath.Split('\\')[pointFilePath.Split('\\').Count() - 1].Count() - 4);
                List<Point> pts = new List<Point>();
                for (int i = 1; i < pointFile.Count(); i++)
                {
                    string str;
                    str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(pointFile[i], " ");
                    string[] strList = str.Split(' ');
                    pts.Add(new Point
                    {
                        //pointNo = int.Parse(strList[1]),
                        //X = Convert.ToDouble(strList[2]),
                        //Y = Convert.ToDouble(strList[3]),
                        //Z = Convert.ToDouble(strList[4]),
                        //I = Convert.ToDouble(strList[5]),
                        //J = Convert.ToDouble(strList[6]),
                        //K = Convert.ToDouble(strList[7]),
                        pointNo = i,
                        X = Convert.ToDouble(strList[0]),
                        Y = Convert.ToDouble(strList[1]),
                        Z = Convert.ToDouble(strList[2]),
                        I = Convert.ToDouble(strList[3]),
                        J = Convert.ToDouble(strList[4]),
                        K = Convert.ToDouble(strList[5]),
                        name = "P" + i,
                        type = "spacePoint"
                    });
                }
                inputPoint.pointList = new List<Point>();
                inputPoint.pointList.AddRange(pts);
                //showMsg("读取点数据文件 " + pointFilePath + " 完成");
                return inputPoint;
            }
            catch (Exception ex)
            {
                //showMsg("读取点数据文件 " + pointFilePath + " 失败 " + ex.ToString());
                return null;
            }
        }
        private void addInfo2PointDGV(InputPoint inputPoint)
        {
            try
            {
                //showMsg("点数据开始显示到列表里");
                //for (int i = 0; i < dataGridView1.Rows.Count; i++)
                //{
                //    if (dataGridView1.Rows[i].Cells[0].Value != null)
                //    {
                //        if (dataGridView1.Rows[i].Cells[0].Value.ToString() == inputPoint.fileName)
                //        {
                //            MessageBox.Show("已经存在相同的点文件信息，请重新选择!");
                //            return;
                //        }
                //    }
                //}
                int startInsertIndex = dataGridView1.Rows.Count;
                //for (int j = 0; j < inputPoint.pointList.Count(); j++)
                //{
                //    dataGridView1.Rows.Add();
                //}
                //dataGridView1.Rows[startInsertIndex - 1].Cells[0].Value = inputPoint.fileName;
                for (int j = 0; j < inputPoint.pointList.Count(); j++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[startInsertIndex - 1].Cells[0].Value = inputPoint.pointList[j].pointNo;
                    dataGridView1.Rows[startInsertIndex - 1].Cells[1].Value = inputPoint.pointList[j].X;
                    dataGridView1.Rows[startInsertIndex - 1].Cells[2].Value = inputPoint.pointList[j].Y;
                    dataGridView1.Rows[startInsertIndex - 1].Cells[3].Value = inputPoint.pointList[j].Z;
                    dataGridView1.Rows[startInsertIndex - 1].Cells[4].Value = inputPoint.pointList[j].I;
                    dataGridView1.Rows[startInsertIndex - 1].Cells[5].Value = inputPoint.pointList[j].J;
                    dataGridView1.Rows[startInsertIndex - 1].Cells[6].Value = inputPoint.pointList[j].K;
                    startInsertIndex++;
                }
                //showMsg("点数据完成显示到列表里");
            }
            catch (Exception ex)
            {
                //showMsg("点数据显示到列表里失败 " + ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            confirmFlag = true;
            this.Close();
        }

        private void inputDataForm_Load(object sender, EventArgs e)
        {
            confirmFlag = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
