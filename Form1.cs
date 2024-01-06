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

namespace Thermo_Control_App
{
    public partial class Form1 : Form
    {
        SerialPort arduinoPort = new SerialPort();
        string recievedData;

        int x = 0;
        int y = 0;
        bool isPauseBtn_Clicked = false;
        Random gene = new Random();
        double celsiusValue = 10.3;
        double fahrenheitValue = 32;
        double minCelsiusValue;
        double maxCelsiusValue;
        public Form1()
        {
            InitializeComponent();

            //Getting the serial ports for the ComboBox

            string[] ports = SerialPort.GetPortNames();
            portsBox.Items.Clear();
            foreach (string comport in ports)
            {
                portsBox.Items.Add(comport);
            }
            deconnectBtn.Enabled = false;
            resetBtn.Enabled = false;
            stopBtn.Enabled = false;
            statusText.Text += "Disconnected";

            //Deactivate all the tab pages before connecting to the arduino

            tabPage2.Enabled = false;
            tabPage3.Enabled = false;
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            //Connecting the arduino

            try
            {
                string portName = portsBox.SelectedItem as string;
                arduinoPort.PortName = portName;
                arduinoPort.BaudRate = 9600;
                arduinoPort.Open();
                statusText.Text = "Status: Connected";
                deconnectBtn.Enabled = true;
                connectBtn.Enabled = false;
            }

            catch (Exception)
            {
                MessageBox.Show("Entrer un port valid pour connecter la carte");
            }
            //Enabling tab pages
            tabPage2.Enabled = true;
    
        }
        private void deconnectBtn_Click(object sender, EventArgs e)
        {
            //Deconnecting the arduino
            try
            {      
                connectBtn.Enabled = true;
                deconnectBtn.Enabled = false;
                statusText.Text = "Status: Disconnected";
                arduinoPort.Close();  
                
            }
            catch (Exception)
            {
                MessageBox.Show("Un erreur");
            }
            //Disabling the tab pages
            tabPage2.Enabled = false;
            tabPage3.Enabled = false;
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
           
            //Deactivate the connection tab
            tabPage1.Enabled = false;

            chartTimer.Interval = 1000;
            chartTimer.Start();
            tabPage3.Enabled = true;
            stopBtn.Enabled = true;
            startBtn.Enabled = false;
            pauseBtn.Enabled = true;
            timeInterval.Enabled = false;
            resetBtn.Enabled = false;
            timeInterval.Value = 1000;
            timerInervalLabel.Text = timeInterval.Value + "ms";       
            timeAxisLabel.Text = "Time (" + timeInterval.Value + "ms)";
        }
        private void stopBtn_Click(object sender, EventArgs e)
        {
          
            //Activate the connection tab
            tabPage1.Enabled = true;
            tabPage3.Enabled = false;
            startBtn.Enabled = true;
            stopBtn.Enabled = false;
            chart1.Series["Temp"].Points.Clear();
            chartTimer.Stop();
            pauseBtn.Text = "Pause";
            isPauseBtn_Clicked = false;
            x = 0;
        }
        double CelsiusToFah(double cValue)
        {
            return ((cValue * 1.8) + 32);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            recievedData = arduinoPort.ReadLine();
            celsiusValue = y;
    
            if (y <= minValue.Value || y >= maxValue.Value)
            {
                tempValueLabel.ForeColor = Color.Red;
            }
            else
            {
                tempValueLabel.ForeColor = Color.Black;
            }
            fahrenheitValue = CelsiusToFah(celsiusValue);
            tempValueLabel.Text = "C= " + celsiusValue + "                F= " + fahrenheitValue;    
           
        }

        private void chartTimer_Tick(object sender, EventArgs e)
        {
            y = gene.Next(20, 26);
            recievedData = arduinoPort.ReadLine();
            celsiusValue = y;

            if (y <= minValue.Value || y >= maxValue.Value)
            {
                tempValueLabel.ForeColor = Color.Red;
            }
            else
            {
                tempValueLabel.ForeColor = Color.Black;
            }
            fahrenheitValue = CelsiusToFah(celsiusValue);
            tempValueLabel.Text = "C= " + celsiusValue + "                F= " + fahrenheitValue;

            if (x == 0)
            {
                minCelsiusValue = y;

            }
            else
            {

            }

                chartTimer.Interval = timeInterval.Value;
            chart1.Series["Temp"].Points.AddXY(x, y);
            int start = x;
            if (y <= minValue.Value || y >= maxValue.Value)
            {
                for (int i = start; i < chart1.Series["Temp"].Points.Count; i++)
                {
                    chart1.Series["Temp"].Points[i].Color = Color.Red;
                }                
            }
            //Get the min and max value



            x++;            
        }
        private void resetBtn_Click(object sender, EventArgs e)
        {
            x = 0;
            chart1.Series["Temp"].Points.Clear();
            if(isPauseBtn_Clicked)
            {
                pauseBtn.Text = "Pause";
            }
            chartTimer.Start();
            resetBtn.Enabled = false;
            pauseBtn.Enabled = true ;
            timeInterval.Enabled = false;
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            
            if (!isPauseBtn_Clicked)
            {
                pauseBtn.Text = "Resume";
                chartTimer.Stop();
                isPauseBtn_Clicked = true;
                resetBtn.Enabled = true;
                timeInterval.Enabled = true;
            }
            else
            {
                pauseBtn.Text = "Pause";
                chartTimer.Start();
                timer1.Start();
                isPauseBtn_Clicked = false;
                resetBtn.Enabled = false;
                timeInterval.Enabled = false;
            }
        }

        private void timeInterval_Scroll(object sender, EventArgs e)
        {
            timerInervalLabel.Text = timeInterval.Value + "ms";
            pauseBtn.Enabled = false;
            timeAxisLabel.Text = "Time (" + timeInterval.Value + "ms)";
            
        }
    }
}
