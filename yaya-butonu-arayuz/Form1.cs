using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;

namespace yayaButonuArayuz
{
    public partial class mainFrm : Form
    {
        public mainFrm()
        {
            InitializeComponent();
        }

        private const char cmdStartChar = '$';
        private const char cmdQueryChar = '?';
        private const char cmdAutoChar = '#';
        private const char parameterChar = ':';

        public SerialPort sp;

        public class ID
        {
            public string idNo;
            public object o;
        }


        public class Cmd
        {
            public string command;
            public object o;
        }

        private List<ID> controls = new List<ID>();
        private List<Cmd> cmdControls = new List<Cmd>();
         
        private bool dosyaBekleniyor = false;
        private bool cevapGelmedi = false;
        private bool dosyaGonderiliyor = false;

        AutoResetEvent dosyaGonderEvent = new AutoResetEvent(false);
        AutoResetEvent parcaGonderEvent = new AutoResetEvent(false);

        AutoResetEvent cevapGeldiEvent = new AutoResetEvent(false);
        readonly object _locker = new object();

        private object findID(string idNo)
        {
            foreach (ID tempID in controls)
            {
                if (tempID.idNo.Equals(idNo)) 
                    return tempID.o;
            }
            return null;
        }

        private string idToText(string idNo)
        {
            string ret = String.Empty;
            object o = findID(idNo);
            if (o == null)
                return idNo;
            if (o.GetType() == typeof(TextBox))
                ret = (o as TextBox).Text;
            else if (o.GetType() == typeof(Label))
                ret = (o as Label).Text;
            else if (o.GetType() == typeof(ComboBox))
                ret = (o as ComboBox).Text;
            else if (o.GetType() == typeof(TrackBar))
                ret = (o as TrackBar).Value.ToString();
            else if (o.GetType() == typeof(NumericUpDown))
                ret = (o as NumericUpDown).Value.ToString();
            return ret;
        }

        private void writeOutput(object obj, string output)
        { 
            if (obj.GetType() == typeof(TextBox))
                (obj as TextBox).Text = output;
            else if (obj.GetType() == typeof(Label))
                (obj as Label).Text = output;
        }

        private void writeCmd(string cmd)
        {
            string sendCmd = String.Format("{0}{1}{2}\r\n", cmdStartChar, cmd, cmdQueryChar);
            BackgroundWorker writeWorker = new BackgroundWorker();
            writeWorker.DoWork += (sender, e) =>
            {
                lock (_locker)
                {
                    if (cevapGelmedi == true)
                        cevapGeldiEvent.WaitOne();
                    sp.Write(sendCmd);
                    Console.WriteLine(sendCmd + " gonderildi");
                    cevapGelmedi = true;
                }
            };
            writeWorker.RunWorkerAsync();
        }

        private string handleMessage(string cmd, object o = null)
        {
            string ret = cmd;
            Regex r = new Regex(@"write\((.*)\)");
            if (r.IsMatch(cmd))
            {
                Match m = r.Match(cmd);
                string seriwr = convertIDtoText(m.Groups[1].Value + "\r\n");
                sp.Write(seriwr);
            }
            else
            {
                r = new Regex(@"send\((.*)\)");
                if (r.IsMatch(cmd))
                {
                    Match m = r.Match(cmd);
                    string dosyaYolu = openFile();
                    if (!dosyaYolu.Equals(String.Empty))
                    {
                        string dosyaNumara = idToText(m.Groups[1].Value);
                        FileInfo f = new FileInfo(dosyaYolu);
                        sp.Write(String.Format("$SEND:{0}:{1}\r\n", dosyaNumara, f.Length));
                        dosyaBekleniyor = true;
                        BackgroundWorker dosyaHazirlaGonderWorker = new BackgroundWorker();
                        dosyaHazirlaGonderWorker.DoWork += (sender, e) => dosyaHazirlaGonderWorker_DoWork(sender, e, dosyaYolu);
                        dosyaHazirlaGonderWorker.RunWorkerAsync();
                    }
                    ret = String.Empty;
                }
            }
            return ret;
        }

        private void dosyaHazirlaGonderWorker_DoWork(object sender, DoWorkEventArgs e, string dosyaYolu)
        {
            BackgroundWorker sendFileWork = new BackgroundWorker();
            sendFileWork.DoWork += (sender2, e2) => sendFileWork_DoWork(sender, e2, dosyaYolu);
            //sendFileWork.RunWorkerCompleted += (sender2, e2) =>
            //{
            //    MessageBox.Show("Dosya gönderildi", "Yaya Butonu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //};

            dosyaGonderEvent.WaitOne();
            dosyaBekleniyor = false;
            sendFileWork.RunWorkerAsync();
        }

        public byte[] ReadByteArrayFromFile(string fileName)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }

        void sendFileWork_DoWork(object sender, DoWorkEventArgs e, string fileName)
        {
            dosyaGonderiliyor = true;
            try
            {
                Console.WriteLine("Dosya gönderiliyor: " + fileName);
                int index = 0, blockSize = 512;
                byte[] fileBytes = ReadByteArrayFromFile(fileName);
                if (fileBytes.Length <= blockSize)
                    sp.Write(fileBytes, 0, fileBytes.Length);
                else
                {
                    while (index < fileBytes.Length)
                    {
                        int data;
                        if (fileBytes.Length - index < blockSize)
                            data = fileBytes.Length - index;
                        else
                            data = blockSize;
                        sp.Write(fileBytes, index, data);
                        index += data;
                        if (index < fileBytes.Length)
                            parcaGonderEvent.WaitOne();
                    }
                }

                if (index == fileBytes.Length)
                    Console.WriteLine(String.Format("Gönderildi. {0} bytes, Dosya: {1}", index, fileName));
                else
                    Console.WriteLine(String.Format("Aktarım hatası!! Gerçek: {0} bytes, Gönderilen: {1} bytes", fileBytes.Length, index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata!!! " + ex.Message);
            }
            dosyaGonderiliyor = false;
        }

        private string openFile()
        {
            string ret = String.Empty;
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
                ret = ofd.FileName;
            return ret;
        }

        private void spRead()
        {
            StringBuilder strb = new StringBuilder();
            try
            {
                while (true)
                {
                    int readByte = sp.ReadByte();
                    if (readByte == 6 && dosyaGonderiliyor)
                    {
                        parcaGonderEvent.Set();
                        continue;
                    }

                    if (readByte != 10)
                    {
                        strb.Append(Convert.ToChar(readByte));
                        continue;
                    }
                    else
                    {
                        string receivedData = strb.ToString();
                        Console.WriteLine("Received: " + receivedData);
                        strb.Clear();
                        string regOkPattern = @"\$(.*)\r";
                        string regCmdPattern = @"[\$#](.*)\:(.*)\r";
                        Regex regOk = new Regex(regOkPattern);
                        Regex regCmd = new Regex(regCmdPattern);

                        if (regCmd.IsMatch(receivedData))
                        {
                            cevapGelmedi = false;
                            cevapGeldiEvent.Set();
                            Match m = regCmd.Match(receivedData);
                            lock (cmdControls)
                            {
                                formCntDegis(m);
                            }
                        }
                        else if (regOk.IsMatch(receivedData))
                        {
                            Match m = regOk.Match(receivedData);
                            switch (m.Groups[1].Value)
                            {
                                case "OK":
                                    //dosya kabul cevabı geldiyse
                                    if (dosyaBekleniyor)
                                        dosyaGonderEvent.Set();
                                    break;
                                default:
                                    break;
                            }
                        }

                    }

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void formCntDegis(Match m)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<Match>(formCntDegis), m);
            }
            else
            {
                foreach (Cmd tempCmd in cmdControls)
                {
                    if (tempCmd.command.Equals(m.Groups[1].Value))
                    {
                        object o = tempCmd.o;
                        string cevap = m.Groups[2].Value;
                        if (o.GetType() == typeof(Label))
                            (o as Label).Text = cevap;
                        else if (o.GetType() == typeof(TextBox))
                            (o as TextBox).Text = cevap;
                        else if (o.GetType() == typeof(TrackBar))
                            (o as TrackBar).Value = Convert.ToInt32(cevap);
                        else if (o.GetType() == typeof(NumericUpDown))
                            (o as NumericUpDown).Value = Convert.ToInt32(cevap);
                    }
                }
            }
        }

        private string convertIDtoText(string cmd)
        {
            string ret = cmd;
            if (cmd.IndexOf('{') < 0)
                return cmd;
            else
            {
                Regex r = new Regex("({)(?<idNo>.*?)(})");
                MatchCollection c = r.Matches(cmd);
                foreach (Match m in c)
                {
                    string txt = String.Empty;
                    object o = findID(m.Groups["idNo"].Value);
                    if (o.GetType() == typeof(TextBox))
                        txt = (o as TextBox).Text;
                    else if (o.GetType() == typeof(TrackBar))
                        txt = (o as TrackBar).Value.ToString();
                    else if (o.GetType() == typeof(NumericUpDown))
                        txt = (o as NumericUpDown).Value.ToString();
                    else if (o.GetType() == typeof(ComboBox))
                        txt = (o as ComboBox).Text;
                    else if (o.GetType() == typeof(Label))
                        txt = (o as Label).Text;
                    ret = ret.Replace("{" + m.Groups["idNo"].Value + "}", txt);
                }
            }
            return ret;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //config dosyası kontrolü (yoksa hata ver)
            if (!File.Exists(@"config.xml")) {
                MessageBox.Show("Config dosyası bulunamadı!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } 
             
            //config dosyasını yükle
            XDocument configXML = XDocument.Load(@"config.xml");
            var settingsXML = configXML.Root.Element("settings");
            string sPort = settingsXML.Element("serialPort").Value;

            sp = new SerialPort(sPort);
            if (settingsXML.Element("baudRate") != null && !settingsXML.Element("baudRate").Value.Equals(String.Empty))
                sp.BaudRate = Convert.ToInt32(settingsXML.Element("baudRate").Value);
            else
                sp.BaudRate = 115200;
            if (settingsXML.Element("parity") != null && !settingsXML.Element("parity").Value.Equals(String.Empty))
            {
                string par = settingsXML.Element("parity").Value.ToLower();
                switch (par)
                {
                    case "none":
                        sp.Parity = Parity.None;
                        break;
                    case "odd":
                        sp.Parity = Parity.Odd;
                        break;
                    case "even":
                        sp.Parity = Parity.Even;
                        break;
                    case "mark":
                        sp.Parity = Parity.Mark;
                        break;
                    case "space":
                        sp.Parity = Parity.Space;
                        break;
                    default:
                        sp.Parity = Parity.None;
                        break;
                }
            }
            else
                sp.Parity = Parity.None;
            if (settingsXML.Element("dataBits") != null && !settingsXML.Element("dataBits").Value.Equals(String.Empty))
            {
                string dataStr = settingsXML.Element("dataBits").Value;
                sp.DataBits = Convert.ToInt32(dataStr);
            }
            else
                sp.DataBits = 8;
            if (settingsXML.Element("stopBits") != null && !settingsXML.Element("stopBits").Value.Equals(String.Empty))
            {
                string stopStr = settingsXML.Element("stopBits").Value.ToLower();
                switch (stopStr)
                {
                    case "none":
                        sp.StopBits = StopBits.None;
                        break;
                    case "one":
                        sp.StopBits = StopBits.One;
                        break;
                    case "two":
                        sp.StopBits = StopBits.Two;
                        break;
                    case "onepointfive":
                        sp.StopBits = StopBits.OnePointFive;
                        break;
                    default:
                        sp.StopBits = StopBits.One;
                        break;
                }
            }
            else
                sp.StopBits = StopBits.One;

            sp.Open();

            Thread spReadThread = new Thread(new ThreadStart(spRead));
            spReadThread.IsBackground = true;
            spReadThread.Start();

            //tasarım koordinatları
            try
            {
                int posX = 5, posY = 10;
                var queryGroups = configXML.Root.Element("groups").Elements("group");
                foreach (var group in queryGroups)
                {
                    bool groupEnabled = true;
                    if (group.Attribute("enabled") != null)
                        if (group.Attribute("enabled").Value.ToLower().Equals("false"))
                            groupEnabled = false;
                    foreach (var item in group.Elements("item"))
                    {
                        switch (item.Attribute("type").Value)
                        {
                            case "label":
                                Label tempLbl = new Label();
                                if (item.Element("text") != null && !item.Element("text").Value.Equals(String.Empty))
                                    tempLbl.Text = item.Element("text").Value;
                                if (item.Attribute("cmd") != null && !item.Attribute("cmd").Value.Equals(String.Empty))
                                {
                                    Cmd tempCmd = new Cmd();
                                    tempCmd.command = item.Attribute("cmd").Value;
                                    tempCmd.o = tempLbl;
                                    lock (cmdControls) cmdControls.Add(tempCmd);

                                    if (item.Attribute("query") != null && item.Attribute("query").Value.ToLower().Equals("true"))
                                        writeCmd(item.Attribute("cmd").Value);
                                }
                                tempLbl.Bounds = new Rectangle(posX, posY, 0, 0);
                                tempLbl.AutoSize = true;
                                if (item.Attribute("enabled") != null)
                                {
                                    if (item.Attribute("enabled").Value.ToLower().Equals("false")) 
                                        tempLbl.Enabled = false;
                                } 
                                else if (!groupEnabled)
                                    tempLbl.Enabled = false;
                                tempLbl.Parent = this;
                                posX += tempLbl.Width + 5;
                                resizeMainFrm(tempLbl);
                                break;

                            case "scrollbar":
                                TrackBar tempScrl = new TrackBar();
                                if (item.Attribute("id") != null && !item.Attribute("id").Value.Equals(String.Empty))
                                {
                                    ID tempID = new ID();
                                    tempID.o = tempScrl;
                                    tempID.idNo = item.Attribute("id").Value;
                                    controls.Add(tempID);
                                }
                                if (item.Attribute("cmd") != null && !item.Attribute("cmd").Value.Equals(String.Empty))
                                {
                                    Cmd tempCmd = new Cmd();
                                    tempCmd.command = item.Attribute("cmd").Value;
                                    tempCmd.o = tempScrl;
                                    lock (cmdControls) cmdControls.Add(tempCmd);
                                    if (item.Attribute("query") != null && item.Attribute("query").Value.ToLower().Equals("true"))
                                    {
                                        writeCmd(item.Attribute("cmd").Value);
                                    }
                                }
                                tempScrl.Bounds = new Rectangle(posX, posY, 100, 1);
                                tempScrl.AutoSize = true; 
                                if (item.Attribute("min") != null)
                                    tempScrl.Minimum = Convert.ToInt32(item.Attribute("min").Value); 

                                if (item.Attribute("max") != null)
                                    tempScrl.Maximum = Convert.ToInt32(item.Attribute("max").Value);
                                if (item.Attribute("enabled") != null)
                                {
                                    if (item.Attribute("enabled").Value.ToLower().Equals("false"))
                                        tempScrl.Enabled = false;
                                }
                                else if (!groupEnabled)
                                    tempScrl.Enabled = false;
                                tempScrl.Parent = this;
                                tempScrl.TickStyle = TickStyle.BottomRight;
                                posX += tempScrl.Width + 5;
                                resizeMainFrm(tempScrl);
                                break;

                            case "numeric":
                                NumericUpDown tempNumeric = new NumericUpDown();
                                if (item.Attribute("id") != null && !item.Attribute("id").Value.Equals(String.Empty))
                                {
                                    ID tempID = new ID();
                                    tempID.o = tempNumeric;
                                    tempID.idNo = item.Attribute("id").Value;
                                    controls.Add(tempID);
                                }
                                if (item.Attribute("cmd") != null && !item.Attribute("cmd").Value.Equals(String.Empty))
                                {
                                    Cmd tempCmd = new Cmd();
                                    tempCmd.command = item.Attribute("cmd").Value;
                                    tempCmd.o = tempNumeric;
                                    lock (cmdControls) cmdControls.Add(tempCmd);
                                    if (item.Attribute("query") != null && item.Attribute("query").Value.ToLower().Equals("true"))
                                    {
                                        writeCmd(item.Attribute("cmd").Value);
                                    }
                                }
                                tempNumeric.Bounds = new Rectangle(posX, posY, 50, 1);
                                tempNumeric.AutoSize = true;
                                if (item.Attribute("min") != null)
                                    tempNumeric.Minimum = Convert.ToInt32(item.Attribute("min").Value);
                                if (item.Attribute("max") != null)
                                    tempNumeric.Maximum = Convert.ToInt32(item.Attribute("max").Value);
                                if (item.Attribute("enabled") != null)
                                {
                                    if (item.Attribute("enabled").Value.ToLower().Equals("false"))
                                        tempNumeric.Enabled = false;
                                }
                                else if (!groupEnabled)
                                    tempNumeric.Enabled = false;
                                tempNumeric.Parent = this;
                                
                                posX += tempNumeric.Width + 5;
                                resizeMainFrm(tempNumeric);
                                break;

                            case "textbox":
                                TextBox tempTxtBox = new TextBox();
                                if (item.Attribute("id") != null && !item.Attribute("id").Value.Equals(String.Empty))
                                {
                                    ID tempID = new ID();
                                    tempID.o = tempTxtBox;
                                    tempID.idNo = item.Attribute("id").Value;
                                    controls.Add(tempID);
                                }
                                if (item.Element("text") != null && !item.Element("text").Value.Equals(String.Empty))
                                    tempTxtBox.Text = handleMessage(item.Element("text").Value, tempTxtBox);
                                if (item.Attribute("cmd") != null && !item.Attribute("cmd").Value.Equals(String.Empty))
                                {
                                    Cmd tempCmd = new Cmd();
                                    tempCmd.command = item.Attribute("cmd").Value;
                                    tempCmd.o = tempTxtBox;
                                    lock (cmdControls) cmdControls.Add(tempCmd);
                                    if (item.Attribute("query") != null && item.Attribute("query").Value.ToLower().Equals("true"))
                                    {
                                        writeCmd(item.Attribute("cmd").Value);
                                    }
                                }
                                tempTxtBox.Bounds = new Rectangle(posX, posY, 100, 100);
                                if (item.Attribute("enabled") != null)
                                {
                                    if (item.Attribute("enabled").Value.ToLower().Equals("false"))
                                        tempTxtBox.Enabled = false;
                                }
                                else if (!groupEnabled)
                                    tempTxtBox.Enabled = false;
                                tempTxtBox.Parent = this;
                                posX += tempTxtBox.Width + 5;
                                resizeMainFrm(tempTxtBox);
                                break;

                            case "button":
                                Button tempBtn = new Button();
                                if (item.Element("text") != null && !item.Element("text").Value.Equals(String.Empty))
                                    tempBtn.Text = item.Element("text").Value;
                                tempBtn.Bounds = new Rectangle(posX, posY, 0, 0);
                                tempBtn.AutoSize = true;
                                if (item.Attribute("enabled") != null)
                                {
                                    if (item.Attribute("enabled").Value.ToLower().Equals("false"))
                                        tempBtn.Enabled = false;
                                }
                                else if (!groupEnabled)
                                    tempBtn.Enabled = false;
                                tempBtn.Parent = this;
                                if (item.Element("click") != null)
                                {
                                    var clickEvent = item.Element("click");
                                    string cmd = clickEvent.Value;
                                    tempBtn.Click += new EventHandler(
                                            delegate(object o2, EventArgs ev)
                                            {
                                                if (clickEvent.Attribute("output") != null)
                                                {
                                                    string outputID = clickEvent.Attribute("output").Value;
                                                    object tempObj = findID(outputID);
                                                    if (tempObj != null)
                                                        writeOutput(tempObj, handleMessage(cmd));
                                                }
                                                else
                                                    handleMessage(cmd);
                                            }
                                        );
                                }
                                posX += tempBtn.Width + 5;
                                resizeMainFrm(tempBtn);
                                break;

                            case "combobox":
                                ComboBox tempCmbBox = new ComboBox();
                                if (item.Attribute("id") != null)
                                {
                                    ID tempID = new ID();
                                    tempID.o = tempCmbBox;
                                    tempID.idNo = item.Attribute("id").Value;
                                    controls.Add(tempID);
                                }
                                if (item.Element("nodes") != null)
                                {
                                    if (item.Element("nodes").Elements("nodes") != null)
                                    {
                                        var nodes = item.Element("nodes").Elements("node");
                                        foreach (var node in nodes)
                                            tempCmbBox.Items.Add(node.Value);
                                    }
                                }
                                else
                                {
                                    var auto = item.Attribute("fill");
                                    if (auto != null && auto.Value.ToLower().Equals("true"))
                                    {
                                        var min = item.Attribute("min");
                                        var max = item.Attribute("max");
                                        if (min != null && max != null)
                                        {
                                            int minitm = Convert.ToInt32(min.Value);
                                            int maxitm = Convert.ToInt32(max.Value);
                                            for (int i = minitm; i <= maxitm; i++)
                                                tempCmbBox.Items.Add(i);
                                        }
                                    }
                                }
                                if (tempCmbBox.Items.Count > 0)
                                    tempCmbBox.SelectedIndex = 0;
                                tempCmbBox.Bounds = new Rectangle(posX, posY, DropDownWidth(tempCmbBox) + 20, 100);
                                tempCmbBox.DropDownStyle = ComboBoxStyle.DropDownList;
                                tempCmbBox.DropDownWidth = DropDownWidth(tempCmbBox);
                                tempCmbBox.AutoSize = true;
                                if (item.Attribute("enabled") != null)
                                {
                                    if (item.Attribute("enabled").Value.ToLower().Equals("false"))
                                        tempCmbBox.Enabled = false;
                                }
                                else if (!groupEnabled)
                                    tempCmbBox.Enabled = false;
                                tempCmbBox.Parent = this;
                                posX += tempCmbBox.Width + 5;
                                resizeMainFrm(tempCmbBox);
                                break;
                        }
                    }

                    //Yeni grup için X sıfırla, Y koordinatını kaydır
                    posX = 5;
                    posY += 45;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void resizeMainFrm(Object o)
        {
            int posX = 0, posY = 0, width = 0, height = 0;
            if (o.GetType() == typeof(Label))
            {
                posX = ((Label)o).Left;
                posY = ((Label)o).Top;
                width = ((Label)o).Width;
                height = ((Label)o).Height;
            }
            else if (o.GetType() == typeof(TextBox))
            {
                posX = ((TextBox)o).Left;
                posY = ((TextBox)o).Top;
                width = ((TextBox)o).Width;
                height = ((TextBox)o).Height;
            }
            else if (o.GetType() == typeof(Button))
            {
                posX = ((Button)o).Left;
                posY = ((Button)o).Top;
                width = ((Button)o).Width;
                height = ((Button)o).Height;
            }
            else if (o.GetType() == typeof(ComboBox))
            {
                posX = ((ComboBox)o).Left;
                posY = ((ComboBox)o).Top;
                width = ((ComboBox)o).Width;
                height = ((ComboBox)o).Height;
            }
            else if (o.GetType() == typeof(TrackBar))
            {
                posX = ((TrackBar)o).Left;
                posY = ((TrackBar)o).Top;
                width = ((TrackBar)o).Width;
                height = ((TrackBar)o).Height;
            }
            else if (o.GetType() == typeof(NumericUpDown))
            {
                posX = ((NumericUpDown)o).Left;
                posY = ((NumericUpDown)o).Top;
                width = ((NumericUpDown)o).Width;
                height = ((NumericUpDown)o).Height;
            }
            //Kontrolün formun boyutunu aşıp aşmadığını kontrol et
            //Aşıyorsa formun boyutunu büyüt
            while ((posX + width) >= this.Width)
                this.Width += ((posX + width) - this.Width) + 30;
            while ((posY + height + 40) >= this.Height)
                this.Height += ((posY + height) - this.Height) + 50;
        }

        private int DropDownWidth(ComboBox myCombo)
        { 
            int maxWidth = 0;
            int temp = 0;
            Label label1 = new Label();

            foreach (var obj in myCombo.Items)
            {
                label1.Text = obj.ToString();
                temp = label1.PreferredWidth;
                if (temp > maxWidth)
                    maxWidth = temp;
            }
            label1.Dispose();
            return maxWidth;
        }

        public string dosyaYolu { get; set; }
    }
}
