using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TSS.UniverseLogic;
using System.Drawing;


namespace TSS.WinForms
{

    class TextBoxOutputManagerOld 
    {
        int ticksPause, framesPause;
        int ticksPerFrame;
        float ticksPerSec, framesPerSec;
        Universe universe;
        RichTextBox outputRichTextBox;
        Label infoLabel;
        int width, height;
        string status;
        Timer updateTimer;
        bool onWork = false;
        Color border = Color.Black;
        string fieldStr;
        bool disposed;
        Task universeUpdateThread;
        bool asyncThreadMood;
        

        public TextBoxOutputManagerOld(Universe universe, RichTextBox outputRichTextBox, Label infoLabel)
        {
            disposed = false;
            width = universe.GetWidth();
            height = universe.GetHeight();
            this.universe = universe;
            this.outputRichTextBox = outputRichTextBox;
            CalcOutputRichTextBox();
            this.infoLabel = infoLabel;
            fieldStr = @"";
            fieldStr += '+';
            for (int i = 0; i < width; i++)
            {
                fieldStr += '-';
            }
            fieldStr += '+';
            status = @"stoped";

            updateTimer = new Timer();
            updateTimer.Stop();
            updateTimer.Tick += delegate
            {
                if (asyncThreadMood)
                    OnTimerMultiThread();
                else
                    TimerLoop();
            };
            SetThreadMood(false);
            SetTicksPerSecond(2);
            SetFramesPerSecond(2);
        }

        //public void BeginInvoke(Action method)
        //{
        //    InvokedMethods.Add(method);
        //}

        public bool IsDisposed()
        {
            return disposed;
        }

        public int CalcOutputRichTextBox()
        {
            Font font;
            int countH=1;
            outputRichTextBox.Invoke(new Action(() =>
            {
                CalcChars(width + 2, outputRichTextBox.Width, outputRichTextBox.Height, out font, out countH);
                outputRichTextBox.Font = font;
                outputRichTextBox.ScrollBars = RichTextBoxScrollBars.None;
                outputRichTextBox.ReadOnly = true;
                outputRichTextBox.Enabled = false;
            }));
            return countH;
        }

        public static void CalcChars(int count, int width, int height, out Font font, out int countH)
        {
            RichTextBox rtbLoc = new RichTextBox();

            rtbLoc.ScrollBars = RichTextBoxScrollBars.None;
            rtbLoc.Width = width;
            rtbLoc.Height = height;
            font = CalcFont(count, rtbLoc);
            rtbLoc.Font = font;

            countH = rtbLoc.Height / (font.Height + 1);

        }

        static Font CalcFont(int count, RichTextBox rtb)
        {
            float size = 8;

            rtb.Clear();
            for (int i = 0; i < count; i++)
                rtb.AppendText("Q");


            rtb.Font = new Font(@"Courier New", size, FontStyle.Bold);
            int posY = rtb.GetPositionFromCharIndex(0).Y;
            int countDec = count - 1;
            while (rtb.GetPositionFromCharIndex(countDec).Y == posY)
            {
                rtb.Font = new Font(@"Courier New", size, FontStyle.Bold);
                //rtbLoc.Refresh();
                size += (float)0.1;
            }
            size -= (float)(size / 20);

            return new Font(@"Courier New", size, FontStyle.Bold);
        }


        public RichTextBox GetOutputRichTextBox()
        {
            return outputRichTextBox;
        }

        public Label GetInfoLabel()
        {
            return infoLabel;
        }

        //void AppendText(string text, Color color)
        //{
        //    outputRichTextBox.SelectionStart = outputRichTextBox.TextLength;
        //    outputRichTextBox.SelectionLength = 0;

        //    outputRichTextBox.SelectionColor = color;
        //    outputRichTextBox.AppendText(text);
        //}

        void AppendText(string text)
        {
            outputRichTextBox.Invoke(new Action(() =>
            {
                outputRichTextBox.AppendText(text);
            }));
        }


        //void AppendText(char c, Color color)
        //{
        //    AppendText(c.ToString(), color);
        //}

        //void AppendText(char c)
        //{
        //    AppendText(c.ToString());
        //}

        void SetColor(int startInd, int length, Color color)
        {
            outputRichTextBox.Invoke(new Action(() =>
            {
                outputRichTextBox.SelectionStart = startInd;
                outputRichTextBox.SelectionLength = length;

                outputRichTextBox.SelectionColor = color;
            }));

        }

        void DrawPicture()
        {
            if (UniverseConsts.DrawMoveDirections)
                DrawPicturePro();
            else
                DrawPictureDef();
        }

        void DrawPictureDef()
        {
            SuspendControlUpdate.Suspend(outputRichTextBox);
            outputRichTextBox.Clear();
            char cell = 'O';
            char deadCell = '#', food = '*', empty = ' ', poison = '*';
            int[][] desc = universe.GetAllDescriptors();

            string outputStr = fieldStr + '\n';
            for (int j = 0; j < height; j++)
            {
                outputStr += '|';
                for (int i = 0; i < width; i++)
                {
                    int descriptor = desc[i][j];
                    if (descriptor == 0)
                    {
                        outputStr += empty;
                    }
                    else if (descriptor == -1)
                    {
                        outputStr += food;
                    }
                    else if (descriptor == -2)
                    {
                        outputStr += deadCell;
                    }
                    else if (descriptor == -3)
                    {
                        outputStr += poison;
                    }
                    else
                    {
                        outputStr += cell;
                    }
                }
                outputStr += "|\n";

            }

            outputStr += fieldStr;
            AppendText(outputStr);

            SetColor(0, fieldStr.Length, border);

            int charNum = fieldStr.Length + 1;
            for (int j = 0; j < height; j++)
            {
                SetColor(charNum, 1, border);
                charNum++;
                for (int i = 0; i < width; i++)
                {
                    int descriptor = desc[i][j];
                    if (descriptor == -1)
                    {
                        SetColor(charNum, 1, Color.Green);
                    }
                    else if (descriptor == -2)
                    {
                        SetColor(charNum, 1, Color.Gray);
                    }
                    else if (descriptor == -3)
                    {
                        SetColor(charNum, 1, Color.DarkOrange);
                    }
                    else if (descriptor >= 100)
                    {
                        SetColor(charNum, 1, Color.FromArgb(descriptor));
                    }
                    charNum++;
                }
                SetColor(charNum, 1, border);
                charNum += 2;


            }
            SetColor(charNum, fieldStr.Length, border);
            SuspendControlUpdate.Resume(outputRichTextBox);

        }

        void DrawPicturePro()
        {
            SuspendControlUpdate.Suspend(outputRichTextBox);
            outputRichTextBox.Clear();
            //char cellStand = '☻', cellUp = '▲', cellDown = '▼', cellLeft = '◄', cellRight = '►'; 
            //char deadCell = '❖', food = '░', empty=' ';
            char cellStand = 'O', cellUp = '∧', cellDown = '∨', cellLeft = '<', cellRight = '>';
            char deadCell = '#', food = '*', empty = ' ', poison = '*';
            DescAndMoveDir[][] descAndMD = universe.GetAllDescriptorsAndMoveDisp();


            string outputStr = fieldStr + '\n';
            for (int j = 0; j < height; j++)
            {
                outputStr += '|';
                for (int i = 0; i < width; i++)
                {
                    int descriptor = descAndMD[i][j].desc;
                    if (descriptor == 0)
                    {
                        outputStr += empty;
                    }
                    else if (descriptor == -1)
                    {
                        outputStr += food;
                    }
                    else if (descriptor == -2)
                    {
                        outputStr += deadCell;
                    }
                    else if (descriptor == -3)
                    {
                        outputStr += poison;
                    }
                    else
                    {
                        char cell = 'O';
                        switch (descAndMD[i][j].moveDir)
                        {
                            case MoveDirection.up:
                                cell = cellUp;
                                break;

                            case MoveDirection.down:
                                cell = cellDown;
                                break;

                            case MoveDirection.left:
                                cell = cellLeft;
                                break;

                            case MoveDirection.right:
                                cell = cellRight;
                                break;

                            default:
                                cell = cellStand;
                                break;
                        }
                        outputStr += cell;
                    }
                }
                outputStr += "|\n";

            }

            outputStr += fieldStr;
            AppendText(outputStr);

            SetColor(0, fieldStr.Length, border);

            int charNum = fieldStr.Length + 1;
            for (int j = 0; j < height; j++)
            {
                SetColor(charNum, 1, border);
                charNum++;
                for (int i = 0; i < width; i++)
                {
                    int descriptor = descAndMD[i][j].desc;
                    if (descriptor == -1)
                    {
                        SetColor(charNum, 1, Color.Green);
                    }
                    else if (descriptor == -2)
                    {
                        SetColor(charNum, 1, Color.Gray);
                    }
                    else if (descriptor == -3)
                    {
                        SetColor(charNum, 1, Color.DarkOrange);
                    }
                    else
                    {
                        SetColor(charNum, 1, Color.FromArgb(descriptor));
                    }
                    charNum++;
                }
                SetColor(charNum, 1, border);
                charNum += 2;


            }
            SetColor(charNum, fieldStr.Length, border);

            SuspendControlUpdate.Resume(outputRichTextBox);
        }


        //is OLD
        //void DrawPictureDef()
        //{
        //    SuspendControlUpdate.Suspend(outputRichTextBox);
        //    outputRichTextBox.Clear();
        //    //char cellStand = '☻', cellUp = '▲', cellDown = '▼', cellLeft = '◄', cellRight = '►'; 
        //    //char deadCell = '❖', food = '░', empty=' ';
        //    char cell = 'O';
        //    char deadCell = '#', food = '*', empty = ' ';
        //    int[][] desc = universe.GetAllDescriptors();
        //    int heightDec = height - 1;

        //    AppendText('+', border);
        //    for (int i = 0; i < width; i++)
        //    {
        //        AppendText('-', border);
        //    }
        //    AppendText("+\n", border);


        //    for (int j = 0; j < height; j++)
        //    {
        //        AppendText('|', border);
        //        for (int i = 0; i < width; i++)
        //        {
        //            int descriptor = desc[i][j];
        //            if (descriptor == 0)
        //            {
        //                AppendText(empty);
        //            }
        //            else if (descriptor == -1)
        //            {
        //                AppendText(food, Color.Green);
        //            }
        //            else if (descriptor == -2)
        //            {
        //                AppendText(deadCell, Color.Gray);
        //            }
        //            else
        //            {
        //                AppendText(cell, Color.FromArgb(descriptor));
        //            }
        //        }
        //        AppendText("|\n", border);

        //    }

        //    AppendText('+', border);
        //    for (int i = 0; i < width; i++)
        //    {
        //        AppendText('-', border);
        //    }
        //    AppendText('+', border);


        //    SuspendControlUpdate.Resume(outputRichTextBox);



        //}

        void WriteInfo()
        {
            string info = @"Info";
            long uTick = universe.GetTicksCount();

            info += string.Format("\nWidth: {0};", width);
            info += string.Format("\nHeight: {0};", height);
            info += string.Format("\nStatus: {0};", status);
            info += string.Format("\nTick number: {0};", uTick);

            object[] mostFit = universe.GetMostFitCell();
            if (mostFit == null)
                info += "\nNone cell;";
            else
            {
                Cell cell = (Cell)mostFit[0];
                int cellsCount = (int)mostFit[1];
                info += string.Format("\nCells count: {0};\nMost fit genome({1} cells):", universe.GetCellsCount(), cellsCount);

                info += string.Format("\n    descriptor: {0};\n    hunger = {1};\n    aggression: {2};\n    friendly: {3};\n    Reproduction: {4};\n    poisonAddiction: {5}",
                    cell.GetDesc(),
                    cell.GetGenome().GetHunger(),
                    cell.GetGenome().GetAggression(),
                    cell.GetGenome().GetFriendly(),
                    cell.GetGenome().GetReproduction(),
                    cell.GetGenome().GetPoisonAddiction()
                    );


                border = Color.FromArgb(cell.GetDesc());
            }

            infoLabel.Invoke(new Action(() =>
            {
                infoLabel.Text = info;
            }));
        }

        public void Dispose()
        {
            PauseSimulation();
            universe = null;
            outputRichTextBox = null;
            infoLabel = null;
            status = null;
            if (updateTimer != null)
            {
                updateTimer.Stop();
                updateTimer.Dispose();
                updateTimer = null;
            }
            onWork = false;
            if (universeUpdateThread != null)
            {
                try
                {
                    universeUpdateThread.Dispose();
                    universeUpdateThread = null;
                }
                catch { }
            }
            fieldStr = null;
            disposed = true;
        }

        public void SetTicksPerSecond(float value)
        {
            if (value <= 0)
                return;
            ticksPause = (int)(1 / value * 1000);
            ticksPerSec = value;
            ticksPerFrame = (int)(ticksPerSec / framesPerSec);
            if (ticksPerFrame <= 0)
                ticksPerFrame = 1;
        }
        public void SetFramesPerSecond(float value)
        {
            if (value <= 0)
                return;
            framesPause = (int)(1 / value * 1000);
            updateTimer.Interval = (int)(1 / value * 1000);
            framesPerSec = value;
            ticksPerFrame = (int)(ticksPerSec / framesPerSec);
            if (ticksPerFrame <= 0)
                ticksPerFrame = 1;

        }

        public float GetTicksPerSecond()
        {
            return (float)(1 / ticksPause * 1000);
        }

        public Universe GetUniverse()
        {
            return universe;
        }

        void TimerLoop()
        {

            if (!onWork)
            {
                if (universe.IsDisposed())
                {
                    Dispose();
                    return;
                }
                //foreach (Action func in InvokedMethods.ToArray())
                //{
                //    func();
                //    InvokedMethods.Remove(func);
                //}
                onWork = true;
                for(int i = 0; i<ticksPerFrame; i++)
                    universe.DoUniverseTick();
                DrawPicture();
                WriteInfo();
                onWork = false;
            }

        }

        void OnTimerMultiThread()
        {
            onWork = true;
        }

        class ThreadTechnique
        {
            TextBoxOutputManager om;
            Task universeUpdThread, screenUpdThread;
            bool blockedUniverseUpdThread, blockedScreenUpdThread;

            public ThreadTechnique(TextBoxOutputManager textBoxOutputManager, System.Threading.ThreadPriority threadPriority)
            {
                blockedUniverseUpdThread = false;
                blockedScreenUpdThread = false;
                om = textBoxOutputManager;
                universeUpdThread = new Task(() =>
                {
                    System.Threading.Thread.CurrentThread.Priority = threadPriority;
                    UniverseUpdateLoop();
                });
                universeUpdThread.Start();
                screenUpdThread = new Task(() =>
                {
                    System.Threading.Thread.CurrentThread.Priority = threadPriority;
                    UniverseUpdateLoop();
                });
                screenUpdThread.Start();
            }

            void UniverseUpdateLoop()
            {
                try
                {
                    while (universeUpdThread != null)
                    {

                        if (om.universe.IsDisposed())
                        {
                            om.Dispose();
                            return;
                        }

                        om.universe.DoUniverseTick();

                        blockedScreenUpdThread = false;
                        while(blockedUniverseUpdThread)
                            System.Threading.Thread.Sleep(5);
                        System.Threading.Thread.Sleep(om.ticksPause);

                    }
                }
                catch { }
            }

            void ScreenUpdateLoop()
            {
                try
                {
                    System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
                    while (screenUpdThread != null)
                    {
                        if (om.universe.IsDisposed())
                        {
                            om.Dispose();
                            return;
                        }

                        blockedUniverseUpdThread = true;
                        blockedScreenUpdThread = true;
                        while (blockedScreenUpdThread)
                            System.Threading.Thread.Sleep(5);



                        blockedUniverseUpdThread = false;
                        System.Threading.Thread.Sleep(om.framesPause);

                    }
                }
                catch { }
            }
        }

        

        public void SetThreadMood(bool multiThreading)
        {
            asyncThreadMood = multiThreading;
            bool wasEn = updateTimer.Enabled;
            PauseSimulation();
            if (wasEn)
                StartSimulation();
        }

        public void StartSimulation()
        {
            if (asyncThreadMood)
            {
                //universeUpdateThread = new Task(UniverseUpdateThread);
                universeUpdateThread.Start();
            }
            updateTimer.Start();
            status = @"running";
        }

        public void PauseSimulation()
        {
            updateTimer.Stop();
            if (universeUpdateThread != null)
            {
                Task ts = universeUpdateThread;
                universeUpdateThread = null;
                ts.Wait();
                ts.Dispose();
            }
            status = @"paused";

        }




    }

    public static class SuspendControlUpdate
    {
        private const int WM_SETREDRAW = 0x000B;

        public static void Suspend(Control control)
        {
            Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        public static void Resume(Control control)
        {
            // Create a C "true" boolean as an IntPtr
            IntPtr wparam = new IntPtr(1);
            Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);

            control.Invalidate();
        }
    }
}
