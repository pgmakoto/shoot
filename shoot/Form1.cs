using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace shoot
{
    public partial class Form1 : Form
    {
        Point MouseLocationPrev = new Point();
        man man = new man();
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            man.Width = Width;
            man.Height = Height;

            man.tim.Tick += OnTimer;
            man.tim.Interval = 50;
            man.tim.Enabled = true;

            man.F_target = new fighter(new Quaternion(Width / 2, Height / 2, 0, 0), new Quaternion(1, 0, 0, 0));

        }

        private void OnTimer(object sender,EventArgs e)
        {
            man.refresh(0.02f);
            this.Invalidate();
        }

        float xdif;
        float ydif;

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                 xdif =( xdif + e.Location.X - MouseLocationPrev.X)*0.5f;
                 ydif =(ydif+ e.Location.Y - MouseLocationPrev.Y)*0.5f;
                float v = (float)Math.Sqrt(xdif * xdif + ydif * ydif);
                float vx =50f * xdif / v;
                float vy =50f * ydif / v;

                man.Add(e.Location, -vx, -vy);
            }
            MouseLocationPrev = e.Location;
            man.target = e.Location;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            man.Draw(e.Graphics);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            man.Width = Width;
            man.Height = Height;

        }
        float launchX = 10;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            int num = 1;
            if (e.Button == MouseButtons.Right) num = 2;
            while (num > 0)
            {
                man.Ignite(new PointF(launchX, Height - 50), 0, -1);
                launchX += 30;
                if (launchX > Width) launchX = 10;
                num--;
            }
        }
    }
}
