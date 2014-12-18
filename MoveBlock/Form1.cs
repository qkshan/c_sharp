/********************************************************************
	created:	2014/12/18
	created:	18:12:2014   11:06
	filename: 	f:\document\document\visual studio 2010\Projects\MoveBlock\MoveBlock\Form1.cs
	file path:	f:\document\document\visual studio 2010\Projects\MoveBlock\MoveBlock
	file base:	Form1
	file ext:	cs
	author:		Qkshan
	
	purpose:	
*********************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MoveBlock
{
    public partial class Form1 : Form
    {
        const int N = 4; //按钮的行、列数
        Button[,] buttons = new Button[N, N]; //按钮的数组

        bool m_GameisStart = false;
        UInt32 m_GameCount = 0;
        Button blankBtn;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!m_GameisStart)
            {
                //打乱顺序
                Shuffle();
                timer1.Start();
                m_GameCount = 0;
                m_GameisStart = true;
            }else
            {
                timer1.Stop();
                m_GameisStart = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //产生所有按钮
            GenerateAllButtons();

            // set the caption
            this.Text = "十五指游戏";
        }

        //打乱顺序
        void Shuffle()
        {
            //多次随机交换两个按钮
            Random rnd = new Random();
            for (int i = 0; i < 100; i++)
            {
                int a = rnd.Next(N);
                int b = rnd.Next(N);
                int c = rnd.Next(N);
                int d = rnd.Next(N);
                Swap(buttons[a, b], buttons[c, d]);
            }

            blankBtn = FindHiddenButton();
        }

        //生成所有的按钮
        void GenerateAllButtons()
        {
            int x0 = 100, y0 = 10, w = 45, d = 50;

            for (int r = 0; r < N; r++)
                for (int c = 0; c < N; c++)
                {
                    int num = r * N + c;
                    Button btn = new Button();
                    btn.Text = (num + 1).ToString();
                    btn.Top = y0 + r * d;
                    btn.Left = x0 + c * d;
                    btn.Width = w;
                    btn.Height = w;
                    btn.Visible = true;
                    btn.Tag = r * N + c; //这个数据用来表示它所在行列位置

                    //注册事件
                    btn.Click += new EventHandler(btn_Click);

                    buttons[r, c] = btn; //放到数组中
                    this.Controls.Add(btn); //加到界面上
                }

            buttons[N - 1, N - 1].Visible = false; //最后一个不可见
            blankBtn = buttons[N - 1, N - 1];
        }

        //交换两个按钮
        void Swap(Button btna, Button btnb)
        {
            string t = btna.Text;
            btna.Text = btnb.Text;
            btnb.Text = t;

            bool v = btna.Visible;
            btna.Visible = btnb.Visible;
            btnb.Visible = v;
        }

        //按钮点击事件处理
        void btn_Click(object sender, EventArgs e)
        {
            if(!m_GameisStart)
            {
                return;
            }

            Button btn = sender as Button; //当前点中的按钮
            Button blank = blankBtn;  //空白按钮

            //判断是否与空白块相邻，如果是，则交换
            if (IsNeighbor(btn, blank))
            {
                Swap(btn, blank);
                blank.Focus();
                blankBtn = btn;            //判断是否完成了
            }

            if (ResultIsOk())
            {
                MessageBox.Show("ok");
            }
        }

        //查找要隐藏的按钮
        Button FindHiddenButton()
        {
            for (int r = 0; r < N; r++)
                for (int c = 0; c < N; c++)
                {
                    if (!buttons[r, c].Visible)
                    {
                        return buttons[r, c];
                    }
                }
            return null;
        }

        //判断是否相邻
        bool IsNeighbor(Button btnA, Button btnB)
        {
            int a = (int)btnA.Tag; //Tag中记录是行列位置
            int b = (int)btnB.Tag;
            int r1 = a / N, c1 = a % N;
            int r2 = b / N, c2 = b % N;

            if (r1 == r2 && (c1 == c2 - 1 || c1 == c2 + 1) //左右相邻
                || c1 == c2 && (r1 == r2 - 1 || r1 == r2 + 1))
                return true;
            return false;
        }

        //检查是否完成
        bool ResultIsOk()
        {
            for (int r = 0; r < N; r++)
                for (int c = 0; c < N; c++)
                {
                    if (buttons[r, c].Text != (r * N + c + 1).ToString())
                    {
                        return false;
                    }
                }
            return true;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            if(m_GameisStart)
            {
                button1.BackColor = Color.FromArgb(255, 0, 0);
            }
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            if (m_GameisStart)
            {
                button1.BackColor = Form1.DefaultBackColor;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(m_GameisStart)
            {
                m_GameCount++;
                button1.Text = m_GameCount.ToString();
            }
            else
            {
                timer1.Stop();
            }
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            if (m_GameisStart)
            {
                button1.BackColor = Color.FromArgb(255, 0, 0);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:                
                case Keys.Left:
                case Keys.Right:
                case Keys.Down:
                    moveBtnByKeys(keyData);
                    return true;

                case Keys.Enter:
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void moveBtnByKeys(Keys keyData)
        {
            if (!m_GameisStart)
            {
                return;
            }

            int tgt_r = (int)blankBtn.Tag / N;
            int tgt_c = (int)blankBtn.Tag % N;

            switch (keyData)
            {
                case Keys.Up:
                    tgt_r++;
                    break;
                case Keys.Left:
                    tgt_c++;
                    break;
                case Keys.Right:
                    tgt_c--;
                    break;
                case Keys.Down:
                    tgt_r--;
                    break;
            }

            if (tgt_r >= 0 && tgt_r < N
                && tgt_c >=0 && tgt_c < N)
            {
                Swap(buttons[tgt_r, tgt_c], blankBtn);
                blankBtn = buttons[tgt_r, tgt_c];
            }
        }
    }
}
