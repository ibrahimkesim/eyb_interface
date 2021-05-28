using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yaya_butonu_test
{
    

    public partial class Form2 : Form
    {
        int data;

        Form1 form1;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            form1 = new Form1();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            data++;

            if (data == 20)
            {
                timer1.Stop();
                timerOut.Enabled = true;
                //Form1 frm1 = new Form1();
                //frm1.Show();
            }
        }

        private void timerIn_Tick(object sender, EventArgs e)
        {
            this.Opacity += 0.02;

            if (this.Opacity == 1)
            {
                timerIn.Stop();

                timer1.Enabled = true;
            }
        }

        private void timerOut_Tick(object sender, EventArgs e)
        {
            this.Opacity -= 0.02;

            if (this.Opacity == 0)
            {
                timerOut.Stop();

                Hide();
                form1.ShowDialog();
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
