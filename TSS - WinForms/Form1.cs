using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TSS.UniverseLogic;
using TSS.WorkHelpers;

namespace TSS.WinForms
{
    public partial class Form1 : Form
    {
        Universe universe;
        TextBoxOutputManager textBoxOutputManager;
        bool isWorking = false;

        public Form1()
        {
            InitializeComponent();
            richTextBox1.Text = @"";
            label1.Text = @"";
            textBox1.Text = @"120";
            textBox2.Text = @"35";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int width, height;
            UniverseParser.LoadFromFile();
            try
            {
                width = Convert.ToInt32(textBox1.Text);
                height = Convert.ToInt32(textBox2.Text);
            }
            catch
            {
                MessageBox.Show(@"Wrong field value!");
                return;
            }
            UniverseParser.LoadFromFile();
            universe = new Universe(width, height);
            textBoxOutputManager = new TextBoxOutputManager(universe, richTextBox1, label1);


            InitConfigs();
            universe.GenerateCells(UniverseConsts.DefGenerateCells);
            textBoxOutputManager.StartSimulation();
            isWorking = true;

            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            label1.Text = @"";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBoxOutputManager.CalcOutputRichTextBox();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBoxOutputManager == null)
                return;
            if (isWorking)
            {
                textBoxOutputManager.PauseSimulation();
                isWorking = false;
            }
            else
            {
                textBoxOutputManager.StartSimulation();
                isWorking = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Really want to destroy universe?", "Universe destroying.", 
                MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
                return;

            if (universe != null)
            {
                universe.Dispose();
            }
            button1.Enabled = true;
            button2.Enabled = false;
            //button3.Enabled = false;
            button5.Enabled = false;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            richTextBox1.Text = @"";
            label1.Text = @"";

            // MessageBox.Show(UniverseConsts.VarsToString());
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            ChangeConfigs();
        }

        void ChangeConfigs()
        {
            string path = Environment.CurrentDirectory + @"\universe_configs.txt";
            Process proc = new Process();
            proc.StartInfo.FileName = path;
            proc.Start();
            while (!proc.HasExited)
                System.Threading.Thread.Sleep(1000);
            try
            {
                UniverseParser.LoadFromFile();

            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                ChangeConfigs();
                return;
            }

            //    textBoxOutputManager.BeginInvoke(InitConfigs);
            InitConfigs();
        }

        void InitConfigs()
        {
            if (textBoxOutputManager == null || textBoxOutputManager.IsDisposed())
                return;
            
            universe.SetFoodForTick(UniverseConsts.DefFoodForTick);
            universe.SetPoisonForTick(UniverseConsts.DefPoisonForTick);
            universe.SetMaxCellCount(UniverseConsts.MaxCellCount);

            string[] arr = UniverseConsts.DefFoodPlace.Replace(@" ", @"").Trim().Split(',');
            universe.SetFoodPlace(Convert.ToInt32(arr[0]),
                Convert.ToInt32(arr[1]),
                Convert.ToInt32(arr[2]),
                Convert.ToInt32(arr[3]));

            arr = UniverseConsts.DefPoisonPlace.Replace(@" ", @"").Trim().Split(',');
            universe.SetPoisonPlace(Convert.ToInt32(arr[0]),
                Convert.ToInt32(arr[1]),
                Convert.ToInt32(arr[2]),
                Convert.ToInt32(arr[3]));

        
            textBoxOutputManager.SetTicksPerSecond(UniverseConsts.TicksPerSecond);
            textBoxOutputManager.SetFramesPerSecond(UniverseConsts.FramesPerSecond);
            textBoxOutputManager.SetPriority(System.Threading.ThreadPriority.Highest);
            textBoxOutputManager.SetThreadMood(UniverseConsts.MultiThreading);
            //textBoxOutputManager.SetThreadMood(UniverseConsts.MultiThreading);
        }

        private void richTextBox1_SizeChanged(object sender, EventArgs e)
        {
            if (textBoxOutputManager == null || textBoxOutputManager.IsDisposed())
                return;
            textBoxOutputManager.CalcOutputRichTextBox();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (universe == null || universe.IsDisposed())
                return;
            universe.Dispose();
        }
    }
}
