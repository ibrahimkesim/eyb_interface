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
    public partial class Form3 : Form
    {

        List<TimeClass> zamanim = new List<TimeClass>();

        internal object ahmet = new object();

        public static Form3 frm3 = new Form3();

        Functions fnc = new Functions();

        public struct Referanslar
        {
            public int max_vol;
            public int min_vol;
            public int max_beep;
            public int min_beep;
            public int Set_Value;
        }

        Referanslar referans1;
        Referanslar referans2;
        Referanslar referans3;

        public struct Alarms
        {
            public string baslangıc;
            public string bitis;
            public Referanslar ses_deger;

        }


        public Alarms[,] tablom = new Alarms[7, 50];

        public Form3()
        {
            InitializeComponent();
            frm3 = this;
        }


        private void groupBox3_Enter(object sender, EventArgs e)
        {
            
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ClearDataGrid()
        {
            zamanim.Clear();

            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < 60; j += 30)
                {
                    TimeClass mc = new TimeClass();

                    mc.Zaman = i.ToString().PadLeft(2, '0') + ":" + j.ToString().PadLeft(2, '0');

                    mc.Pazartesi    = "Default";
                    mc.Sali         = "Default";
                    mc.Carsamba     = "Default";
                    mc.Persembe     = "Default";
                    mc.Cuma         = "Default";
                    mc.Cumartesi    = "Default";
                    mc.Pazar        = "Default";

                    zamanim.Add(mc);

                }

            }
            data_Liste.DataSource = zamanim.ToList();
        }
        private void Form3_Load(object sender, EventArgs e)
        {

            ClearDataGrid();

            

            //zamanim[2].Pazar = "tuncay";
            //data_Liste.DataSource = zamanim.ToList();

            for (int i = 1; i < 8; i++ )
                data_Liste.Columns[i].DefaultCellStyle.ForeColor = Color.Gray;

            wanted_max_vol_trc.Value = 0;
            wanted_max_vol_text.Text = "0";

            wanted_min_vol_trc.Value = 0;
            wanted_min_vol_text.Text = "0";

            wanted_max_beep_trc.Value = 0;
            wanted_max_beep_text.Text = "0";

            wanted_min_beep_trc.Value = 0;
            wanted_min_beep_text.Text = "0";

            wanted_max_vol_trc1.Value = 0;
            wanted_max_vol_text1.Text = "0";

            wanted_min_vol_trc1.Value = 0;
            wanted_min_vol_text1.Text = "0";

            wanted_max_beep_trc1.Value = 0;
            wanted_max_beep_text1.Text = "0";

            wanted_min_beep_trc1.Value = 0;
            wanted_min_beep_text1.Text = "0";

            wanted_max_vol_trc2.Value = 0;
            wanted_max_vol_text2.Text = "0";

            wanted_min_vol_trc2.Value = 0;
            wanted_min_vol_text2.Text = "0";

            wanted_max_beep_trc2.Value = 0;
            wanted_max_beep_text2.Text = "0";

            wanted_min_beep_trc2.Value = 0;
            wanted_min_beep_text2.Text = "0";

            referans1.Set_Value = 1;
            referans2.Set_Value = 2;
            referans3.Set_Value = 3;

            calendar_bar.Hide();
 
            //dgv_Liste.Columns[0].HeaderText = "fndef";

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        public void calc_0_100_Limit(TrackBar tr, float step)
        {

            float Fark = tr.Value % step;
            float Mod = (tr.Value - Fark) / step;

            if (Fark >= ((float)step / 2))
            {
                tr.Value = (Int32)((Mod * step) + step);
            }
            else
            {
                if (Mod == 0)
                    tr.Value = 0;
                else
                    tr.Value = (Int32)(Mod * step);
            }
        }

        private void ref1_Click(object sender, EventArgs e)
        {
            if ((wanted_max_beep_trc.Value == 0) && (wanted_min_beep_trc.Value == 0) && (wanted_max_vol_trc.Value == 0) && (wanted_min_vol_trc.Value == 0))
                MessageBox.Show("Referans 1 değerlerini belirlemediniz!");
            else
                Change_Colour(Color.LimeGreen, "Referans 1");
        }

        private void ref2_Click(object sender, EventArgs e)
        {
            if ((wanted_max_beep_trc1.Value == 0) && (wanted_min_beep_trc1.Value == 0) && (wanted_max_vol_trc1.Value == 0) && (wanted_min_vol_trc1.Value == 0))
                MessageBox.Show("Referans 2 değerlerini belirlemediniz!");
            else
                Change_Colour(Color.Yellow, "Referans 2");
        }

        private void ref3_Click(object sender, EventArgs e)
        {
            if ((wanted_max_beep_trc2.Value == 0) && (wanted_min_beep_trc2.Value == 0) && (wanted_max_vol_trc2.Value == 0) && (wanted_min_vol_trc2.Value == 0))
                MessageBox.Show("Referans 3 değerlerini belirlemediniz!");
            else
                Change_Colour(Color.Aqua, "Referans 3");
        }

      
        private void Change_Colour(Color x, String s)
        {
            Int32 selectedCellCount = data_Liste.GetCellCount(DataGridViewElementStates.Selected);
          
            if (selectedCellCount > 0)
            {
                if (data_Liste.AreAllCellsSelected(true))
                {
                    MessageBox.Show("All cells are selected", "Selected Cells");
                }
                else
                {
                    for (int i = 0; i < selectedCellCount; i++)
                    {
                        if (data_Liste.SelectedCells[i].ColumnIndex != 0)
                        {

                            data_Liste.Rows[data_Liste.SelectedCells[i].RowIndex].Cells[data_Liste.SelectedCells[i].ColumnIndex].Style.BackColor = x;

                            switch (data_Liste.SelectedCells[i].ColumnIndex)
                            {
                                case 1:
                                    {
                                        zamanim[data_Liste.SelectedCells[i].RowIndex].Pazartesi = s;
       
                                        break;
                                    }
                                case 2:
                                    {
                                        zamanim[data_Liste.SelectedCells[i].RowIndex].Sali = s;
                                        break;
                                    }
                                case 3:
                                    {
                                        zamanim[data_Liste.SelectedCells[i].RowIndex].Carsamba = s;
                                        break;
                                    }
                                case 4:
                                    {
                                        zamanim[data_Liste.SelectedCells[i].RowIndex].Persembe = s;
                                        break;
                                    }
                                case 5:
                                    {
                                        zamanim[data_Liste.SelectedCells[i].RowIndex].Cuma = s;
                                        break;
                                    }
                                case 6:
                                    {
                                        zamanim[data_Liste.SelectedCells[i].RowIndex].Cumartesi = s;
                                        break;
                                    }
                                case 7:
                                    {
                                        zamanim[data_Liste.SelectedCells[i].RowIndex].Pazar = s;
                                        break;
                                    }
                            }
                            
                        }
                            
                    }
                    
                    //data_Liste.DataSource = zamanim.ToList();


                    data_Liste.ClearSelection();
                }
            }
        }

        private void clr_Click(object sender, EventArgs e)
        {
            zamanim.Clear();

            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < 60; j += 30)
                {
                    TimeClass mc = new TimeClass();

                    mc.Zaman = i.ToString().PadLeft(2, '0') + ":" + j.ToString().PadLeft(2, '0');

                    mc.Pazartesi    = "Default";
                    mc.Sali         = "Default";
                    mc.Carsamba     = "Default";
                    mc.Persembe     = "Default";
                    mc.Cuma         = "Default";
                    mc.Cumartesi    = "Default";
                    mc.Pazar        = "Default";

                    zamanim.Add(mc);

                }

            }
            data_Liste.DataSource = zamanim.ToList();

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 48; j++)
                {
                    tablom[i, j].baslangıc = null;
                    tablom[i, j].bitis = null;
                    tablom[i, j].ses_deger.max_beep = 0;
                    tablom[i, j].ses_deger.min_beep = 0;
                    tablom[i, j].ses_deger.max_vol = 0;
                    tablom[i, j].ses_deger.min_vol = 0;
                }
            }

            Form1.form1.button5.Enabled = false;
            //data_Liste.DataSource = zamanim_yedek.ToList();
        }

        private void kaydetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String[] Send_String_Data = new string[100];

            int status = 0;

            int count = 0;
            
            for(int i = 1; i < 8; i++ )
            {
                for (int j = 0; j < 48; j++)
                {
                    if(data_Liste.Rows[j].Cells[i].Value == "Referans 1")
                    {
                        if ((status == 2) || (status == 3))
                        {

                            tablom[i - 1, count++].bitis = data_Liste.Rows[j].Cells[0].Value.ToString();

                            tablom[i - 1, count].baslangıc = data_Liste.Rows[j].Cells[0].Value.ToString();

                            tablom[i - 1, count].ses_deger = referans1;

                            status = 1;
                        }
                        else if (status == 0)
                        {
                            tablom[i - 1, count].baslangıc = data_Liste.Rows[j].Cells[0].Value.ToString();

                            tablom[i - 1, count].ses_deger = referans1;

                            status = 1;
                        }
                        else if((status == 1) && (j == 47))
                        {
                            tablom[i - 1, count++].bitis = data_Liste.Rows[0].Cells[0].Value.ToString();
                            status = 0;
                        }
                    }
                    else if(data_Liste.Rows[j].Cells[i].Value == "Referans 2")
                    {
                        if ((status == 1) || (status == 3))
                        {
                            tablom[i - 1, count++].bitis = data_Liste.Rows[j].Cells[0].Value.ToString();

                            tablom[i - 1, count].baslangıc = data_Liste.Rows[j].Cells[0].Value.ToString();

                            tablom[i - 1, count].ses_deger = referans2;

                            status = 2;
                        }
                        else if (status == 0)
                        {
                            tablom[i - 1, count].baslangıc = data_Liste.Rows[j].Cells[0].Value.ToString();

                            tablom[i - 1, count].ses_deger = referans2;

                            status = 2;
                        }
                        else if ((status == 2) && (j == 47))
                        {
                            tablom[i - 1, count++].bitis = data_Liste.Rows[0].Cells[0].Value.ToString();
                            status = 0;
                        }
                    }
                    else if(data_Liste.Rows[j].Cells[i].Value == "Referans 3")
                    {
                        if ((status == 2) || (status == 1))
                        {
                            tablom[i - 1, count++].bitis = data_Liste.Rows[j].Cells[0].Value.ToString();

                            tablom[i - 1, count].baslangıc = data_Liste.Rows[j].Cells[0].Value.ToString();

                            tablom[i - 1, count].ses_deger = referans3;

                            status = 3;
                        }
                        else if (status == 0)
                        {
                            tablom[i - 1, count].baslangıc = data_Liste.Rows[j].Cells[0].Value.ToString();

                            tablom[i - 1, count].ses_deger = referans3;

                            status = 3;
                        }
                        else if ((status == 3) && (j == 47))
                        {
                            tablom[i - 1 , count++].bitis = data_Liste.Rows[0].Cells[0].Value.ToString();
                            status = 0;
                        }
                    }
                    else
                    {
                        if ((status == 1) || (status == 2) || (status == 3))
                        {
                            tablom[i - 1, count++].bitis = data_Liste.Rows[j].Cells[0].Value.ToString();
                            status = 0;
                        }
                        
                    }
                }

                count = 0;
                status = 0;
            }

            Form1.form1.button5.Enabled = true;
            this.Close();
        }

        private void wanted_max_vol_trc_Scroll(object sender, EventArgs e)
        {
            TrackBar Trcbar = (TrackBar)sender;

            switch(Trcbar.Name)
            {
                case "wanted_max_vol_trc":
                    calc_0_100_Limit(Trcbar, 4);

                    wanted_max_vol_text.Text = Trcbar.Value.ToString();

                    break;

                case "wanted_max_beep_trc":
                    calc_0_100_Limit(Trcbar, 4);

                    wanted_max_beep_text.Text = Trcbar.Value.ToString();

                    break;

                case "wanted_min_vol_trc":
                    calc_0_100_Limit(Trcbar, 4);

                    wanted_min_vol_text.Text = Trcbar.Value.ToString();

                    break;

                case "wanted_min_beep_trc":
                    calc_0_100_Limit(Trcbar, 4);

                    wanted_min_beep_text.Text = Trcbar.Value.ToString();

                    break;
                case "wanted_max_vol_trc1":
                    calc_0_100_Limit(Trcbar, 4);

                    wanted_max_vol_text1.Text = Trcbar.Value.ToString();

                    break;

                case "wanted_max_beep_trc1":
                    calc_0_100_Limit(Trcbar, 4);

                    wanted_max_beep_text1.Text = Trcbar.Value.ToString();

                    break;

                case "wanted_min_vol_trc1":
                    calc_0_100_Limit(Trcbar, 4);

                    wanted_min_vol_text1.Text = Trcbar.Value.ToString();

                    break;

                case "wanted_min_beep_trc1":
                    calc_0_100_Limit(Trcbar, 4);

                    wanted_min_beep_text1.Text = Trcbar.Value.ToString();

                    break;
                case "wanted_max_vol_trc2":
                    calc_0_100_Limit(Trcbar, 4);

                    wanted_max_vol_text2.Text = Trcbar.Value.ToString();

                    break;

                case "wanted_max_beep_trc2":
                    calc_0_100_Limit(Trcbar, 4);

                    wanted_max_beep_text2.Text = Trcbar.Value.ToString();

                    break;

                case "wanted_min_vol_trc2":
                    calc_0_100_Limit(Trcbar, 4);

                    wanted_min_vol_text2.Text = Trcbar.Value.ToString();

                    break;

                case "wanted_min_beep_trc2":
                    calc_0_100_Limit(Trcbar, 4);

                    wanted_min_beep_text2.Text = Trcbar.Value.ToString();

                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            referans1.max_vol = wanted_max_vol_trc.Value / 4;
            referans1.min_vol = wanted_min_vol_trc.Value / 4;
            referans1.max_beep = wanted_max_beep_trc.Value / 4;
            referans1.min_beep = wanted_min_beep_trc.Value / 4;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            referans2.max_vol = wanted_max_vol_trc1.Value / 4;
            referans2.min_vol = wanted_min_vol_trc1.Value / 4;
            referans2.max_beep = wanted_max_beep_trc1.Value / 4;
            referans2.min_beep = wanted_min_beep_trc1.Value / 4;
 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            referans3.max_vol = wanted_max_vol_trc2.Value / 4;
            referans3.min_vol = wanted_min_vol_trc2.Value / 4;
            referans3.max_beep = wanted_max_beep_trc2.Value / 4;
            referans3.min_beep = wanted_min_beep_trc2.Value / 4;
        }

        private void SelectDayString(int day, int row, string s)
        {
            switch(day)
            {
                case 1:
                    zamanim[row].Pazartesi = s;
                    break;
                case 2:
                    zamanim[row].Sali = s;
                    break;
                case 3:
                    zamanim[row].Carsamba = s;
                    break;
                case 4:
                    zamanim[row].Persembe = s;
                    break;
                case 5:
                    zamanim[row].Cuma = s;
                    break;
                case 6:
                    zamanim[row].Cumartesi = s;
                    break;
                case 7:
                    zamanim[row].Pazar = s;
                    break;
            }
        }
        private void ProccesReadCalendar(int day)
        {
            int temp;
            int j = 0;

            for (int i = 3; i < 50; i += 2)
            {
 
                temp = Convert.ToInt32((Form1.form1.CalendarString[i].ToString() + Form1.form1.CalendarString[i + 1].ToString()), 16);


                if ((temp & 0x80) == 0x80)
                {
                    switch ((temp & 0x38) >> 3)
                    {
                        case 1:
                            {
                                data_Liste.Rows[j].Cells[day].Style.BackColor = Color.LimeGreen;

                                SelectDayString(day, j, "Referans1");
                                break;
                            }
                        case 2:
                            {
                                data_Liste.Rows[j].Cells[day].Style.BackColor = Color.Yellow;
                                SelectDayString(day, j, "Referans2");
                                break;
                            }
                        case 3:
                            {
                                data_Liste.Rows[j].Cells[day].Style.BackColor = Color.Aqua;
                                SelectDayString(day, j, "Referans3");
                                break;
                            }

                    }

                }

                j++;

                if ((temp & 0x40) == 0x40)
                {
                    switch (temp & 0x07)
                    {
                        case 1:
                            {
                                data_Liste.Rows[j].Cells[day].Style.BackColor = Color.LimeGreen;
                                SelectDayString(day, j, "Referans1");
                                break;
                            }
                        case 2:
                            {
                                data_Liste.Rows[j].Cells[day].Style.BackColor = Color.Yellow;
                                SelectDayString(day, j, "Referans2");
                                break;
                            }
                        case 3:
                            {
                                data_Liste.Rows[j].Cells[day].Style.BackColor = Color.Aqua;
                                SelectDayString(day, j, "Referans3");
                                break;
                            }

                    }
                }

                j++;
            }
        }
        private void kayıtlıAyarlarıOkuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (frm3.ahmet)
            {



                int i = 1, j;

                ClearDataGrid();

                calendar_bar.Show();

                calendar_bar.Value = 0;

                while (i < 8)
                {
                    j = 0;

                    while (j < 3)
                    {
                        if (Form1.form1.Send_Message_With_Timeout(i + 15, 0) > 0)
                        {
                            //MessageBox.Show("ok mesaj");
                            break;
                        }


                        //MessageBox.Show("hata mesaj");
                        j++;
                    }



                    if (j < 3)
                    {
                        calendar_bar.Increment(1);
                        ProccesReadCalendar(i); // günler;

                        Form1.form1.Message_Received[i + 15] = false;
                    }
                    else
                    {
                        //MessageBox.Show(i.ToString() + ". mesaj");
                        calendar_bar.Hide();
                        return;
                    }

                    i++;


                }

                if (i == 8)
                {
                    j = 0;

                    while (j < 3)
                    {
                        if (Form1.form1.Send_Message_With_Timeout(i + 15, 0) > 0)
                        {
                            //MessageBox.Show("son ok mesaj");
                            break;
                        }


                        //MessageBox.Show("son hata mesaj");
                        j++;
                    }

                    if (j < 3)
                    {

                        wanted_max_vol_trc.Value = (Convert.ToInt32((Form1.form1.CalendarString[3].ToString() + Form1.form1.CalendarString[4].ToString()), 16)) * 4;
                        wanted_max_vol_text.Text = wanted_max_vol_trc.Value.ToString();

                        wanted_min_vol_trc.Value = (Convert.ToInt32((Form1.form1.CalendarString[5].ToString() + Form1.form1.CalendarString[6].ToString()), 16)) * 4;
                        wanted_min_vol_text.Text = wanted_min_vol_trc.Value.ToString();

                        wanted_max_beep_trc.Value = (Convert.ToInt32((Form1.form1.CalendarString[7].ToString() + Form1.form1.CalendarString[8].ToString()), 16)) * 4;
                        wanted_max_beep_text.Text = wanted_max_beep_trc.Value.ToString();

                        wanted_min_beep_trc.Value = (Convert.ToInt32((Form1.form1.CalendarString[9].ToString() + Form1.form1.CalendarString[10].ToString()), 16)) * 4;
                        wanted_min_beep_text.Text = wanted_min_beep_trc.Value.ToString();



                        wanted_max_vol_trc1.Value = (Convert.ToInt32((Form1.form1.CalendarString[11].ToString() + Form1.form1.CalendarString[12].ToString()), 16)) * 4;
                        wanted_max_vol_text1.Text = wanted_max_vol_trc1.Value.ToString();

                        wanted_min_vol_trc1.Value = (Convert.ToInt32((Form1.form1.CalendarString[13].ToString() + Form1.form1.CalendarString[14].ToString()), 16)) * 4;
                        wanted_min_vol_text1.Text = wanted_min_vol_trc1.Value.ToString();

                        wanted_max_beep_trc1.Value = (Convert.ToInt32((Form1.form1.CalendarString[15].ToString() + Form1.form1.CalendarString[16].ToString()), 16)) * 4;
                        wanted_max_beep_text1.Text = wanted_max_beep_trc1.Value.ToString();

                        wanted_min_beep_trc1.Value = (Convert.ToInt32((Form1.form1.CalendarString[17].ToString() + Form1.form1.CalendarString[18].ToString()), 16)) * 4;
                        wanted_min_beep_text1.Text = wanted_min_beep_trc1.Value.ToString();



                        wanted_max_vol_trc2.Value = (Convert.ToInt32((Form1.form1.CalendarString[19].ToString() + Form1.form1.CalendarString[20].ToString()), 16)) * 4;
                        wanted_max_vol_text2.Text = wanted_max_vol_trc2.Value.ToString();

                        wanted_min_vol_trc2.Value = (Convert.ToInt32((Form1.form1.CalendarString[21].ToString() + Form1.form1.CalendarString[22].ToString()), 16)) * 4;
                        wanted_min_vol_text2.Text = wanted_min_vol_trc2.Value.ToString();

                        wanted_max_beep_trc2.Value = (Convert.ToInt32((Form1.form1.CalendarString[23].ToString() + Form1.form1.CalendarString[24].ToString()), 16)) * 4;
                        wanted_max_beep_text2.Text = wanted_max_beep_trc2.Value.ToString();

                        wanted_min_beep_trc2.Value = (Convert.ToInt32((Form1.form1.CalendarString[25].ToString() + Form1.form1.CalendarString[26].ToString()), 16)) * 4;
                        wanted_min_beep_text2.Text = wanted_min_beep_trc2.Value.ToString();

                        MessageBox.Show("Okuma Tamamlandı.");
                        calendar_bar.Hide();
                    }
                    else
                    {
                        //MessageBox.Show(i.ToString() + ". hata mesaj");
                        MessageBox.Show("Okuma Tamamlanamadı Hata!");
                        calendar_bar.Hide();
                        return;
                    }


                }

            }
            
        }

        private void data_Liste_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }


         
    }
}
