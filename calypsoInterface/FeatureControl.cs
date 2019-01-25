using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace calypsoInterface
{
    public partial class FeatureControl : UserControl
    {
        public FeatureControl()
        {
            InitializeComponent();
        }
        public Image icon;
        public string featureName;
        public bool clickFlag;
        public bool cancelFalg;
        public navAllData navAllDatas;
        public event Action<InputPoint> btnPointClick;

        private void FeatureControl_Load(object sender, EventArgs e)
        {
            this.pictureBox1.Image = icon;
            this.label1.Text = featureName;

        }
        private void tableLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!clickFlag)
            {
                this.tableLayoutPanel1.BackColor = Color.LightGray;
            }
        }
        private void tableLayoutPanel1_MouseLeave(object sender, EventArgs e)
        {
            if (!clickFlag)
            {
                this.tableLayoutPanel1.BackColor = Color.Transparent;
            }
        }
        private void tableLayoutPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!cancelFalg)
            {
                clickFlag = true;
                this.tableLayoutPanel1.BackColor = Color.Gray;
                cancelFalg = true;
                //

            }
            else
            {
                clickFlag = false ;
                this.tableLayoutPanel1.BackColor = Color.Transparent;
                cancelFalg = false ;
            }
        }
    }
}
