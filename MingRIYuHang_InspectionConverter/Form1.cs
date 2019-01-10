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
using log4net;
using System.Reflection;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace MingRIYuHang_InspectionConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        InspectionFileAnalyse inspectionFileAnalyse;
        string inspectionPlanPath;
        string pointFilePath;
        InputPoint inputPoint;
        InputCircle inputCircle;
        bool creatNewInspectionPlanFlag;
        string newInspectionPlanName;
        int copyFileFolderIndex;
        List<InputCircle> circleDataList;


        private void tempLicense()
        {
            if (File.Exists(@"C:\ProgramData\Zeiss\SDCO\pd.dat"))
            {

            }
            else
            {
                string currentDate = DateTime.Now.ToShortDateString();
                StreamWriter sw = new StreamWriter(@"C:\ProgramData\Zeiss\SDCO\pd.dat");
                sw.WriteLine(currentDate);
                sw.Close();
            }

        }
        /// <summary>
        /// read input point
        /// </summary>
        /// <param name="pointFilePath"></param>
        /// <returns></returns>
        public InputPoint readInputPointFile(string pointFilePath)
        {
            try
            {
                showMsg("读取点数据文件 " + pointFilePath + " 开始");
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
                inputPoint.fileName = pointFilePath.Split('\\')[pointFilePath.Split('\\').Count()-1].Substring(0, pointFilePath.Split('\\')[pointFilePath.Split('\\').Count() - 1].Count()-4);
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
                showMsg("读取点数据文件 " + pointFilePath + " 完成");
                return inputPoint;
            }catch(Exception ex)
            {
                showMsg("读取点数据文件 " + pointFilePath + " 失败 "+ex.ToString());
                return null;
            }  
        }
        //读取圆文件信息
        public InputCircle readInputCircleFile(string pointFilePath)
        {
            try
            {
                showMsg("读取圆数据文件 " + pointFilePath + " 开始");
                inputCircle = new InputCircle();
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
                inputCircle.fileName = pointFilePath.Split('\\')[pointFilePath.Split('\\').Count() - 1].Substring(0, pointFilePath.Split('\\')[pointFilePath.Split('\\').Count() - 1].Count() - 4);
                List<Circle> pts = new List<Circle>();
                inputCircle.circleList = new List<Circle>();
                for (int i = 1; i < pointFile.Count(); i++)
                {
                    string str;
                    str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(pointFile[i], " ");
                    string[] strList = str.Split(' ');
                    //if(circleDataList[0].pointList.FindAll(n=>n.pointNo== int.Parse(strList[1])).Count == 0)//不含有相同的点序号
                    //{
                    double diameter = 0;
                    if (strList.Count() == 6)
                    {
                        diameter = 0;
                    }
                    else
                    {

                        diameter = Convert.ToDouble(strList[6]);
                    }
                    inputCircle.circleList.Add(new Circle
                        {
                            X = Convert.ToDouble(strList[0]),
                            Y = Convert.ToDouble(strList[1]),
                            Z = Convert.ToDouble(strList[2]),
                            I = Convert.ToDouble(strList[3]),
                            J = Convert.ToDouble(strList[4]),
                            K = Convert.ToDouble(strList[5]),
                            diameter = diameter,
                            type = "innerCircle"
                        });             
                }
                showMsg("读取圆数据文件 " + pointFilePath + " 完成");
                return inputCircle;
            }
            catch (Exception ex)
            {
                showMsg("读取圆数据文件 " + pointFilePath + " 失败 " + ex.ToString());
                return null;
            }
        }
        public void getCirclePointFile(string circlePointFilePath)
        {
            if (readInputCircleFile(circlePointFilePath) != null)//读取点文件
            {
                //显示点的信息到点datagridview中
                addInfo2CircleDGV(inputCircle);
            }
        }
        //将圆信息显示到圆列表里
        private void addInfo2CircleDGV(InputCircle inputCircle)
        {
            try
            {
                showMsg("圆数据开始显示到列表里");
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    if (dataGridView2.Rows[i].Cells[0].Value != null)
                    {
                        if (dataGridView2.Rows[i].Cells[0].Value.ToString() == inputCircle.fileName)
                        {
                            MessageBox.Show("已经存在相同的圆文件信息，请重新选择!");
                            return;
                        }
                    }
                }
                int startInsertIndex = dataGridView2.Rows.Count;
                int startInsertIndex2 = startInsertIndex;
                int circleCountIndex = 0;
                foreach (Circle circle in inputCircle.circleList)
                {
                    dataGridView2.Rows.Add();
                    circleCountIndex++;
                    dataGridView2.Rows[startInsertIndex - 1].Cells[1].Value = circleCountIndex;
                    dataGridView2.Rows[startInsertIndex - 1].Cells[2].Value = circle.X;
                    dataGridView2.Rows[startInsertIndex - 1].Cells[3].Value = circle.Y;
                    dataGridView2.Rows[startInsertIndex - 1].Cells[4].Value = circle.Z;
                    dataGridView2.Rows[startInsertIndex - 1].Cells[5].Value = circle.I;
                    dataGridView2.Rows[startInsertIndex - 1].Cells[6].Value = circle.J;
                    dataGridView2.Rows[startInsertIndex - 1].Cells[7].Value = circle.K;
                    dataGridView2.Rows[startInsertIndex - 1].Cells[7].Value = circle.K;
                    dataGridView2.Rows[startInsertIndex - 1].Cells[8].Value = circle.diameter;
                    //dataGridView2.Rows[startInsertIndex - 1].Cells[9].Value = circle.type;
                    startInsertIndex++;
                }
                dataGridView2.Rows[startInsertIndex2 - 1].Cells[0].Value = inputCircle.fileName;
                showMsg("圆数据完成显示到列表里");
            }
            catch (Exception ex)
            {
                showMsg("圆数据显示到列表里失败 " + ex.ToString());
            }
        }
        //将点信息显示到列表里
        private void addInfo2PointDGV(InputPoint inputPoint)
        {
            try
            {
                showMsg("点数据开始显示到列表里");
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value != null)
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString() == inputPoint.fileName)
                        {
                            MessageBox.Show("已经存在相同的点文件信息，请重新选择!");
                            return;
                        }
                    }

                }
                int startInsertIndex = dataGridView1.Rows.Count;
                for (int j = 0; j < inputPoint.pointList.Count(); j++)
                {
                    dataGridView1.Rows.Add();
                }
                dataGridView1.Rows[startInsertIndex - 1].Cells[0].Value = inputPoint.fileName;
                for (int j = 0; j < inputPoint.pointList.Count(); j++)
                {
                    dataGridView1.Rows[startInsertIndex - 1].Cells[1].Value = inputPoint.pointList[j].pointNo;
                    dataGridView1.Rows[startInsertIndex - 1].Cells[2].Value = inputPoint.pointList[j].X;
                    dataGridView1.Rows[startInsertIndex - 1].Cells[3].Value = inputPoint.pointList[j].Y;
                    dataGridView1.Rows[startInsertIndex - 1].Cells[4].Value = inputPoint.pointList[j].Z;
                    dataGridView1.Rows[startInsertIndex - 1].Cells[5].Value = inputPoint.pointList[j].I;
                    dataGridView1.Rows[startInsertIndex - 1].Cells[6].Value = inputPoint.pointList[j].J;
                    dataGridView1.Rows[startInsertIndex - 1].Cells[7].Value = inputPoint.pointList[j].K;
                    startInsertIndex++;
                }
                showMsg("点数据完成显示到列表里");
            }
            catch (Exception ex)
            {
                showMsg("点数据显示到列表里失败 " + ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";
            dialog.Filter = "所有文件(*.txt*)|*.txt;*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                textBox2.Text = file;
                pointFilePath = file;
                showMsg("选择点文件：" + file);
                getPointFile();//
            }
        }
        public void getPointFile()
        {
            if(readInputPointFile(pointFilePath)!=null)//读取点文件
            {
                //显示点的信息到点datagridview中
                addInfo2PointDGV(inputPoint); 
            }
        }
        //选择测量程序路径
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                textBox1.Text = foldPath;
                inspectionPlanPath = foldPath;
                inspectionFileAnalyse.inspectionPath = inspectionPlanPath + @"\inspection";
                showMsg("选择测量程序路径" + inspectionPlanPath);               
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            showMsg("软件启动开始");

            #region 临时许可证
            showMsg("验证临时许可证开始");
            tempLicense();
            StreamReader sr = new StreamReader(@"C:\ProgramData\Zeiss\SDCO\pd.dat");
            string oldDate = sr.ReadToEnd();
            sr.Close();
            DateTime dtOLD = Convert.ToDateTime(oldDate);
            DateTime currentDate = DateTime.Now;
            TimeSpan timeSpan = currentDate - dtOLD;
            if (timeSpan.TotalDays > 30)
            {
                showMsg("临时许可证验证失败");
                MessageBox.Show("试用版超期了！");
                Application.Exit();
            }
            showMsg("验证临时许可证完成"); 
            #endregion

            inspectionFileAnalyse = new InspectionFileAnalyse();
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        //显示操作信息和记录log
        private void showMsg(string msg)
        {
            string msg2Show = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "--" + msg;
            textBox4.Text += msg2Show+"\r\n";
            this.textBox4.Focus();//获取焦点
            this.textBox4.Select(this.textBox4.TextLength, 0);//光标定位到文本最后
            this.textBox4.ScrollToCaret();//滚动到光标处
            log.Info(msg);
        }
        //删除点列表里的数据
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                showMsg("开始删除点列表里的数据");
                //选中的行数
                int iCount = dataGridView1.SelectedRows.Count;
                if (iCount < 1)
                {
                    MessageBox.Show("Delete Data Fail!", "Error", MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                    return;
                }
                if (DialogResult.Yes == MessageBox.Show("是否删除选中的数据？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    if (dataGridView1.SelectedRows.Count == dataGridView1.Rows.Count)
                    {
                        this.dataGridView1.Rows.Clear();
                    }
                    else
                    {
                        for (int i = 0; i < this.dataGridView1.Rows.Count; i++)  //循环遍历所有行
                        {
                            if (true == this.dataGridView1.Rows[i].Selected)
                            {

                                this.dataGridView1.Rows.RemoveAt(i);
                                //if (i != 0)
                                //{
                                i = 0;
                                //}
                            }
                        }
                    }
                }
                showMsg("点列表数据删除完成");
            }catch(Exception ex)
            {
                showMsg("点列表数据删除失败 " + ex.ToString());
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                showMsg("导出开始");
                inspectionFileAnalyse.readInspectionFile();//读取原始测量程序
                //导出点
                if (this.dataGridView1.SelectedRows.Count != 0)
                {
                    //导出点
                    if (textBox6.Text == "")
                    {
                        MessageBox.Show("未输入点的开始名称，请输入！");
                        showMsg("未输入点的开始名称");
                        return;
                    }
                    string pointStartName = textBox6.Text.Trim();
                    //点数据导入inspection
                    showMsg("导出点数据开始");
                    List<Point> outputPointList = new List<Point>();
                    int pointNo = 0;
                    for (int i = 0; i < this.dataGridView1.Rows.Count - 1; i++)  //循环遍历所有行
                    {
                        if (true == this.dataGridView1.Rows[i].Selected)
                        {
                            pointNo++;
                            outputPointList.Add(new Point
                            {
                                pointNo = pointNo,
                                name = pointStartName + "-" + pointNo,
                                type = "spacePoint",
                                X = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value.ToString()),
                                Y = Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value.ToString()),
                                Z = Convert.ToDouble(dataGridView1.Rows[i].Cells[4].Value.ToString()),
                                I = Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value.ToString()),
                                J = Convert.ToDouble(dataGridView1.Rows[i].Cells[6].Value.ToString()),
                                K = Convert.ToDouble(dataGridView1.Rows[i].Cells[7].Value.ToString()),
                            });
                        }
                    }
                    inputPoint.pointList = outputPointList;
                    if (textBox1.Text != "" && textBox2.Text != "")
                    {
                        inspectionPlanPath = textBox1.Text;
                        if (creatNewInspectionPlanFlag)
                        {
                            inspectionFileAnalyse.inspectionPath = inspectionPlanPath + "\\" + newInspectionPlanName + @"\inspection";
                        }
                        else
                        {
                            inspectionFileAnalyse.inspectionPath = inspectionPlanPath + @"\inspection";
                        }
                        inspectionFileAnalyse.insertData2InspectionFile(inputPoint.pointList, creatNewInspectionPlanFlag,pointStartName);
                        creatNewInspectionPlanFlag = false;
                    }
                    showMsg("导出点数据完成");
                }
                //导出圆
                if (this.dataGridView2.SelectedRows.Count != 0)
                {
                    //导出圆
                    if (textBox7.Text == "")
                    {
                        MessageBox.Show("未输入圆的开始名称，请输入！");
                        showMsg("未输入圆的开始名称");
                        return;
                    }
                    string circleStartName = textBox7.Text.Trim();
                    showMsg("导出圆数据开始");
                    List<Circle> outputCircleList = new List<Circle>();
                    int circleNo = 0;
                    for (int i = 0; i < this.dataGridView2.Rows.Count - 1; i++)  //循环遍历所有行
                    {
                        if (true == this.dataGridView2.Rows[i].Selected)
                        {
                            circleNo++;
                            string type = "";
                            if ((bool)dataGridView2.Rows[i].Cells[9].EditedFormattedValue == false)
                            {
                                type = "innerCircle";
                            }
                            else
                            {
                                type = "outterCircle";
                            }
                            outputCircleList.Add(new Circle
                            {
                                name = circleStartName + "-" + circleNo,
                                X = Convert.ToDouble(dataGridView2.Rows[i].Cells[2].Value.ToString()),
                                Y = Convert.ToDouble(dataGridView2.Rows[i].Cells[3].Value.ToString()),
                                Z = Convert.ToDouble(dataGridView2.Rows[i].Cells[4].Value.ToString()),
                                I = Convert.ToDouble(dataGridView2.Rows[i].Cells[5].Value.ToString()),
                                J = Convert.ToDouble(dataGridView2.Rows[i].Cells[6].Value.ToString()),
                                K = Convert.ToDouble(dataGridView2.Rows[i].Cells[7].Value.ToString()),
                                diameter = Convert.ToDouble(dataGridView2.Rows[i].Cells[8].Value.ToString()),
                                type = type,
                            });
                        }
                    }
                    inputCircle.circleList = outputCircleList;
                    if (textBox1.Text != "" && textBox3.Text != "")
                    {
                        inspectionPlanPath = textBox1.Text;
                        if (creatNewInspectionPlanFlag)
                        {
                            inspectionFileAnalyse.inspectionPath = inspectionPlanPath + "\\" + newInspectionPlanName + @"\inspection";
                        }
                        else
                        {
                            inspectionFileAnalyse.inspectionPath = inspectionPlanPath + @"\inspection";
                        }
                        inspectionFileAnalyse.insertData2InspectionFile(inputCircle.circleList, creatNewInspectionPlanFlag,circleStartName);
                        creatNewInspectionPlanFlag = false;
                    }
                    showMsg("导出圆数据完成");
                }

                inspectionFileAnalyse.write2InspectionFile();//写入到原始测量程序

                MessageBox.Show("导出完成！");
            }catch(Exception ex)
            {
                showMsg("导出失败 " + ex.ToString());
            }
        }
        //准备创建新的测量程序
        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                showMsg("创建新测量程序开始");
                if (textBox1.Text != "" && textBox5.Text != "")
                {
                    newInspectionPlanName = textBox5.Text.Trim();
                    if (Directory.Exists(textBox1.Text) && Directory.Exists(System.Environment.CurrentDirectory + @"\Resources\SampleBlankInspectionPlan"))
                    {
                        if(Directory.Exists(textBox1.Text + "\\" + newInspectionPlanName))
                        {
                            MessageBox.Show("目标路径下已经存在 " + newInspectionPlanName + " 测量程序");
                        }
                        else
                        {
                            showMsg("在目标路径下创建测量程序开始");
                            copyFileFolderIndex = 0;
                            CopyDirectory(System.Environment.CurrentDirectory + @"\Resources\SampleBlankInspectionPlan", textBox1.Text, newInspectionPlanName);
                            showMsg("在目标路径下创建测量程序完成");
                        }
                    }
                    showMsg("更改目标路径下文件名称开始");
                    changeInspectionName(textBox1.Text + "\\" + newInspectionPlanName + "\\username", newInspectionPlanName);
                    showMsg("更改目标路径下文件名称完成");
                    creatNewInspectionPlanFlag = true;
                }
                else
                {
                    MessageBox.Show("请选择测量程序存放文件夹或者输入测量程序名称！");
                }
            }catch(Exception ex)
            {
                showMsg("准备创建新测量程序时出错 " + ex.ToString());
            }
           
        }

        //拷贝示例程序文件夹到指定文件夹
        public void CopyDirectory(string srcPath, string destPath,string newInspectionName)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (copyFileFolderIndex == 0)
                        {
                            if (!Directory.Exists(destPath + "\\" + newInspectionName))
                            {
                                Directory.CreateDirectory(destPath + "\\" + newInspectionName);   //目标目录下不存在此文件夹即创建子文件夹
                                copyFileFolderIndex = 1;
                            }
                        }
                        else
                        {
                            if (!Directory.Exists(destPath + "\\" + i.Name))
                            {
                                Directory.CreateDirectory(destPath + "\\" + newInspectionPlanName +"\\"+ i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                            }
                        }
                        CopyDirectory(i.FullName, destPath + "\\" + i.Name,newInspectionPlanName);    //递归调用复制子文件夹
                    }
                    else
                    {
                        if (copyFileFolderIndex == 0)
                        {
                            if (!Directory.Exists(destPath + "\\" + newInspectionName))
                            {
                                Directory.CreateDirectory(destPath + "\\" + newInspectionName);   //目标目录下不存在此文件夹即创建子文件夹
                                copyFileFolderIndex = 1;
                            }
                        }
                        File.Copy(i.FullName, destPath + "\\" + newInspectionName +"\\"+ i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        //改变new inspection plan name
        private void changeInspectionName(string path,string name)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("GB18030"));
            sw.WriteLine(name);
            sw.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";
            dialog.Filter = "所有文件(*.txt*)|*.txt;*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                textBox3.Text = file;
                showMsg("选择圆文件：" + file);
                getCirclePointFile(file);//
            }
        }
        //删除圆数据列表
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                showMsg("开始删除圆列表里的数据");
                //选中的行数
                int iCount = dataGridView2.SelectedRows.Count;
                if (iCount < 1)
                {
                    MessageBox.Show("Delete Data Fail!", "Error", MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                    return;
                }
                if (DialogResult.Yes == MessageBox.Show("是否删除选中的数据？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    if (dataGridView2.SelectedRows.Count == dataGridView2.Rows.Count)
                    {
                        this.dataGridView2.Rows.Clear();
                    }
                    else
                    {
                        for (int i = 0; i < this.dataGridView2.Rows.Count; i++)  //循环遍历所有行
                        {
                            if (true == this.dataGridView2.Rows[i].Selected)
                            {

                                this.dataGridView2.Rows.RemoveAt(i);
                                //if (i != 0)
                                //{
                                i = 0;
                                //}
                            }
                        }
                    }
                }
                showMsg("圆列表数据删除完成");
            }
            catch (Exception ex)
            {
                showMsg("圆列表数据删除失败 " + ex.ToString());
            }

        }
    }
}
