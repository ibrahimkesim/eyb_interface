using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;


namespace yaya_butonu_test
{
 

    public partial class Form1 : Form
    {
        //SerialPort sp;
        Boolean seri_port_flag = false;
        Boolean Serial_Event = false;
        string Alınan_Data;
        Int32 Read_Byte;

        List<files> myfileList = new List<files>();

        DateTime currentTime;

        public string CalendarString;

        public Boolean weekcalendarsend = false;

        string[,] Message;
        Int32[] Temp_Value;
        Int32[] Vib_Temp;
        public Boolean[] Message_Received;
        Int32 Send_Message;
        Int32 Send_Message_Index;
        Int32 Device_Version;
        Int32 Sended_Data;
        Int32 Sended_Data_1;
        Thread Sorgu_thread;
        Boolean timeout_exception = false;
        string pow;
        string time;
        string year, month, day, hour, minute, day_of_week;

        byte[] vaw_read;
        byte[] firmware_read;
        FileStream Vaw_Array;
        FileStream Firmware_Array;

        int[] fileIndex;

        Boolean File_Opened = false;

        Boolean Vaw_send = false;
        Boolean Ack_received = false;
        Boolean file_sending = false;
        Boolean vaw_send_timeout = false;

        OpenFileDialog vaw;


        System.Timers.Timer timeout;
        System.Timers.Timer timeout_file;

        public static Form1 form1 = new Form1();
        //Functions fnc = new Functions();

        public Form1()
        {       
            InitializeComponent();

            form1 = this;
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        public void gecikme(int saniye)
        { 
            saniye = (saniye + Convert.ToInt32(DateTime.Now.Second) % 60);
            for (;;)
            {
                if (saniye <= DateTime.Now.Second) 
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Thread thread = new Thread(delegate ()
            {
                baglan();
            });
            thread.Start();
        }

        public void baglan()
        {
            int counter = 0;
            //label27.Text = "Button Clicked";

            try
            {
                if (!sp.IsOpen)
                {
                    sp.PortName = comboBox1.SelectedItem.ToString();

                    sp.NewLine = "\r\n";
                    sp.Open();

                    button2.Enabled = false;
                    button9.Enabled = true;
                    button1.Enabled = true;
                    //button2.BackColor = Color.LightGray;
                    button9.BackColor = Color.LightGray;

                    label27.Text = "Cihaz Versiyonu: -";

                    //SerialSend("Selamun_Aleykum_Ben_Geldim\r\n");

                    Thread.Sleep(500);

                    Message_Received[14] = false;

                    while (true)
                    {
                        Serial_Event = true;
                        sp.Write("$VERSION?\r\n");
                        Send_Message = 14;
                        Send_Message_Index = 0;

                        //Send_Message_With_Timeout(14, 0);

                        if (Message_Received[14])
                        {
                            seri_port_flag = true;
                            toolStripStatusLabel1.Text = "Bağlandı";
                            toolStripStatusLabel1.ForeColor = Color.Green;
                            toolStripStatusLabel1.BackColor = Color.SeaShell;
                            comboBox1.Enabled = false;

                            break;

                        }
                        else
                        {
                            counter++;
                        }

                        if (counter > 5)
                        {
                            toolStripStatusLabel1.ForeColor = Color.Red;
                            toolStripStatusLabel1.Text = "Versiyon Bilgisi Alınamadı!";

                            break;
                        }

                        Thread.Sleep(500);

                    }


                }

                else
                {
                    toolStripStatusLabel1.Text = "Cihaz Zaten Bağlı!";
                    toolStripStatusLabel1.ForeColor = Color.Red;

                }
            }
            catch (Exception)
            {
                if (comboBox1.SelectedItem == null)
                {
                    toolStripStatusLabel1.Text = "Haberleşme Ayarlarını Yapınız!";
                    toolStripStatusLabel1.ForeColor = Color.Red;
                }

                else
                {
                    toolStripStatusLabel1.Text = "Bağlantı Hatası!";
                    toolStripStatusLabel1.ForeColor = Color.Red;
                    //button2.BackColor = SystemColors.ButtonFace;
                }
            }
        }
            
            
      
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            //toolStripStatusLabel1.BackColor = Color.SeaShell;
            button9.Enabled = false;
            button1.Enabled = false;

            toolStripStatusLabel1.BackColor = Color.SeaShell;

            toolStripStatusLabel1.Text = "Bağlantı Ayarlarını Yapınız";
            toolStripStatusLabel1.ForeColor = Color.Green;

            label27.ForeColor = Color.Black;
            label27.BackColor = Color.SeaShell;
            label27.Text = "";

            Message = new string[25, 4]     {   
                                            { "$VOLMAX?\r\n", "$VOLMAX:", "$OK", "8" }, 
                                            { "$VOLMIN?\r\n", "$VOLMIN:", "$OK", "8" }, 
                                            { "$VOICDELAY?\r\n", "$VOICDELAY:", "$OK", "11" },
                                            { "$BUTTONTIME?\r\n", "$BUTTONTIME:", "$OK", "12" }, 
                                            { "$SENS?\r\n", "$SENS:", "$OK", "6"},
                                            { "$BEEPMAX?\r\n", "$BEEPMAX:", "$OK", "9"},
                                            { "$BEEPMIN?\r\n", "$BEEPMIN:", "$OK", "9"},
                                            { "$BEEPDELAY?\r\n", "$BEEPDELAY:", "$OK", "11"},
                                            { "$VIB?\r\n", "$VIB:", "$OK", "5"},
                                            { "$VIBGREEN?\r\n", "$VIBGREEN:", "$OK", "10"},
                                            { "$RTC?\r\n", "$RTC:", "$OK", "5"},
                                            { "", "$TIME:", "$OK", "3"},
                                            { "", "$SEND:", "$OK", "3"},
                                            { "$RST:EYB\r\n", "$OK", "","3"},
                                            { "$VERSION?\r\n", "$VERSION:", "", "9"},
                                            { "$CLREEPROM\r\n", "$OK", "", "3"},
                                            { "$DAY1?\r\n", "$OK", "","3"},
                                            { "$DAY2?\r\n", "$OK", "","3"},
                                            { "$DAY3?\r\n", "$OK", "","3"},
                                            { "$DAY4?\r\n", "$OK", "","3"},
                                            { "$DAY5?\r\n", "$OK", "","3"},
                                            { "$DAY6?\r\n", "$OK", "","3"},
                                            { "$DAY7?\r\n", "$OK", "","3"},
                                            { "$PRESETS?\r\n", "$OK", "","3"},
                                            { "$REQUEST:", "$OK", "","3"}
                                            };

            //Vaw_Array = new FileStream();

            

            Temp_Value = new int[14];
            Message_Received = new Boolean[25];

            timeout = new System.Timers.Timer();
            timeout_file = new System.Timers.Timer();
            
            Vib_Temp = new Int32[2];

            timeout_file.Enabled = false;
            timeout_file.Interval = 2000;
            timeout_file.Elapsed += timeout_file_Elapsed;

            timeout.Enabled = false;
            timeout.Interval = 2000;
            timeout.Elapsed += timeout_Elapsed;
            //toolStripStatusLabel1.Text = "";
            textBox3.PasswordChar = '*';
            textBox2.MaxLength = 10;
            textBox3.MaxLength = 6;

            button5.Enabled = false;

        }

        void timeout_file_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            vaw_send_timeout = true;
            timeout_file.Enabled = false;
        }

        void timeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timeout_exception = true;
            Serial_Event = false;
            timeout.Enabled = false;
        }
      

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (Serial_Event && !Vaw_send)
            {
                string alinan = sp.ReadLine();

                //char[] alınan = new char[sp.BytesToRead];

                //textBox4.Text = alinan;

                Thread thread = new Thread(delegate()
                {
                    SerialDataProccess(alinan, alinan.Length);
                });
                thread.Start();

                
            }
            
            if (Vaw_send)
            {
                byte[] ACK = new byte[sp.BytesToRead];
                sp.Read(ACK, 0, ACK.Length);

                if (ACK[ACK.Length - 1] == 6)
                    Ack_received = true; 
            }
            else
                sp.DiscardInBuffer();

        }

        internal static object ahmet = new object();

        public void SerialDataProccess(string Data, Int32 Byte)
        {
            lock (ahmet)
            {




                Alınan_Data = Data;
                Read_Byte = Byte;

                //MessageBox.Show(Send_Message.ToString() + ". Send Mesage /" + Send_Message_Index.ToString() + ". Index /");
                if (string.Compare(Alınan_Data, 0, Message[Send_Message, Send_Message_Index + 1], 0, Convert.ToInt32(Message[Send_Message, 3])) == 0)
                //  && Alınan_Data[Read_Byte - 2] == '\r' && Alınan_Data[Read_Byte - 1] == '\n')
                {
                    switch (Send_Message)
                    {
                        case 0:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[0] = true;
                                Temp_Value[0] = Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 4;
                            }
                            else if (Send_Message_Index == 1)
                            {
                                Message_Received[0] = true;
                                Temp_Value[0] = Sended_Data;
                            }
                            break;
                        case 1:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[1] = true;
                                Temp_Value[1] = Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 4;
                            }
                            else if (Send_Message_Index == 1)
                            {
                                Message_Received[1] = true;
                                Temp_Value[1] = Sended_Data;
                            }
                            break;
                        case 2:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[2] = true;

                                if (Device_Version == 0)
                                    Temp_Value[2] = (Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) / 50);
                                else
                                    Temp_Value[2] = (Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 10);

                                /*
                                 * if (Device_Version == 0)
                                    repeat_text.Text = ((repeat_trc.Value = Temp_Value[2] = (Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) / 50)) / (float)20).ToString();
                                else
                                    repeat_text.Text = ((repeat_trc.Value = Temp_Value[2] = (Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 10)) / (float)20).ToString();
                                 */
                            }
                            else if (Send_Message_Index == 1)
                            {
                                Message_Received[2] = true;
                                Temp_Value[2] = Sended_Data;
                            }
                            break;
                        case 3:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[3] = true;

                                if (Device_Version == 0)
                                    Temp_Value[3] = (Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) / 50);
                                else
                                    Temp_Value[3] = (Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 10);

                                /* 
                                if (Device_Version == 0)
                                    location_delay_push_text.Text = ((location_delay_push_trc.Value = Temp_Value[3] = (Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) / 50)) / (float)20).ToString();
                                else
                                    location_delay_push_text.Text = ((location_delay_push_trc.Value = Temp_Value[3]  = (Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 10)) / (float)20).ToString();
                                 */
                            }
                            else if (Send_Message_Index == 1)
                            {
                                Message_Received[3] = true;
                                Temp_Value[3] = Sended_Data;
                            }
                            break;
                        case 4:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[4] = true;
                                Temp_Value[4] = Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 5;
                            }
                            else if (Send_Message_Index == 1)
                            {
                                Message_Received[4] = true;
                                Temp_Value[4] = Sended_Data;
                            }
                            break;
                        case 5:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[5] = true;
                                Temp_Value[5] = Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 4;
                            }
                            else if (Send_Message_Index == 1)
                            {
                                Message_Received[5] = true;
                                Temp_Value[5] = Sended_Data;
                            }
                            break;

                        case 6:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[6] = true;
                                Temp_Value[6] = Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 4;
                            }
                            else if (Send_Message_Index == 1)
                            {
                                Message_Received[6] = true;
                                Temp_Value[6] = Sended_Data;
                            }
                            break;

                        case 7:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[7] = true;

                                if (Device_Version == 0)
                                    Temp_Value[7] = (Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) / 50);
                                else
                                    Temp_Value[7] = (Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 10);

                                /*
                                 * if (Device_Version == 0)
                                    beep_repeat_text.Text = ((beep_repeat_trc.Value = Temp_Value[7] = (Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) / 50)) / (float)20).ToString();
                                else
                                    beep_repeat_text.Text = ((beep_repeat_trc.Value = Temp_Value[7] = (Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 10)) / (float)20).ToString();
                                 */
                            }
                            else if (Send_Message_Index == 1)
                            {
                                Message_Received[7] = true;
                                Temp_Value[7] = Sended_Data;
                            }
                            break;
                        case 8:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[8] = true;
                                Int32 j = Convert.ToInt32(Message[Send_Message, 3]);
                                pow = "";
                                time = "";
                                if (Alınan_Data[j] != ':')
                                {
                                    pow += Alınan_Data[j++].ToString();
                                }
                                j++;
                                if (Alınan_Data[j] != ':')
                                {
                                    time += Alınan_Data[j++].ToString();
                                }
                                Vib_Temp[0] = (((Convert.ToInt32(pow) - 5) * -1) + 5) * 10;
                                //vib_power.Text = (vib_power_trc.Value = Vib_Temp[0] = (((Convert.ToInt32(pow) - 5) * -1) + 5) * 10).ToString();

                                Vib_Temp[1] = Convert.ToInt32(time) * 10;

                                /*
                                 * 
                                 * if (Device_Version == 0)
                                    vib_time_trc.Value = Vib_Temp[1] = Convert.ToInt32(vib_time.Text = (Convert.ToInt32(time) / 100).ToString()) * 10;
                                else
                                    vib_time_trc.Value = Vib_Temp[1] = Convert.ToInt32(vib_time.Text = time) * 10;
                                 */
                            }
                            else if (Send_Message_Index == 1)
                            {
                                Message_Received[8] = true;
                                Vib_Temp[0] = Sended_Data;
                                Vib_Temp[1] = Sended_Data_1;
                            }
                            break;
                        case 9:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[9] = true;

                                if (Device_Version == 0)
                                    Temp_Value[9] = Convert.ToInt32(Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) / 25);
                                else
                                    Temp_Value[9] = Convert.ToInt32(Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 4);

                                /*
                                 * if (Device_Version == 0)
                                    green_delay.Text = ((green_delay_trc.Value = Temp_Value[9] = Convert.ToInt32(Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) / 25)) / 40).ToString();
                                else */
                                green_delay.Text = ((green_delay_trc.Value = Temp_Value[9] = Convert.ToInt32(Convert.ToInt32(Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])))) * 4)) / 40).ToString();

                            }
                            else if (Send_Message_Index == 1)
                            {
                                Message_Received[9] = true;
                                Temp_Value[9] = Sended_Data;
                            }
                            break;

                        case 10:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[10] = true;

                                Int32 k = Convert.ToInt32(Message[Send_Message, 3]);

                                year_.Text = ((Convert.ToInt32(Alınan_Data[5].ToString() + Alınan_Data[6].ToString())) + 2000).ToString();
                                month_.Text = (Convert.ToInt32(Alınan_Data[7].ToString() + Alınan_Data[8].ToString())).ToString().PadLeft(2,'0');
                                day_.Text = (Convert.ToInt32(Alınan_Data[9].ToString() + Alınan_Data[10].ToString())).ToString().PadLeft(2, '0');
                                hour_.Text = (Convert.ToInt32(Alınan_Data[14].ToString() + Alınan_Data[15].ToString())).ToString().PadLeft(2, '0');
                                minute_.Text = (Convert.ToInt32(Alınan_Data[16].ToString() + Alınan_Data[17].ToString())).ToString().PadLeft(2, '0');

                                string weekdyStr = (Convert.ToInt32(day_of_week = Alınan_Data[12].ToString())).ToString();

                                switch (weekdyStr)
                                {
                                    case "1":
                                        week_day_.Text = "Pazartesi";
                                        break;
                                    case "2":
                                        week_day_.Text = "Salı";
                                        break;
                                    case "3":
                                        week_day_.Text = "Çarşamba";
                                        break;
                                    case "4":
                                        week_day_.Text = "Perşembe";
                                        break;
                                    case "5":
                                        week_day_.Text = "Cuma";
                                        break;
                                    case "6":
                                        week_day_.Text = "Cumartesi";
                                        break;
                                    case "7":
                                        week_day_.Text = "Pazar";
                                        break;

                                }
                            }
                            else if (Send_Message_Index == 1)
                            {
                                Message_Received[10] = true;
                            }
                            break;

                        case 11:
                            if (Send_Message_Index == 1)
                                Message_Received[11] = true;
                            break;

                        case 12:
                            if (Send_Message_Index == 1)
                            {
                                Message_Received[12] = true;

                            }
                            break;

                        case 13:
                            if (Send_Message_Index == 0)
                                Message_Received[13] = true;
                            break;


                        case 14:
                            Message_Received[14] = true;
                            label27.Text = "Cihaz Versiyonu: " + Alınan_Data.Substring(Convert.ToInt32(Message[Send_Message, 3]), (Read_Byte - Convert.ToInt32(Message[Send_Message, 3])));


                            Device_Version = 1;

                            if (label27.Text == "Cihaz Versiyonu: 2.10")
                            {
                                Device_Version = 2;
                                checkBox1.Enabled = true;
                            }
                            else if (label27.Text == "Cihaz Versiyonu: 2.11" || label27.Text == "Cihaz Versiyonu: 2.12" || label27.Text == "Cihaz Versiyonu: 2.13")
                            {
                                Device_Version = 3;
                                checkBox1.Enabled = true;
                            }
                            else if (label27.Text == "Cihaz Versiyonu: 2.14" || label27.Text == "Cihaz Versiyonu: 2.15" || label27.Text == "Cihaz Versiyonu: 2.16" || label27.Text == "Cihaz Versiyonu: 2.17" || label27.Text == "Cihaz Versiyonu: 2.18"
                                || label27.Text == "Cihaz Versiyonu: 2.19" || label27.Text == "Cihaz Versiyonu: 2.20")
                            {
                                Device_Version = 4;
                                checkBox1.Enabled = true;
                            }

                            break;

                        case 15:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[15] = true;
                            }
                            break;

                        case 16:
                            if (Send_Message_Index == 0)
                            {
                                CalendarString = Alınan_Data;
                                Message_Received[16] = true;
                            }

                            break;
                        case 17:
                            if (Send_Message_Index == 0)
                            {
                                CalendarString = Alınan_Data;
                                Message_Received[17] = true;
                            }
                            break;
                        case 18:
                            if (Send_Message_Index == 0)
                            {
                                CalendarString = Alınan_Data;
                                Message_Received[18] = true;
                            }
                            break;
                        case 19:
                            if (Send_Message_Index == 0)
                            {
                                CalendarString = Alınan_Data;
                                Message_Received[19] = true;
                            }
                            break;
                        case 20:
                            if (Send_Message_Index == 0)
                            {
                                CalendarString = Alınan_Data;
                                Message_Received[20] = true;
                            }
                            break;
                        case 21:
                            if (Send_Message_Index == 0)
                            {
                                CalendarString = Alınan_Data;
                                Message_Received[21] = true;
                            }
                            break;
                        case 22:
                            if (Send_Message_Index == 0)
                            {
                                CalendarString = Alınan_Data;
                                Message_Received[22] = true;
                            }
                            break;
                        case 23:
                            if (Send_Message_Index == 0)
                            {
                                CalendarString = Alınan_Data;
                                Message_Received[23] = true;
                            }
                            break;
                        case 24:
                            if (Send_Message_Index == 0)
                            {
                                CalendarString = Alınan_Data;
                                Message_Received[24] = true;
                            }
                            break;
                        case 25:
                            if (Send_Message_Index == 0)
                            {
                                Message_Received[24] = true;
                            }
                            break;

                    }

                    Alınan_Data = "";
                    Read_Byte = 0;

                    try
                    {
                        sp.DiscardInBuffer();
                    }
                    catch
                    {

                    }

                    Serial_Event = false;
                }
                else
                {
                    if (Read_Byte >= 30)
                    {
                        Alınan_Data = "";
                        Read_Byte = 0;
                        sp.DiscardInBuffer();
                    }

                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            label27.Text = "";
            
            try
            {
                if (sp.IsOpen)
                {
                    sp.Close();

                    seri_port_flag = false;

                    button2.BackColor = SystemColors.ButtonFace;

                    button9.BackColor = SystemColors.ButtonFace;

                    button9.Enabled = false;

                    button2.Enabled = true;

                    button1.Enabled = false;

                    toolStripStatusLabel1.Text = "Bağlantı Ayarlarını Yapınız";
                    toolStripStatusLabel1.ForeColor = Color.Green;

                    comboBox1.Enabled = true;


                }
            }
            catch (Exception)
            {
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Bağlantı Hatası";

            }
        
        }
    
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
      

        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!seri_port_flag || file_sending)
            {
                foreach (TabPage ctl in tabControl1.TabPages)
                {
                    if (e.TabPage == ctl)
                    {
                        e.Cancel = true;
                    }
                }
            }
            else
            {
                switch (tabControl1.SelectedIndex)
                {
                    case 1:
                        button10.Enabled = true;

                        //Sorgu_thread = new Thread(delegate()
                        //{
                        //    Query_Data();
                        //});
                        //Sorgu_thread.Start();
                        break;
                    case 2:
                        //Thread Sorgu_thread_1 = new Thread(delegate()
                        //{
                        //    Query_Data_1();
                        //});
                        //Sorgu_thread_1.Start();

                        button12.Enabled = true;
                        break;
                    case 3:
                        button4.Enabled = true;
            
                        //Thread Sorgu_thread_2 = new Thread(delegate()
                        //{
                        //    Query_Data_2();
                        //});
                        //Sorgu_thread_2.Start();
                        break;
                    


                }

            }
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

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";

            TrackBar Trcbar = (TrackBar)sender;

            

            switch(Trcbar.Name)
            {
                //case "wanted_max_vol_trc":
                //    calc_0_100_Limit(Trcbar, 4);

                //    wanted_max_vol_text.Text = Trcbar.Value.ToString();

                //    break;

                //case "wanted_max_beep_trc":
                //    calc_0_100_Limit(Trcbar, 4);

                //    wanted_max_beep_text.Text = Trcbar.Value.ToString();

                //    break;

                //case "wanted_min_vol_trc":
                //    calc_0_100_Limit(Trcbar, 4);

                //    wanted_min_vol_text.Text = Trcbar.Value.ToString();

                //    break;

                //case "wanted_min_beep_trc":
                //    calc_0_100_Limit(Trcbar, 4);

                //    wanted_min_beep_text.Text = Trcbar.Value.ToString();

                //    break;

                case "mak_ses_trc":
                    calc_0_100_Limit(Trcbar, 4);
                    if (Trcbar.Value <= min_ses_trc.Value)
                    {
                        toolStripStatusLabel1.Text = "Maksimum Ses Seviyesi Daha Küçük Olamaz!";
                        toolStripStatusLabel1.ForeColor = Color.Red;
                        Trcbar.Value = min_ses_trc.Value + 4;
                    }
                    mak_ses_text.Text = Trcbar.Value.ToString();
                    break;
                case "min_ses_trc":
                    calc_0_100_Limit(Trcbar, 4);
                    if (mak_ses_trc.Value <= Trcbar.Value)
                    {
                        toolStripStatusLabel1.Text = "Minimum Ses Seviyesi Daha Büyük Olamaz!";
                        toolStripStatusLabel1.ForeColor = Color.Red;
                        if (mak_ses_trc.Value > 4)
                            Trcbar.Value = mak_ses_trc.Value - 4;
                        else
                            Trcbar.Value = 0;
                    }
                    min_ses_text.Text = Trcbar.Value.ToString();
                    break;
                case "repeat_trc":
                    calc_0_100_Limit(Trcbar, 10);
                    if (Trcbar.Value == 1)
                        repeat_text.Text = "0";
                    else
                        repeat_text.Text = (Trcbar.Value / (float)20).ToString();
                    break;
                case "location_delay_push_trc":
                    calc_0_100_Limit(Trcbar, 10);
                    if (Trcbar.Value == 1)
                        location_delay_push_text.Text = "0";
                    else
                        location_delay_push_text.Text = (Trcbar.Value / (float)20).ToString();
                    break;
                case "sens_trc":
                    calc_0_100_Limit(Trcbar, 5);

                    sens_text.Text = Trcbar.Value.ToString();
                    break;
                case "beep_mak_trc":
                    calc_0_100_Limit(Trcbar, 4);
                    if (Trcbar.Value <= beep_min_trc.Value)
                    {
                        toolStripStatusLabel1.Text = "Maksimum Ses Seviyesi Daha Küçük Olamaz!";
                        toolStripStatusLabel1.ForeColor = Color.Red;
                        Trcbar.Value = beep_min_trc.Value + 4;
                    }
                    beep_mak_text.Text = Trcbar.Value.ToString();
                    break;

                case "beep_min_trc":
                    calc_0_100_Limit(Trcbar, 4);
                    if (beep_mak_trc.Value <= Trcbar.Value)
                    {
                        toolStripStatusLabel1.Text = "Minimum Ses Seviyesi Daha Büyük Olamaz!";
                        toolStripStatusLabel1.ForeColor = Color.Red;
                        if (beep_mak_trc.Value > 4)
                            Trcbar.Value = beep_mak_trc.Value - 4;
                        else
                            Trcbar.Value = 0;
                    }
                    beep_min_text.Text = Trcbar.Value.ToString();
                    break;
                case "beep_repeat_trc":
                    calc_0_100_Limit(Trcbar, 10);
                    if (Trcbar.Value == 1)
                        beep_repeat_text.Text = "0";
                    else
                        beep_repeat_text.Text = (Trcbar.Value / (float)20).ToString();
                    break;
                case "vib_power_trc":
                    calc_0_100_Limit(Trcbar, 10);

                    vib_power.Text = Trcbar.Value.ToString();

                    break;
                case "vib_time_trc":
                    calc_0_100_Limit(Trcbar, 10);

                    vib_time.Text = (Trcbar.Value / 10).ToString();
                    break;
                case "green_delay_trc":
                    calc_0_100_Limit(Trcbar, 20);
                    if (Trcbar.Value < 20)
                        green_delay.Text = "0";
                    else
                        green_delay.Text = (Trcbar.Value / (float)40).ToString();

                    break;

            }
          
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
   
        }

        private void mak_ses_trc_MouseUp(object sender, MouseEventArgs e)
        {

                
        }

        private void mak_ses_trc_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            //Thread Set_thread = new Thread(delegate()
            //{
            //    Set_Data();
            //});
            //Set_thread.Start();

            button11.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            toolStripStatusLabel1.Text = "Data Gönderiliyor...";
            toolStripStatusLabel1.ForeColor = Color.Green;

            if (mak_ses_trc.Value != Temp_Value[0])
            {
                SerialSend(Message[Send_Message = 0, Send_Message_Index = 1]);
                if (mak_ses_trc.Value >= 4)
                    SerialSend(((Sended_Data = mak_ses_trc.Value) / 4).ToString());
                else
                    SerialSend((Sended_Data = 1).ToString());

                SerialSend("\r\n");

                Message_Received[0] = false;

                send_wait(0);
                //Thread.Sleep(5);
            }
            if (min_ses_trc.Value != Temp_Value[1])
            {
                SerialSend(Message[Send_Message = 1, Send_Message_Index = 1]);
                if (min_ses_trc.Value >= 4)
                    SerialSend(((Sended_Data = min_ses_trc.Value) / 4).ToString());
                else
                    SerialSend((Sended_Data = 0).ToString());

                SerialSend("\r\n");

                Message_Received[1] = false;

                send_wait(1);
            }
            if (repeat_trc.Value != Temp_Value[2])
            {
                if ((float)repeat_trc.Value >= 10)
                { 
                    SerialSend(Message[Send_Message = 2, Send_Message_Index = 1]);
                    SerialSend(((Sended_Data = repeat_trc.Value) / (float)10).ToString());
                    SerialSend("\r\n");

                    Message_Received[2] = false;

                    send_wait(2);
                }
                else if ((float)beep_repeat_trc.Value == 1)
                {

                    SerialSend(Message[Send_Message = 2, Send_Message_Index = 1]);
                    SerialSend("1");
                    SerialSend("\r\n");

                    Message_Received[2] = false;

                    send_wait(2);
                }
                
            }
            if (location_delay_push_trc.Value != Temp_Value[3])
            {
                if ((float)location_delay_push_trc.Value >= 10)
                {
                    SerialSend(Message[Send_Message = 3, Send_Message_Index = 1]);
                    SerialSend(((Sended_Data = location_delay_push_trc.Value) / (float)10).ToString());
                    SerialSend("\r\n");

                    Message_Received[3] = false;

                    send_wait(3);
                }
                else if ((float)beep_repeat_trc.Value == 1)
                {
                    SerialSend(Message[Send_Message = 3, Send_Message_Index = 1]);
                    SerialSend("1");
                    SerialSend("\r\n");

                    Message_Received[3] = false;

                    send_wait(3);
                }
                
            }
            if (sens_trc.Value != Temp_Value[4])
            {
                SerialSend(Message[Send_Message = 4, Send_Message_Index = 1]);
                if (sens_trc.Value >= 5)
                    SerialSend(((Sended_Data = sens_trc.Value) / 5).ToString());
                else
                    SerialSend("1");
                SerialSend("\r\n");

                Message_Received[4] = false;
                send_wait(4);
            }
            if (beep_mak_trc.Value != Temp_Value[5])
            {
                SerialSend(Message[Send_Message = 5, Send_Message_Index = 1]);
                if (beep_mak_trc.Value >= 4)
                    SerialSend(((Sended_Data = beep_mak_trc.Value) / 4).ToString());
                else
                    SerialSend((Sended_Data = 1).ToString());

                SerialSend("\r\n");

                Message_Received[5] = false;

                send_wait(5);
                //Thread.Sleep(5);

  
            }
            if (beep_min_trc.Value != Temp_Value[6])
            {
                SerialSend(Message[Send_Message = 6, Send_Message_Index = 1]);
                if (beep_min_trc.Value >= 4)
                    SerialSend(((Sended_Data = beep_min_trc.Value) / 4).ToString());
                else
                    SerialSend((Sended_Data = 0).ToString());

                SerialSend("\r\n");

                Message_Received[6] = false;

                send_wait(6);
                //Thread.Sleep(5);

            }
            if (beep_repeat_trc.Value != Temp_Value[7])
            {
                if ((float)beep_repeat_trc.Value >= 10)
                {
                    SerialSend(Message[Send_Message = 7, Send_Message_Index = 1]);
                    SerialSend(((Sended_Data = beep_repeat_trc.Value) / (float)10).ToString());
                    SerialSend("\r\n");

                    Message_Received[7] = false;

                    send_wait(7);
                }
                else if ((float)beep_repeat_trc.Value == 1)
                {
                    SerialSend(Message[Send_Message = 7, Send_Message_Index = 1]);
                    SerialSend("1");
                    SerialSend("\r\n");

                    Message_Received[7] = false;

                    send_wait(7);
                }

            }

            button11.Enabled = true;
            this.Cursor = Cursors.Default;

            toolStripStatusLabel1.Text = "Data Gönderildi.";
            toolStripStatusLabel1.ForeColor = Color.Green;
        }

        //private void Set_Data()
        //{
            
        //}

        public void SerialSend(string str)
        {
            Serial_Event = true;
            try
            {
                sp.Write(str);
                //sp.DiscardInBuffer();
                //sp.DiscardOutBuffer();
            }
            catch
            {
                toolStripStatusLabel1.Text = "Seri Port Data Gönderilemedi!";
                toolStripStatusLabel1.ForeColor = Color.Red;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";

            button11.Enabled = true;

            Sorgu_thread = new Thread(delegate()
            {
                Query_Data();
            });
            Sorgu_thread.Start();
            

            
            //{
            //    Send_Message_With_Timeout(i, 0);
            //    Thread.Sleep(5);
            //}
            
        }

        public void Query_Data()
        {


                int ret = 1;

                button10.Enabled = false;

                this.Cursor = Cursors.WaitCursor;

                toolStripStatusLabel1.Text = "Cihazdan Veriler Alınıyor...";
                toolStripStatusLabel1.ForeColor = Color.Green;

                Int16 i;
                for (i = 0; i < 8; i++)
                {
                    if (Send_Message_With_Timeout(i, 0) == 0)
                    {
                        ret = 0;
                    }

                    Thread.Sleep(20);
                }

                if (ret != 0)
                {
                    mak_ses_text.Text = (mak_ses_trc.Value = Temp_Value[0]).ToString();
                    min_ses_text.Text = (min_ses_trc.Value = Temp_Value[1]).ToString();
                    repeat_text.Text = ((repeat_trc.Value = Temp_Value[2]) / (float)20).ToString();
                    location_delay_push_text.Text = ((location_delay_push_trc.Value = Temp_Value[3]) / (float)20).ToString();
                    sens_text.Text = (sens_trc.Value = Temp_Value[4]).ToString();
                    beep_mak_text.Text = (beep_mak_trc.Value = Temp_Value[5]).ToString();
                    beep_min_text.Text = (beep_min_trc.Value = Temp_Value[6]).ToString();
                    beep_repeat_text.Text = ((beep_repeat_trc.Value = Temp_Value[7]) / (float)20).ToString();

                    button10.Enabled = true;

                    this.Cursor = Cursors.Default;

                    toolStripStatusLabel1.Text = "Cihazdan Veriler Alındı.";
                    toolStripStatusLabel1.ForeColor = Color.Green;
                }
                else
                {
                    button10.Enabled = true;

                    this.Cursor = Cursors.Default;

                    toolStripStatusLabel1.Text = "Cihazdan Veriler Alınamadı.";
                    toolStripStatusLabel1.ForeColor = Color.Red;
                }
            
            
        }

        public int Send_Message_With_Timeout(Int32 mesage_num, Int32 message_indx)
        {
            int ret = 0;
            
            Message_Received[mesage_num] = false;

            SerialSend(Message[Send_Message = mesage_num, Send_Message_Index = message_indx]);

            ret = send_wait(mesage_num);

            return ret;
        }

        public int send_wait(Int32 index)
        {
            int ret = 0;

            timeout.Enabled = true;
            timeout.Stop();
            timeout.Start();
            timeout_exception = false;

            while (!Message_Received[index] && !timeout_exception)
                ;

            if (!timeout_exception)
                ret = 1;

            timeout.Stop();
            timeout.Enabled = false;
            timeout_exception = false;
            

            return ret;
            //Message_Received[index] = false;
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Send_Message_With_Timeout(13, 0);
        }

        private void statusStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";

            button13.Enabled = true;

            Thread Sorgu_thread_1 = new Thread(delegate()
            {
                Query_Data_1();
            });
            Sorgu_thread_1.Start();

            button13.Enabled = true;

        }

        public void Query_Data_1()
        {
 
                int ret = 1;

                button12.Enabled = false;

                this.Cursor = Cursors.WaitCursor;

                toolStripStatusLabel1.Text = "Cihazdan Veriler Alınıyor...";
                toolStripStatusLabel1.ForeColor = Color.Green;

                Int16 i;
                for (i = 8; i < 10; i++)
                {
                    if (Send_Message_With_Timeout(i, 0) == 0)
                    {
                        ret = 0;
                    }

                    Thread.Sleep(20);
                }

                if (ret != 0)
                {
                    vib_power.Text = (vib_power_trc.Value = Vib_Temp[0]).ToString();
                    vib_time.Text = ((vib_time_trc.Value = Vib_Temp[1]) / 10).ToString();
                    green_delay.Text = ((green_delay_trc.Value = Temp_Value[9]) / (float)40).ToString();

                    toolStripStatusLabel1.Text = "Cihazdan Veriler Alındı.";
                    toolStripStatusLabel1.ForeColor = Color.Green;
                }
                else
                {
                    toolStripStatusLabel1.Text = "Cihazdan Veriler Alınamadı.";
                    toolStripStatusLabel1.ForeColor = Color.Red;
                }

                button12.Enabled = true;

                this.Cursor = Cursors.Default;

            
        }

        private void button13_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";

            

            if (vib_power_trc.Value != Vib_Temp[0] || vib_time_trc.Value != Vib_Temp[1])
            {
                SerialSend(Message[Send_Message = 8, Send_Message_Index = 1]);

                if (vib_power_trc.Value >= 10)
                    SerialSend(((Sended_Data = vib_power_trc.Value) / 10).ToString());
                else
                    SerialSend((Sended_Data = 1).ToString());

                SerialSend(":");

                if (vib_time_trc.Value >= 10)
                    SerialSend(((Sended_Data = vib_time_trc.Value) / 10).ToString());
                else
                    SerialSend((Sended_Data = 1).ToString());

                SerialSend("\r\n");

                Message_Received[8] = false;

                send_wait(8);
            }
            if (green_delay_trc.Value != Temp_Value[9])
            {
                SerialSend(Message[Send_Message = 9, Send_Message_Index = 1]);

                if (green_delay_trc.Value >= 20)
                    SerialSend(((Sended_Data = green_delay_trc.Value) / 4).ToString());
                else
                    SerialSend("10");

                SerialSend("\r\n");

                Message_Received[9] = false;

                send_wait(9);
            }

            toolStripStatusLabel1.Text = "Data Gönderildi.";
            toolStripStatusLabel1.ForeColor = Color.Green;
        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";

            button4.Enabled = true;
            
            Thread Sorgu_thread_2 = new Thread(delegate()
            {
                Query_Data_2();
            });
            Sorgu_thread_2.Start();
        }

        private void Query_Data_2()
        {
    

                toolStripStatusLabel1.ForeColor = Color.Green;
                toolStripStatusLabel1.Text = "Saat Ayarları Alınıyor...";

                Send_Message_With_Timeout(10, 0);

                Thread.Sleep(20);

                if (Message_Received[10])
                {
                    toolStripStatusLabel1.ForeColor = Color.Green;
                    toolStripStatusLabel1.Text = "Saat Ayarları Alındı.";
                }
                else
                {
                    toolStripStatusLabel1.ForeColor = Color.Red;
                    toolStripStatusLabel1.Text = "Saat Ayarları Alınamadı!";
                }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";

            if (day_.Text == "")
            {
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Günü Seçiniz!";
                return;
            }
            else
                toolStripStatusLabel1.Text = "";

            if (month_.Text == "")
            {
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Ayı Seçiniz!";
                return;
            }
            else
                toolStripStatusLabel1.Text = "";

            if (year_.Text == "")
            {
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Yılı Seçiniz!";
                return;
            }
            else
                toolStripStatusLabel1.Text = "";

            if (week_day_.Text == "")
            {
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Haftanın Gününü Seçiniz!";
                return;
            }
            else
                toolStripStatusLabel1.Text = "";

            if (hour_.Text == "")
            {
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Saati Seçiniz!";
                return;
            }
            else
                toolStripStatusLabel1.Text = "";

            if (minute_.Text == "")
            {
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Dakikayı Seçiniz!";
                return;
            }
            else
                toolStripStatusLabel1.Text = "";

            SerialSend(Message[Send_Message = 10, Send_Message_Index = 1]);

            string yearStr = ((Convert.ToInt32(year_.Text)) - 2000).ToString().PadLeft(2, '0');
            string monthStr = (Convert.ToInt32(month_.Text)).ToString().PadLeft(2, '0');
            string dayStr = (Convert.ToInt32(day_.Text)).ToString().PadLeft(2, '0');
            string weekdayStr = "";

            switch(week_day_.Text)
            {
                case "Pazartesi":
                    weekdayStr = "1";
                    break;               
                case "Salı":
                    weekdayStr = "2";
                    break;
                case "Çarşamba":
                    weekdayStr = "3";
                    break;
                case "Perşembe":
                    weekdayStr = "4";
                    break;
                case "Cuma":
                    weekdayStr = "5";
                    break;
                case "Cumartesi":
                    weekdayStr = "6";
                    break;
                case "Pazar":
                    weekdayStr = "7";
                    break;

            }
            string hourStr = (Convert.ToInt32(hour_.Text)).ToString().PadLeft(2, '0');
            string minuteStr = (Convert.ToInt32(minute_.Text)).ToString().PadLeft(2, '0');

            SerialSend(yearStr + monthStr + dayStr + ":" + weekdayStr + ":" + hourStr + minuteStr + "\r\n");

            Message_Received[10] = false;

            send_wait(10);

            if (Message_Received[10])
            {
                toolStripStatusLabel1.ForeColor = Color.Green;
                toolStripStatusLabel1.Text = "Saat Ayarlandı.";
            }
            else
            {
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Saat Ayarlanamadı!";
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";

            toolStripStatusLabel1.ForeColor = Color.Green;
            toolStripStatusLabel1.Text = "Ayarlar Sıfırlanıyor...";
            
            SerialSend(Message[Send_Message = 15, Send_Message_Index = 0]);

            Message_Received[15] = false;

            send_wait(15);

            if (Message_Received[15])
            {
                toolStripStatusLabel1.ForeColor = Color.Green;
                toolStripStatusLabel1.Text = "Ayarlar Sıfırlandı.";
            }
            else
            {
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Ayarlar Sıfırlanamadı!";
            }

                                            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            

            Thread Button5_Data_2 = new Thread(delegate()
            {
                Button5_Data();
            });

            Button5_Data_2.Start();

           

           

        }

        private void Button5_Data()
        {
            string[] Send_Str = new string[200];

            int count = 0;

            int i = 0;

            toolStripStatusLabel1.Text = "";

            progressBar2.Value = 0;

            for (i = 0; i < 7; i++)
            {
                for (int j = 0; j < 48; j++)
                {
                    if (Form3.frm3.tablom[i, j].baslangıc != null)
                    {

                        Send_Str[count++] = i.ToString() + ":" + Form3.frm3.tablom[i, j].baslangıc[0] + Form3.frm3.tablom[i, j].baslangıc[1] + Form3.frm3.tablom[i, j].baslangıc[3] + Form3.frm3.tablom[i, j].baslangıc[4] +
                            ":" + Form3.frm3.tablom[i, j].ses_deger.Set_Value.ToString() + ":" + Form3.frm3.tablom[i, j].bitis[0] + Form3.frm3.tablom[i, j].bitis[1] + Form3.frm3.tablom[i, j].bitis[3] + Form3.frm3.tablom[i, j].bitis[4] +
                                ":" + Form3.frm3.tablom[i, j].ses_deger.max_vol.ToString() + ":" + Form3.frm3.tablom[i, j].ses_deger.min_vol.ToString() + ":" +
                                    Form3.frm3.tablom[i, j].ses_deger.max_beep.ToString() + ":" + Form3.frm3.tablom[i, j].ses_deger.min_beep.ToString();
                    }
                }
            }

            i = 0;

            if (count > 0)
            {
                toolStripStatusLabel1.ForeColor = Color.Green;
                toolStripStatusLabel1.Text = "Ayarlar Gönderiliyor...";

                button5.Enabled = false;

                Thread.Sleep(500);

                while (Send_Str[i] != null)
                {
                    progressBar2.Maximum = count;


                    SerialSend(Message[Send_Message = 11, Send_Message_Index = 1]);

                    //Thread.Sleep(50);

                    SerialSend(Send_Str[i]);

                    //Thread.Sleep(50);

                    Message_Received[11] = false;

                    SerialSend("\r\n");

                    //Thread.Sleep(50);

                    send_wait(11);

                    

                    if (Message_Received[11])
                    {
                        progressBar2.Increment(1);

                        i++;

                        if (count == i)
                        {
                            toolStripStatusLabel1.ForeColor = Color.Green;
                            toolStripStatusLabel1.Text = "Ayarlar Gönderildi.";
                        }

                    }
                    else
                    {
                        toolStripStatusLabel1.ForeColor = Color.Red;
                        toolStripStatusLabel1.Text = "Ayarlar Gönderilemedi!";

                        break;
                    }
                }

                button5.Enabled = true;

            }
        }
        private void vib_power_TextChanged(object sender, EventArgs e)
        {

        }

        private void vib_time_TextChanged(object sender, EventArgs e)
        {

        }

        private void green_delay_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SerialSend(Message[Send_Message = 24, Send_Message_Index = 0]);

            if (checkBox1.Checked)
                SerialSend("1\r\n");
            else
                SerialSend("0\r\n");

            Message_Received[24] = false;

            send_wait(24);

        }

        private void button7_Click(object sender, EventArgs e)
        {
            //if(comboBox3.SelectedIndex < 0)
            //{
            //    toolStripStatusLabel1.ForeColor = Color.Red;
            //    toolStripStatusLabel1.Text = "Ses Dosyası Seçiniz!";
            //    return;
            //}
            //else
            //{
            //    toolStripStatusLabel1.ForeColor = Color.Red;
            //    toolStripStatusLabel1.Text = "";
            //}
            int i = 0, indexOfVoice = 0;
            string str;

            vaw = new OpenFileDialog();

            vaw.Multiselect = true;
            vaw.Filter = " Ses Dosyaları |*.wav";

            

            DialogResult dr = vaw.ShowDialog();

            myfileList.Clear();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                //vaw.OpenFile();
                try
                {
                    fileIndex = new int[vaw.FileNames.Length];


                    foreach (String file in vaw.FileNames)
                    {

                        if (file.IndexOf("01.wav") >= 0)
                        {
                            richTextBox1.Text += ("01.wav\r\n");
                            fileIndex[i++] = 1;

                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }
                            
                                
                        else if (file.IndexOf("02.wav") >= 0)
                        {
                            richTextBox1.Text += ("02.wav\r\n");
                            fileIndex[i++] = 2;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("03.wav") >= 0)
                        {
                            richTextBox1.Text += ("03.wav\r\n");
                            fileIndex[i++] = 3;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("04.wav") >= 0)
                        {
                            richTextBox1.Text += ("04.wav\r\n");
                            fileIndex[i++] = 4;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("05.wav") >= 0)
                        {
                            richTextBox1.Text += ("05.wav\r\n");
                            fileIndex[i++] = 5;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("06.wav") >= 0)
                        {
                            richTextBox1.Text += ("06.wav\r\n");
                            fileIndex[i++] = 6;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("07.wav") >= 0)
                        {
                            richTextBox1.Text += ("07.wav\r\n");
                            fileIndex[i++] = 7;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("08.wav") >= 0)
                        {
                            richTextBox1.Text += ("08.wav\r\n");
                            fileIndex[i++] = 8;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("09.wav") >= 0)
                        {
                            richTextBox1.Text += ("09.wav\r\n");
                            fileIndex[i++] = 9;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("10.wav") >= 0)
                        {
                            richTextBox1.Text += ("10.wav\r\n");
                            fileIndex[i++] = 10;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("11.wav") >= 0)
                        {
                            richTextBox1.Text += ("11.wav\r\n");
                            fileIndex[i++] = 11;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }
                        else if (file.IndexOf("12.wav") >= 0)
                        {
                            richTextBox1.Text += ("12.wav\r\n");
                            fileIndex[i++] = 12;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("13.wav") >= 0)
                        {
                            richTextBox1.Text += ("13.wav\r\n");
                            fileIndex[i++] = 13;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("14.wav") >= 0)
                        {
                            richTextBox1.Text += ("14.wav\r\n");
                            fileIndex[i++] = 14;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("15.wav") >= 0)
                        {
                            richTextBox1.Text += ("15.wav\r\n");
                            fileIndex[i++] = 15;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("16.wav") >= 0)
                        {
                            richTextBox1.Text += ("16.wav\r\n");
                            fileIndex[i++] = 16;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("17.wav") >= 0)
                        {
                            richTextBox1.Text += ("17.wav\r\n");
                            fileIndex[i++] = 17;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("18.wav") >= 0)
                        {
                            richTextBox1.Text += ("18.wav\r\n");
                            fileIndex[i++] = 18;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("19.wav") >= 0)
                        {
                            richTextBox1.Text += ("19.wav\r\n");
                            fileIndex[i++] = 19;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("20.wav") >= 0)
                        {
                            richTextBox1.Text += ("20.wav\r\n");
                            fileIndex[i++] = 20;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("bekleyin.wav") >= 0)
                        {
                            richTextBox1.Text += ("bekleyin.wav\r\n");
                            fileIndex[i++] = 21;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("geciniz.wav") >= 0)
                        {
                            richTextBox1.Text += ("geciniz.wav\r\n");
                            fileIndex[i++] = 22;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("konum.wav") >= 0)
                        {
                            richTextBox1.Text += ("konum.wav\r\n");
                            fileIndex[i++] = 23;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }

                        else if (file.IndexOf("talep.wav") >= 0)
                        {
                            richTextBox1.Text += ("talep.wav\r\n");
                            fileIndex[i++] = 24;
                            myfileList.Add(new files(vaw.FileNames[indexOfVoice], vaw.FileNames[indexOfVoice].Length));
                        }
                        else
                        {


                        }

                        indexOfVoice++;

                    }

                    File_Opened = true;

                    button8.Enabled = true;
                }
                catch
                {

                }
            }

                    
        }

        private void tabPage5_Click_1(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

            Thread thread = new Thread(delegate ()
            {
                ses_gonder();
            });
            thread.Start();

        }

        public void ses_gonder()
        {
            int i;

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    toolStripStatusLabel1.Text = "";

                    if (!File_Opened)
                    {
                        toolStripStatusLabel1.ForeColor = Color.Red;
                        toolStripStatusLabel1.Text = "Ses Dosyası Seçiniz!";
                        return;
                    }

                    button7.Enabled = false;
                    button8.Enabled = false;
                    button1.Enabled = false;

                    file_sending = true;

                    label10.Visible = true;

                    label10.Text = "Gönderilmeye Hazırlanıyor...";
                });

            }
            

            for (i = 0; i < myfileList.Count; i++)
            {

                Vaw_Array = File.OpenRead(myfileList[i].filename);

                vaw_read = new byte[Vaw_Array.Length];

                Vaw_Array.Read(vaw_read, 0, Convert.ToInt32(Vaw_Array.Length));

                string[] fileWay = (myfileList[i].filename).Split('\\');

                SerialSend(Message[Send_Message = 12, Send_Message_Index = 1]);

                if((Device_Version == 4) || ((fileIndex[i]-1 != 23) && (Device_Version != 4)))
                {
                    SerialSend((fileIndex[i] - 1).ToString() + ":" + Vaw_Array.Length.ToString() + "\r\n");
                }      
                else
                {
                    toolStripStatusLabel1.ForeColor = Color.Red;
                    toolStripStatusLabel1.Text = "Yanlış Dosya Seçimi!";
                }
                                   
                Message_Received[12] = false;

                Ack_received = false;

                send_wait(12);

                toolStripStatusLabel1.ForeColor = Color.Green;
                toolStripStatusLabel1.Text = "Dosya Gönderilmeye Hazırlanıyor...";

                Thread.Sleep(500);

                toolStripStatusLabel1.Text = "Dosya Gönderiliyor...";
                label10.Text = fileWay.Last() + " gönderiliyor...";

                if (Message_Received[12])
                {                 

                    Vaw_send = true;

                    int Ack_count = 0;

                    progressBar1.Maximum = Convert.ToInt32(Vaw_Array.Length);



                    for (int j = 0; j < Vaw_Array.Length; j++)
                    {

                        try
                        {
                            sp.Write(vaw_read, j, 1);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                        Ack_count++;

                        progressBar1.Increment(1);

                        //if (j >= (Vaw_Array.Length / 100))


                        if (Ack_count == 512)
                        {
                            Ack_count = 0;

                            timeout_file.Enabled = true;

                            while (!Ack_received)
                            {
                                if (vaw_send_timeout)
                                {
                                    vaw_send_timeout = false;
                                    timeout_file.Enabled = false;

                                    toolStripStatusLabel1.Text = "Dosya Gönderme Hatası!";
                                    MessageBox.Show("Dosya Gönderme Hatası!");

                                    Ack_received = false;

                                   

                                    progressBar1.Value = 0;

                                    Vaw_send = false;

                                    file_sending = false;

                                    return;
                                }
                            }
                            timeout_file.Enabled = false;

                            Ack_received = false;
                        }
                    }

                    progressBar1.Value = 0;

                    Vaw_send = false;

                    //file_sending = false;

                    toolStripStatusLabel1.Text = "Ses Dosyası Gönderildi.";

                    label10.Text = "Ses Dosyası Gönderildi.";

                    //ShowPopup("Dosyası Gönderildi.", 185, 80);

                    Thread.Sleep(5000);

                }
                else
                {

                    progressBar1.Value = 0;

                    Vaw_send = false;

                    toolStripStatusLabel1.ForeColor = Color.Red;
                    toolStripStatusLabel1.Text = "Dosya Gönderme Hatası!";

                    label10.Text = "";

                    MessageBox.Show(vaw.FileNames[i] + " dosyası gönderilemedi!");

                }
            }
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    label10.Text = "";
                    richTextBox1.Text = "";

                    button7.Enabled = true;
                    button8.Enabled = true;
                    button1.Enabled = true;

                    label10.Visible = false;

                    Vaw_send = false;

                    file_sending = false;

                });
            }

                    



        }

        void ShowPopup(string text, int width, int height)
        {
            // Popup adında bir form oluştur
            Form Popup = new Form
            {
                Width = width, // genişlik parametresini ata
                Height = height, // yükseklik parametresini ata
                ShowInTaskbar = false, // başlat çubuğunda görünme
                FormBorderStyle = FormBorderStyle.None, // Form kenarlıkları olmasın
                BackColor = Color.LightCyan, // Arkaplan "Mısır çiçeği mavisi" rengi
                StartPosition = FormStartPosition.CenterScreen, // Formu ekrana ortala
                TopMost = true, // Her zaman üstte
                Cursor = Cursors.Hand // İmleç, el şeklinde olsun
            };

            // Form click eventi
            Popup.Click += delegate
            {
                Popup.Dispose(); // tıklanıldığında formu kapat
            };

            // Form içi grafik işlemleri
            Popup.Paint += delegate
            {
                // Formun etrafına bir dörtgen çiz (Rengi siyah = Pens.Black)
                Popup.CreateGraphics().DrawRectangle(Pens.Black, 0, 0, (width - 1), (height - 1));
            };

            // lbl_text adında bir label oluştur
            Label lbl_text = new Label
            {
                Left = 25, // sol tarafa uzaklık 30 pixel
                Top = 30, // yukarıya uzaklık 30 pixel
                AutoSize = true, // label boyutunu text'e göre  ayarla
                Font = new Font(this.Font, FontStyle.Bold), // font kalın olsun
                Text = text // metin parametresini ata
            };

            lbl_text.Click += delegate
            {
                Popup.Dispose(); // tıklanıldığında formu kapat
            };

            // oluşturulan labeli forma ekle
            Popup.Controls.Add(lbl_text);

            // pop-up formu göster
            Popup.ShowDialog();
            System.Threading.Thread.Sleep(1000);
            Popup.Dispose(); // 
        }

        private void clear_program()
        {
            
            seri_port_flag = false;
            Serial_Event = false;
            Alınan_Data = "";
            Read_Byte = 0;

            for (int i = 0; i < Temp_Value.Length; i++)
                Temp_Value[i] = 0;

            for (int i = 0; i < Vib_Temp.Length; i++)
                Vib_Temp[i] = 0;

            for (int i = 0; i < Message_Received.Length; i++)
                Message_Received[i] = false;

            //Int32[] Temp_Value;
            //Int32[] Vib_Temp;

            //Boolean[] Message_Received;
            Send_Message = 0;
            Send_Message_Index = 0;
            Device_Version = 0;
            Sended_Data = 0;
            Sended_Data_1 = 0;
           
            timeout_exception = false;
            pow = "";
            time = "";
            year = "";
            month = "";
            day = "";
            hour = "";
            minute = "";
            day_of_week = "";

            //for (int i = 0; i < vaw_read.Length; i++)
            //    vaw_read[i] = 0;



            File_Opened = false;
            Vaw_send = false;
            Ack_received = false;
            file_sending = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {
            string[] portNames = SerialPort.GetPortNames();

            comboBox1.Items.Clear();

            foreach (string port in portNames)
                comboBox1.Items.Add(port);
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            
        }

        private void comboBox3_MouseClick(object sender, MouseEventArgs e)
        {
            toolStripStatusLabel1.Text = "";
        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {
            Form3.frm3.ShowDialog();

            toolStripStatusLabel1.ForeColor = Color.Green;
            toolStripStatusLabel1.Text = "Ayarlar Kaydedildi.";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button15_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {
            var culture = new System.Globalization.CultureInfo("tr-TR");
    
            currentTime = DateTime.Now;

            hour_.Text = currentTime.Hour.ToString().PadLeft(2,'0');
            minute_.Text = currentTime.Minute.ToString().PadLeft(2, '0');
            year_.Text = currentTime.Year.ToString();
            week_day_.Text = culture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);
            day_.Text = currentTime.Day.ToString().PadLeft(2, '0');
            month_.Text = currentTime.Month.ToString().PadLeft(2, '0');

        }

        private void button16_Click(object sender, EventArgs e)
        {
            OpenFileDialog firmware = new OpenFileDialog();
            firmware.Filter = "Yazılım Dosyası |*.bin";
            firmware.ShowDialog();
            textBox1.Text = firmware.FileName;

            try
            {
                Firmware_Array = File.OpenRead(firmware.FileName);

                firmware_read = new byte[Firmware_Array.Length];

                Firmware_Array.Read(firmware_read, 0, Convert.ToInt32(Firmware_Array.Length));

                File_Opened = true;

                button15.Enabled = true;
            }
            catch
            {

            }

        }



        private void button17_Click(object sender, EventArgs e)
        {
            if(textBox2.Text == "isbak_eyb" && textBox3.Text == "123321")
            {
                //button15.Enabled = true;
                button16.Enabled = true;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void mak_ses_text_TextChanged(object sender, EventArgs e)
        {

        }

        private void button15_Click_1(object sender, EventArgs e)
        {

            Thread thread = new Thread(delegate ()
            {
                yazilim_gonder();
            });
            thread.Start();

        }

        public void yazilim_gonder()
        {
            toolStripStatusLabel1.Text = "";

            if (!File_Opened)
            {
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Yazılım Dosyası Seçiniz!";
                return;
            }

            file_sending = true;

            SerialSend(Message[Send_Message = 12, Send_Message_Index = 1]);

            if(Device_Version == 4)
                SerialSend("24" + ":" + Firmware_Array.Length.ToString() + "\r\n");
            else
                SerialSend("23" + ":" + Firmware_Array.Length.ToString() + "\r\n");

            Message_Received[12] = false;

            Ack_received = false;

            send_wait(12);

            toolStripStatusLabel1.ForeColor = Color.Green;
            toolStripStatusLabel1.Text = "Dosya Gönderilmeye Hazırlanıyor...";

            Thread.Sleep(500);

            button15.Enabled = false;
            button16.Enabled = false;

            toolStripStatusLabel1.Text = "Dosya Gönderiliyor...";

            if (Message_Received[12])
            {

                Vaw_send = true;

                int Ack_count = 0;

                progressBar3.Maximum = Convert.ToInt32(Firmware_Array.Length);



                for (int j = 0; j < Firmware_Array.Length; j++)
                {

                    try
                    {
                        sp.Write(firmware_read, j, 1);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                    Ack_count++;

                    progressBar3.Increment(1);

                    //if (j >= (Vaw_Array.Length / 100))


                    if (Ack_count == 512)
                    {
                        Ack_count = 0;

                        timeout_file.Enabled = true;

                        while (!Ack_received)
                        {
                            if (vaw_send_timeout)
                            {
                                vaw_send_timeout = false;
                                timeout_file.Enabled = false;

                                toolStripStatusLabel1.Text = "Yazılım Gönderme Hatası!";
                                MessageBox.Show("Yazılım Gönderme Hatası!");

                                Ack_received = false;

                                progressBar3.Value = 0;

                                Vaw_send = false;

                                file_sending = false;

                                return;
                            }
                        }
                        timeout_file.Enabled = false;

                        Ack_received = false;
                    }
                }


                progressBar3.Value = 0;

                Vaw_send = false;

                file_sending = false;


                toolStripStatusLabel1.Text = "Yazılım Gönderildi.";

                ShowPopup("Yazılım Gönderildi.", 185, 80);



            }
            else
            {
                progressBar3.Value = 0;

                Vaw_send = false;

                file_sending = false;

                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Dosya Gönderme Hatası!";

                MessageBox.Show("Dosya Gönderme Hatası!");


            }

            button15.Enabled = true;
            button16.Enabled = true;

        }
    }
}
