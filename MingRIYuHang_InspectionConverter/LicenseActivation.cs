using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThoughtWorks.QRCode.Codec;
using ZeissActivationLibrary;

namespace MingRIYuHang_InspectionConverter
{
    public partial class LicenseActivation : Form
    {
        public LicenseActivation()
        {
            InitializeComponent();
        }
        string dongleEncryptSN;
        string currentDongleSN;
        public bool activationState;
        public void dongleActivation()
        {
            #region lic验证
            var all = ZeissActivationServiceFactory.GetZeissActivationService();
            var dataAll = all.GetFulfillments();
            var dongleData = all.GetDongleList();//绑dongle
            var test = all.GetFingerprintId();
            if (dongleData.Count() == 0)
            {
                MessageBox.Show("请检查Dongle是否插好!");
                this.Close();
            }
            else
            {
                currentDongleSN = dongleData[0];
                ShowQRCode("8"+dongleData[0]+"8");
            }
            #endregion
        }
        public bool compareLicData(string currentDongleData)
        {
            string dongleDencrypt= MD5Encrypt64(dongleEncryptSN+"==");
            dongleDencrypt = dongleDencrypt.Substring(0, dongleDencrypt.Length - 2);
            {
                string settingValueSavePath = @"C:\ProgramData\Zeiss\License\";
                string fileName = currentDongleData;
                string fullFillPath = settingValueSavePath + fileName + ".lic";
                if (!File.Exists(fullFillPath))
                {
                    string text = textBox1.Text.Trim();
                    if (dongleDencrypt != text)
                    {
                        MessageBox.Show("注册码无效!");
                        return false;
                    }
                    else
                    {
                        StreamWriter sw = new StreamWriter(fullFillPath);
                        sw.WriteLine(textBox1.Text.Trim());
                        sw.Close();
                        MessageBox.Show("模块激活完成！");
                        return true;
                    }                   
                }
                StreamWriter sw1 = new StreamWriter(fullFillPath);
                sw1.WriteLine(textBox1.Text.Trim());
                sw1.Close();
                StreamReader sr = new StreamReader(fullFillPath);
                string line;
                List<string> allValuesList = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    allValuesList.Add(line.Trim());
                }
                sr.Close();
                bool activatedFlag = false;
                foreach (var data in allValuesList)
                {
                    if (dongleDencrypt != data)
                    {
                        activatedFlag = false;
                    }
                    else
                    {
                        activatedFlag = true;
                        break;
                    }
                }
                return activatedFlag;
            }
           
        }
        public static string MD5Encrypt64(string password)
        {
            string cl = password;
            //string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            return Convert.ToBase64String(s);
        }
        /// <summary>
        /// 显示生成的二维码
        /// </summary>
        public void ShowQRCode(string dongleSN)
        {
            dongleSN = MD5Encrypt64(dongleSN);
            dongleEncryptSN = dongleSN.Substring(0, dongleSN.Length - 2);
            Bitmap bimg = CreateQRCode(dongleSN.Substring(0, dongleSN.Length - 2));
            pictureBox2.Image = Image.FromHbitmap(bimg.GetHbitmap());
            //pictureBox2.Source = BitmapToBitmapImage(bimg);
            //ResetImageStrethch(imgQRcode, bimg);
        }
        public Bitmap CreateQRCode(string content)
        {
            QRCodeEncoder qrEncoder = new QRCodeEncoder();
            qrEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrEncoder.QRCodeScale = Convert.ToInt32(4);
            qrEncoder.QRCodeVersion = Convert.ToInt32(10);
            qrEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            try
            {
                Bitmap qrcode = qrEncoder.Encode(content, Encoding.UTF8);
                //if (!logoImagepath.Equals(string.Empty))
                //{
                //    Graphics g = Graphics.FromImage(qrcode);
                //    Bitmap bitmapLogo = new Bitmap(logoImagepath);
                //    int logoSize = Convert.ToInt32(txtLogoSize.Text);
                //    bitmapLogo = new Bitmap(bitmapLogo, new System.Drawing.Size(logoSize, logoSize));
                //    PointF point = new PointF(qrcode.Width / 2 - logoSize / 2, qrcode.Height / 2 - logoSize / 2);
                //    g.DrawImage(bitmapLogo, point);
                //}
                return qrcode;
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show("超出当前二维码版本的容量上限，请选择更高的二维码版本！", "系统提示");
                return new Bitmap(100, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show("生成二维码出错！", "系统提示");
                return new Bitmap(100, 100);
            }
        }

        private void LicenseActivation_Load(object sender, EventArgs e)
        {
            dongleActivation();
            string settingValueSavePath = @"C:\ProgramData\Zeiss\License\";
            string fileName = currentDongleSN;
            string fullFillPath = settingValueSavePath + fileName + ".lic";
            if (File.Exists(fullFillPath))
            {
                StreamReader sr = new StreamReader(fullFillPath);
                string line;
                List<string> allValuesList = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    allValuesList.Add(line.Trim());
                }
                sr.Close();
                bool activatedFlag = false;
                string dongleDencrypt = MD5Encrypt64(dongleEncryptSN + "==");
                dongleDencrypt = dongleDencrypt.Substring(0, dongleDencrypt.Length - 2);
                foreach (var data in allValuesList)
                {
                    if (dongleDencrypt != data)
                    {
                        activatedFlag = false;
                    }
                    else
                    {
                        activatedFlag = true;
                        break;
                    }
                }
                if (activatedFlag)
                {
                    activationState = true;
                    this.Hide();
                    Form1 form1 = new Form1();
                    form1.ShowDialog();
                }
                else
                {

                }
            }
        }
        public void checkActivationState()
        {
            dongleActivation();
            string settingValueSavePath = @"C:\ProgramData\Zeiss\License\";
            string fileName = currentDongleSN;
            string fullFillPath = settingValueSavePath + fileName + ".lic";
            if (File.Exists(fullFillPath))
            {
                StreamReader sr = new StreamReader(fullFillPath);
                string line;
                List<string> allValuesList = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    allValuesList.Add(line.Trim());
                }
                sr.Close();
                bool activatedFlag = false;
                string dongleDencrypt = MD5Encrypt64(dongleEncryptSN + "==");
                dongleDencrypt = dongleDencrypt.Substring(0, dongleDencrypt.Length - 2);
                foreach (var data in allValuesList)
                {
                    if (dongleDencrypt != data)
                    {
                        activatedFlag = false;
                    }
                    else
                    {
                        activatedFlag = true;
                        break;
                    }
                }
                if (activatedFlag)
                {
                    activationState = true;
                }
                else
                {

                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("请输入注册码！");
            }
            else
            {
                bool activatedRight = compareLicData(currentDongleSN);
                if (!activatedRight)
                {
                    
                    MessageBox.Show("模块未激活!请联系工程师！");
                }
                else
                {
                    if (File.Exists(@"C:\ProgramData\Zeiss\License\trail.lic"))
                    {
                        File.Delete(@"C:\ProgramData\Zeiss\License\trail.lic");
                    }
                    activationState = true;
                    this.Hide();
                    Form1 form1 = new Form1();
                    form1.ShowDialog();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string settingValueSavePath = @"C:\ProgramData\Zeiss\License\";
            string fileName = currentDongleSN;
            string fullFillPath = settingValueSavePath + fileName + ".lic";
            if (File.Exists(fullFillPath))
            {
                MessageBox.Show("已经注册过该软件，无法使用临时许可证");
            }
            else
            {
                StreamWriter sw = new StreamWriter(@"C:\ProgramData\Zeiss\License\trail.lic");
                sw.WriteLine(currentDongleSN);
                sw.Close();
                this.Hide();
                Form1 form1 = new Form1();
                form1.ShowDialog();
            }
        }
    }
}
