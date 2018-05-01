using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorPicker
{
    enum State
    {
        FadeIn,
        FadeOut,
        Show
    }

    public partial class Popup : Form
    {
        private State state = State.FadeIn;
        public Popup()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            this.Location = new Point(Screen.PrimaryScreen.Bounds.Width / 2 - this.Size.Width / 2, Screen.PrimaryScreen.Bounds.Height - this.Size.Height * 2);

            this.TopMost = true;

            this.Opacity = 0.0f;

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (state)
            {
                case State.FadeIn:
                    if(this.Opacity < 1.0f)
                    {
                        this.Opacity += 0.05f;
                    }else
                    {
                        state = State.Show;
                    }
                    break;
                case State.FadeOut:
                    if (this.Opacity > 0.0f)
                    {
                        this.Opacity -= 0.05f;
                    }
                    else
                    {
                        this.Dispose();
                        this.Close();
                    }
                    break;
                case State.Show:
                    //System.Threading.Thread.Sleep(3000);
                    state = State.FadeOut;
                    break;
            }
        }
    }
}
