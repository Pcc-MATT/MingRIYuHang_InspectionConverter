using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace calypsoInterface
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        public InputPoint inputPoint;
        public navAllData navAllDatas;
        public int navPointIndex;
        public int navCircleIndex;
        #region Point
        //打开点文件，加载点信息
        private void button1_Click(object sender, EventArgs e)
        {
            inputPoint = new InputPoint();
            inputDataForm inputPointDataForm = new inputDataForm();
            inputPointDataForm.ShowDialog();
            if (inputPointDataForm.confirmFlag)
            {
                inputPoint = inputPointDataForm.inputPoint;
                showPointData();
                showPointGroup2Nav();
            }
        }
        public void showPointData()
        {
            dataGridView1.Columns.Clear();
            dgvPointDetailini();
            showPointData2dgv(inputPoint);
        }
        public void dgvPointDetailini()
        {
            DataGridViewTextBoxColumn NoIndex = new DataGridViewTextBoxColumn();
            NoIndex.Name = "No";
            NoIndex.HeaderText = "No.";
            dataGridView1.Columns.Add(NoIndex);
            DataGridViewTextBoxColumn X = new DataGridViewTextBoxColumn();
            X.Name = "X";
            X.HeaderText = "X";
            dataGridView1.Columns.Add(X);
            DataGridViewTextBoxColumn Y = new DataGridViewTextBoxColumn();
            Y.Name = "Y";
            Y.HeaderText = "Y";
            dataGridView1.Columns.Add(Y);
            DataGridViewTextBoxColumn Z = new DataGridViewTextBoxColumn();
            Z.Name = "Z";
            Z.HeaderText = "Z";
            dataGridView1.Columns.Add(Z);
            DataGridViewTextBoxColumn I = new DataGridViewTextBoxColumn();
            I.Name = "I";
            I.HeaderText = "I";
            dataGridView1.Columns.Add(I);
            DataGridViewTextBoxColumn J = new DataGridViewTextBoxColumn();
            J.Name = "J";
            J.HeaderText = "J";
            dataGridView1.Columns.Add(J);
            DataGridViewTextBoxColumn K = new DataGridViewTextBoxColumn();
            K.Name = "K";
            K.HeaderText = "K";
            dataGridView1.Columns.Add(K);
            //dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;//自动补全
        }
        public void showPointData2dgv(InputPoint inputPoint)
        {
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
        }
        private void showPointGroup2Nav()
        {
            navPointIndex++;
            navAllDatas.navPointDatas.Add(new navPointData
            {
                index = navPointIndex,
                inputPoint = inputPoint
            });
            
            FeatureControl featureControl = new FeatureControl();
            featureControl.Name = "point" + navPointIndex;
            featureControl.Enabled = true;
            Image image = Properties.Resources._99_Point;
            featureControl.icon = image;
            featureControl.featureName = "点组"+ navPointIndex;
            flowLayoutPanel2.Controls.Add(featureControl);
            featureControl.btnPointClick += new Action<InputPoint>(btPointClick_Event);
        } 
        private void btPointClick_Event(InputPoint inputPoint)
        {
            showPointData();
        }
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            FeatureControl featureControl = new FeatureControl();
            featureControl.Enabled = true;
            Image image = Properties.Resources._100_Circle;
            featureControl.icon = image;
            featureControl.featureName = "圆组1";
            flowLayoutPanel2.Controls.Add(featureControl);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            navAllDatas = new navAllData();
            navAllDatas.navPointDatas = new List<navPointData>();
        }
    }
}
