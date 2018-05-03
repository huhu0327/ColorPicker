using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ColorPicker
{
    public partial class Form1 : Form
    {
        private Bitmap _bitmap = new Bitmap(128, 128);
        private Bitmap _colorBitmap = new Bitmap(1, 1);
        private Size _sz = new Size(1, 1);
        private Graphics _graphic;
        private Color _color;
        private bool _isCapture = false;
        private Thread _t = null;
        private byte[] bytes;
        private string hexString;
        private bool _isExpand = false;

        private List<Color> _colorList = new List<Color>();
        private Bunifu.Framework.UI.BunifuFlatButton[] _buttonArray = new Bunifu.Framework.UI.BunifuFlatButton[12];

        public Form1()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            //이 프로그램이 항상 위로
            TopMost = true;

            _t = new Thread(new ThreadStart(Update));

            _t.Start();

            //Update();

            //크로스 스레드 에러 무시
            CheckForIllegalCrossThreadCalls = false;

            pictureBox8.Parent = pictureBox1;

            comboBox1.SelectedIndex = 0;

            this.Size = new Size(this.Size.Width, 259);

            for(int i = 0; i < 12; i++)
            {
                _colorList.Add(Color.Black);
            }

            _buttonArray[0] = uColor1;
            _buttonArray[1] = uColor2;
            _buttonArray[2] = uColor3;
            _buttonArray[3] = uColor4;
            _buttonArray[4] = uColor5;
            _buttonArray[5] = uColor6;
            _buttonArray[6] = uColor7;
            _buttonArray[7] = uColor8;
            _buttonArray[8] = uColor9;
            _buttonArray[9] = uColor10;
            _buttonArray[10] = uColor11;
            _buttonArray[11] = uColor12;

            for(int i = 0; i < 12; i++)
            {
                ChangeColor(_buttonArray[i], _colorList[i]);
            }

        }

        private void ChangeColor(Bunifu.Framework.UI.BunifuFlatButton button, Color color)
        {
            button.Activecolor = color;
            button.BackColor = color;
            button.Normalcolor = color;
            button.OnHovercolor = color;
        }

        private void ShiftColor(Color color)
        {
            for(int i = 11; i > 0; i--)
            {
                _colorList[i] = _colorList[i - 1];
            }
            _colorList[0] = color;

            for (int i = 0; i < 12; i++)
            {
                ChangeColor(_buttonArray[i], _colorList[i]);
            }
        }

        new private void Update()
        {
            while (true)
            {
                label2.Text = "( " + Cursor.Position.X.ToString() + ", " + Cursor.Position.Y.ToString() + ")";

                TrackMouse(Cursor.Position.X, Cursor.Position.Y);

                if (!_isCapture)
                {
                    _color = ScreenColor(Cursor.Position.X, Cursor.Position.Y);

                    pictureBox7.BackColor = _color;

                    TextColor();
                }

                Thread.Sleep(10);
            }
        }

        private void TrackMouse(int width, int height)
        {
            _graphic = Graphics.FromImage(_bitmap);

            _graphic.CopyFromScreen(new Point(width - 64, height - 64), new Point(0, 0), new Size(width + 64, height + 64));

            pictureBox1.Image = _bitmap;

            _graphic.Dispose();
        }

        private Color ScreenColor(int width, int height)
        {
            _graphic = Graphics.FromImage(_colorBitmap);
            _graphic.CopyFromScreen(width, height, 0, 0, _sz);
            _graphic.Dispose();
            return _colorBitmap.GetPixel(0, 0);
        }

        private void TextColor()
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    bunifuCustomTextbox1.Text = GetRgbColor(_color);
                    break;
                case 1:
                    bunifuCustomTextbox1.Text = GethtmlColor(_color);
                    break;
                default:
                    return;
            }
        }

        private string ToHexString(int nor)
        {
            bytes = BitConverter.GetBytes(nor);
            hexString = "";
            for (int i = 0; i < bytes.Length; i++)
                hexString += bytes[i].ToString("X2");

            return hexString;
        }


        //Html 색상 코드
        private string GethtmlColor(Color color)
        {
            return ToHexString(color.R).Substring(0, 2) + ToHexString(color.G).Substring(0, 2) + ToHexString(color.B).Substring(0, 2);
        }

        //Rgb 색상 코드
        private string GetRgbColor(Color color)
        {
            return color.R.ToString() + ", " + color.G.ToString() + ", " + color.B.ToString();
        }

        //현재 선택된 상태의 색상 코드
        private string GetStateColor(Color color)
        {
            return comboBox1.SelectedIndex == 0 ? GetRgbColor(color) : GethtmlColor(color);
        }

        private void bunifuImageButton1_Click_1(object sender, EventArgs e)
        {
            //_graphic.Dispose();
            //_colorBitmap.Dispose();
            //_bitmap.Dispose();
            if(!timer1.Enabled) timer1.Start();
        }

        private void bunifuImageButton2_Click_1(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void CopyColorCode(string txt)
        {
            Clipboard.SetDataObject(txt, true);
            Popup popup = new Popup();
            popup.ShowDialog();
            focusButton.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CopyColorCode(bunifuCustomTextbox1.Text);
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            Info info = new Info();
            info.ShowDialog();
            this.Focus();
        }

        private void focusButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P)
            {
                _isCapture = !_isCapture;
                if (_isCapture)
                {
                    ShiftColor(_color);
                    pictureBox4.BackColor = Color.Red;
                }
                else
                {
                    pictureBox4.BackColor = Color.White;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            focusButton.Focus();
            TextColor();
        }

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            if (!_isExpand)
            {
                this.Size = new Size(this.Size.Width, this.Size.Height + 200);
                bunifuFlatButton2.Iconimage = (Bitmap)Properties.Resources.ResourceManager.GetObject("icons8_Sort_Up_26px");
            }
            else
            {
                this.Size = new Size(this.Size.Width, this.Size.Height - 200);
                bunifuFlatButton2.Iconimage = (Bitmap)Properties.Resources.ResourceManager.GetObject("icons8_Sort_Down_26px");
            }
            focusButton.Focus();
            _isExpand = !_isExpand;
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            focusButton.Focus();
        }

        private void bunifuCustomTextbox1_Enter(object sender, EventArgs e)
        {
            focusButton.Focus();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Opacity > 0.0)
            {
                this.Opacity -= 0.025;
            }
            else
            {
                timer1.Stop();
                timer1.Dispose();
                _t.Abort();
                Dispose(true);
                Application.Exit();
            }
        }

        private void uColor1_Click(object sender, EventArgs e)
        {
            CopyColorCode(GetStateColor(uColor1.BackColor));
        }

        private void uColor2_Click(object sender, EventArgs e)
        {
            CopyColorCode(GetStateColor(uColor2.BackColor));
        }

        private void uColor3_Click(object sender, EventArgs e)
        {
            CopyColorCode(GetStateColor(uColor3.BackColor));
        }

        private void uColor4_Click(object sender, EventArgs e)
        {
            CopyColorCode(GetStateColor(uColor4.BackColor));
        }

        private void uColor5_Click(object sender, EventArgs e)
        {
            CopyColorCode(GetStateColor(uColor5.BackColor));
        }

        private void uColor6_Click(object sender, EventArgs e)
        {
            CopyColorCode(GetStateColor(uColor6.BackColor));
        }

        private void uColor7_Click(object sender, EventArgs e)
        {
            CopyColorCode(GetStateColor(uColor7.BackColor));
        }

        private void uColor8_Click(object sender, EventArgs e)
        {
            CopyColorCode(GetStateColor(uColor8.BackColor));
        }

        private void uColor9_Click(object sender, EventArgs e)
        {
            CopyColorCode(GetStateColor(uColor9.BackColor));
        }

        private void uColor10_Click(object sender, EventArgs e)
        {
            CopyColorCode(GetStateColor(uColor10.BackColor));
        }

        private void uColor11_Click(object sender, EventArgs e)
        {
            CopyColorCode(GetStateColor(uColor11.BackColor));
        }

        private void uColor12_Click(object sender, EventArgs e)
        {
            CopyColorCode(GetStateColor(uColor12.BackColor));
        }
    }
}
