using BrightIdeasSoftware;
using NotificationWindow;
using StreamNotifier.Interfaces;
using StreamNotifier.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;


namespace StreamNotifier
{

    public partial class frmMain : Form
    {

        #region Properties
        System.Windows.Forms.Timer tmrMonitor;
        int TIMERDELAY = 60 * 5 * 1000;
        PopupNotifierCollection NOTIFIER = new PopupNotifierCollection(3);
        List<ToolStripMenuItem> menuItems = new List<ToolStripMenuItem>();
        BackgroundWorker bwCheckStreams;
        BackgroundWorker bwCheckSelectedStreams;
        BackgroundWorker bwLoadApis;
        BackgroundWorker bwLoadStreams;
        delegate void SetTextCallback(string text);
        delegate void SetProgressBarCallback(int value);
        delegate void SetProgressBarStyleCallback(ProgressBarStyle style);
        delegate void SetControlEnabled(bool bol);
        delegate void SetSpinnerEnabled(bool value);



        #endregion

        #region Constructors
        public frmMain()
        {
            InitializeComponent();
            InitBackgroundWorkers();
            AppSettings.Load();

            notifyIcon1.Visible = true;
            this.Text = String.Format("StreamNotifier {0}", AppSettings.VERSION);

        }
        #endregion

        #region Workers
        private void InitBackgroundWorkers()
        {
            bwCheckStreams = new BackgroundWorker();
            bwCheckStreams.WorkerReportsProgress = false;
            bwCheckStreams.DoWork += bwCheckStreams_Dowork;
            bwCheckStreams.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwCheckStreams_RunWorkerCompleted);
            /*
            bwCheckIndividualStream = new BackgroundWorker();
            bwCheckIndividualStream.WorkerReportsProgress = false;
            bwCheckIndividualStream.DoWork += bwCheckIndividualStream_Dowork;
            bwCheckIndividualStream.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwCheckStreams_RunWorkerCompleted);
            */
            bwCheckSelectedStreams = new BackgroundWorker();
            bwCheckSelectedStreams.WorkerReportsProgress = false;
            bwCheckSelectedStreams.DoWork += bwCheckSelectedStreams_Dowork;
            bwCheckSelectedStreams.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwCheckSelectedStreams_RunWorkerCompleted);


            bwLoadApis = new BackgroundWorker();
            bwLoadApis.WorkerReportsProgress = false;
            bwLoadApis.DoWork += bwLoadApis_DoWork;
            bwLoadApis.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoadApis_AfterLoadPlugins);

            bwLoadStreams = new BackgroundWorker();
            bwLoadStreams.WorkerReportsProgress = false;
            bwLoadStreams.DoWork += bwLoadStreams_DoWork;
            bwLoadStreams.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoadStreams_AfterLoadingStreams);

        }

        private bool AnyBusyWorker()
        {
            return (bwCheckSelectedStreams.IsBusy || bwCheckStreams.IsBusy || bwLoadApis.IsBusy || bwLoadStreams.IsBusy);
        }

        //1) Load Apis
        private void bwLoadApis_DoWork(object sender, DoWorkEventArgs e)
        {
            FormEnable(false);
            SpinnerEnable(true);
            ProgressWindowSetStyle(ProgressBarStyle.Marquee);
            AppSettings.LoadApiAssembliesAndTypes();
            
        }
        private void bwLoadApis_AfterLoadPlugins(object sender, RunWorkerCompletedEventArgs e)
        {
            SpinnerEnable(false);
            ProgressWindowSetCaption("Loading Streams from config");
            bwLoadStreams.RunWorkerAsync();
        }
 

        //2) Load Streams
        private void bwLoadStreams_DoWork(object sender, DoWorkEventArgs e)
        {
            FormEnable(false);
            SpinnerEnable(true);
            ProgressWindowSetStyle(ProgressBarStyle.Marquee);
            ProgressWindowSetCaption("Loading Streams");
            AppSettings.LoadStreamsList();
        }
        private void bwLoadStreams_AfterLoadingStreams(object sender, RunWorkerCompletedEventArgs e)
        {
            SpinnerEnable(false);
            objectListView1.SetObjects(AppSettings.LiveStreams);
            ShowHideProgressWindow(false);
            ProgressWindowSetStyle(ProgressBarStyle.Blocks);
            FormEnable(true);
        }

        //POPUMENUITMEM : CheckSelectedStreams
        private void bwCheckSelectedStreams_Dowork(object sender, DoWorkEventArgs e)
        {
            ToolbarEnable(false);

            int i = 0;
            foreach (LiveStream L in AppSettings.LiveStreams)
            {
                if (L.CheckPending) { i++; }
            }
            ProgressWindowSetMax(i);
            CheckSelectedStreams();
        }
        private void bwCheckSelectedStreams_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ShowHideProgressWindow(false);
            ToolbarEnable(true);
            objectListView1.SetObjects(AppSettings.LiveStreams);
            MarkForChecking(false);
            //AppSettings.TmpStreams.Clear();
            objectListView1.Sort(colStatus);
            DisplayPopups();

        }

   
        //Check all Streams in Settings.
        private void bwCheckStreams_Dowork(object sender, DoWorkEventArgs e)
        {
            ToolbarEnable(false);
            doStreamsCheck();
        }
        private void bwCheckStreams_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ToolbarEnable(true);
            ShowHideProgressWindow(false);
            objectListView1.SetObjects(AppSettings.LiveStreams);
            objectListView1.Sort(colStatus);
            DisplayPopups();
        }

        #endregion Workers

        #region Methods

        private void MarkForChecking(bool selected)
        {
            if (selected)
            {
                foreach (LiveStream o in objectListView1.SelectedObjects)
                {
                    o.CheckPending = selected;
                }
            }
            else
            {
                foreach (LiveStream o in AppSettings.LiveStreams)
                {
                    o.CheckPending = selected;
                }
            }


        }


        public bool CheckInProgress()
        {
            return bwCheckSelectedStreams.IsBusy || bwCheckStreams.IsBusy;
        }


        //#UI
        private void InitListView()
        {

            //Colonne de description
            this.colTitle.WordWrap = true;

            //Colonne de type de stream
            this.colService.TextAlign = HorizontalAlignment.Center;
            this.colService.ImageGetter = delegate (object model)
            {
                LiveStream lv = (LiveStream)model;
                return AppSettings.ServicesIcons.Images[lv.StreamType];
            };


            //Colonne de Status
            this.colStatus.TextAlign = HorizontalAlignment.Center;
            this.colStatus.AspectToStringConverter = delegate (object model)
            {
                switch ((LiveStatus)model)
                {
                    case LiveStatus.Offline: return "Offline";
                    case LiveStatus.Online: return "Online";
                    case LiveStatus.Unknown: return "Unknown";
                }
                return "Unknown";
            };

            this.colStatus.ImageGetter = delegate (object model)
            {
                LiveStream lv = (LiveStream)model;
                switch (lv.OnAir)
                {
                    case LiveStatus.Offline: return "offline";
                    case LiveStatus.Online: return "online";
                    case LiveStatus.Unknown: return "unknown";
                }
                return "unknown";
            };

            //Colonne de Logo
            this.colAvatar.Width = 80;
            this.colAvatar.AspectToStringConverter = delegate (object model)
            {
                return String.Empty;
            };

            this.colAvatar.ImageGetter = delegate (object model)
            {
                LiveStream lv = (LiveStream)model;
                return AppSettings.StreamsLogos.Images[lv.PictureKey];
            };

            objectListView1.SetObjects(AppSettings.LiveStreams);
        }

        private void ShowHideProgressWindow(Boolean show, string text = "")
        {

            lblTxt.Text = text;
            //this.Enabled = (!CheckInProgress());

            progressBar1.Visible = show;
            progressBar1.MarqueeAnimationSpeed = (show) ? 100 : 0;
            progressBar1.Value = (show) ? 0 : progressBar1.Maximum;
        }

   

        private ContextMenuStrip BuildContextMenu(object e)
        {

            ContextMenuStrip Mymenu = new ContextMenuStrip(new Container());
            bool MultiSelect = objectListView1.SelectedObjects.Count > 1;

            if (e == null)
            {
                return Mymenu;
            }

            LiveStream lv = (LiveStream)e;

            ToolStripMenuItem mnu = new ToolStripMenuItem();
            mnu.Text = "Launch Livestreamer";
            mnu.Tag = lv.Streamurl;
            mnu.Click += LaunchLiveStreamerEvent;
            mnu.Enabled = !MultiSelect;
            Mymenu.Items.Add(mnu);

            mnu = new ToolStripMenuItem();
            mnu.Text = "View in Browser";
            mnu.Tag = lv.Streamurl;
            mnu.Click += ViewInBrowserMenuEvent;
            mnu.Enabled = !MultiSelect;
            Mymenu.Items.Add(mnu);

            mnu = new ToolStripMenuItem();
            mnu.Text = "Check Status";
            mnu.Click += CheckLiveStateEvent;
            mnu.Enabled = !CheckInProgress();
            Mymenu.Items.Add(mnu);

            return Mymenu;
        }



        //Monitors
        private void StartMonitoring()
        {

            startBtn.Enabled = false;
            stopBtn.Enabled = true;

            NOTIFIER.Clear();
            StreamCheck();

            tmrMonitor = new System.Windows.Forms.Timer();
            tmrMonitor.Interval = TIMERDELAY;
            tmrMonitor.Tick += OnTimerElapsed;
            //tmrMonitor.Elapsed += OnTimerElapsed;
            //tmrMonitor.AutoReset = true;
            tmrMonitor.Start();


        }
        private void StopMonitoring()
        {
            tmrMonitor.Stop();
            tmrMonitor.Dispose();
            startBtn.Enabled = true;
            stopBtn.Enabled = false;

        }

        //Checkers
        private void StreamCheck()
        {
            ProgressWindowSetMax(AppSettings.LiveStreams.Count);
            ShowHideProgressWindow(true, "Checking for streams....");
            bwCheckStreams.RunWorkerAsync();

        }
        private void doStreamsCheck()
        {
            ProgressWindowSetProgress(0);

            foreach (LiveStream o in AppSettings.LiveStreams)
            {
                ProgressWindowSetProgress(progressBar1.Value + 1);

                CheckIndividualStream(o);
            }

        }
        private void CheckIndividualStream(LiveStream o)
        {
            int tempo = int.Parse(AppSettings.Get("retrydelay"));
            int retrymax = int.Parse(AppSettings.Get("retrycount"));

            int trials = 0;
            do
            {
                ProgressWindowSetCaption(String.Format("Check for {0} ({1})...", o.Displayname, trials + 1));

                if (o.GoneOnline())
                {
                    o.ShowPopup = true;
                };

                if (!o.OnAir.Equals(LiveStatus.Unknown)) break;

                trials++;
                Thread.Sleep(tempo);
            } while ((trials < retrymax) && (o.OnAir.Equals(LiveStatus.Unknown)));
        }

        private void CheckSelectedStreams()
        {
            ProgressWindowSetProgress(0);
            foreach (LiveStream o in AppSettings.LiveStreams)
            {
                if (o.CheckPending)
                {
                    o.CheckPending = false;
                    ProgressWindowSetProgress(progressBar1.Value+1);
                    CheckIndividualStream(o);
                }
            }
        }

        //POPUPS
        private PopupNotifier CreatePopup(LiveStream o)
        {
            PopupNotifier win = new PopupNotifier();

            win.BodyColor = Color.FromArgb(34, 34, 34);
            win.ContentColor = Color.White;
            win.TitleColor = Color.SteelBlue;
            win.ShowHeader = false;
            win.ShowGrip = false;
            win.GradientPower = 20;
            win.TitleText = o.Displayname + " is streaming ! - StreamNotifier";
            win.Link = o.Streamurl;
            win.Delay = 5000;
            win.ContentText = o.Title;
            win.Image = AppSettings.StreamsLogos.Images[o.PictureKey];//Image.FromFile(o.Picture); //Avatar !
            win.Click += new System.EventHandler(LaunchLiveStreamerEvent);

            return win;
        }
        private void DisplayPopups()
        {
            foreach (LiveStream o in AppSettings.LiveStreams)
            {
                if (o.ShowPopup)
                {
                    NOTIFIER.Add(CreatePopup(o));
                    o.ShowPopup = false;
                }

            }

            if (NOTIFIER.Count > 0)
            {
                NOTIFIER.Popup();
            }


        }


        private void ViewInBrowser(string url)
        {
            Process.Start(url);
        }
        private void LaunchLiveStreamer(string url)
        {
            Process.Start("livestreamer", url + " best");
        }

        #endregion

        #region Delegates

        private void ProgressWindowSetMax(int max)
        {
            if (progressBar1.GetCurrentParent().InvokeRequired)
            {
                SetProgressBarCallback d = new SetProgressBarCallback(ProgressWindowSetMax);
                this.Invoke(d, new object[] { max });
            }
            else
            {
                progressBar1.Maximum = max;
            }
        }

        private void ProgressWindowSetStyle(ProgressBarStyle style)
        {

            if (progressBar1.GetCurrentParent().InvokeRequired)
            {
                SetProgressBarStyleCallback d = new SetProgressBarStyleCallback(ProgressWindowSetStyle);
                this.Invoke(d, new object[] { style });
            }
            else
            {
                progressBar1.Style = style;
            }

        }

        private void ProgressWindowSetCaption(string text)
        {
            if (lblTxt.GetCurrentParent().InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ProgressWindowSetCaption);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                lblTxt.Text = text;
            }


        }

        private void ProgressWindowSetProgress(int value)
        {

            if (progressBar1.GetCurrentParent().InvokeRequired)
            {
                SetProgressBarCallback d = new SetProgressBarCallback(ProgressWindowSetProgress);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                progressBar1.Value = value;
            }

        }

        private void FormEnable(bool value)
        {

            if (this.InvokeRequired)
            {
                SetControlEnabled d = new SetControlEnabled(FormEnable);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                this.Enabled = value;
            }

        }

        private void ToolbarEnable(bool value)
        {

            if (toolStrip1.InvokeRequired)
            {
                SetControlEnabled d = new SetControlEnabled(ToolbarEnable);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                toolStrip1.Enabled = value;
            }

        }

        private void SpinnerEnable(bool value)
        {

            if (InvokeRequired)
            {
                SetSpinnerEnabled d = new SetSpinnerEnabled(SpinnerEnable);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                LC.Active = value;
                LC.Visible = value;

            }
        }

        #endregion

        #region UI

            //TIMER
        private void OnTimerElapsed(object sender, EventArgs e)
        {
            StreamCheck();
        }

        //BUTTONS
        private void startBtn_Click(object sender, EventArgs e)
        {
            StartMonitoring();
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            StopMonitoring();
        }
        private void refreshBtn_Click_1(object sender, EventArgs e)
        {
            StreamCheck();
        }


        private void addBtn_Click(object sender, EventArgs e)
        {
            frmAddStream frm = new frmAddStream();
            frm.ShowDialog();
            objectListView1.SetObjects(AppSettings.LiveStreams);
        }

        private void clearCacheBtn_Click(object sender, EventArgs e)
        {
            objectListView1.ClearObjects();
            AppSettings.ClearCaches();
            ShowHideProgressWindow(true, "Loading Streams from config");
            bwLoadStreams.RunWorkerAsync();
        }

        //SYSTRAY ICON
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                //notifyIcon1.ShowBalloonTip(500);
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();

            }


        }

        //POPUP MENU
        private void LaunchLiveStreamerEvent(object sender, System.EventArgs e)
        {
            if (sender is PopupNotifier)
            {
                LaunchLiveStreamer((sender as PopupNotifier).Link);
            }
            else if (sender is ToolStripMenuItem)
            {
                LaunchLiveStreamer((string)(sender as ToolStripMenuItem).Tag);
            }
        }
        private void CheckLiveStateEvent(object sender, System.EventArgs e)
        {
             MarkForChecking(true);
             ShowHideProgressWindow(true, "Checking for selected stream(s)....");
             bwCheckSelectedStreams.RunWorkerAsync();
        }

     

        private void ViewInBrowserMenuEvent(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ViewInBrowser((string)(sender as ToolStripMenuItem).Tag);
            }
        }
        private void closeStreamNotifierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        //FORM
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            AppSettings.ServicesIcons.ImageSize = new Size(32, 32);
            AppSettings.StreamsLogos.ImageSize = new Size(80, 80);
            AppSettings.StreamsLogos.ColorDepth = ColorDepth.Depth32Bit;
            AppSettings.StreamsLogos.Images.Add(AppSettings.Default_picturekey,(Image) Properties.Resources.ResourceManager.GetObject(AppSettings.Default_picturekey));


            InitListView();

            //Start by looding apis, then load Streams
            ShowHideProgressWindow(true, "Loading Streams API");
            bwLoadApis.RunWorkerAsync();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            AppSettings.SaveStreamList();
        }


        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AnyBusyWorker())
            {
                e.Cancel = true;
            }

        }

        //LISTVIEW
        private void objectListView1_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            e.MenuStrip = BuildContextMenu(e.Model);
        }
        private void objectListView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ObjectListView olv = (ObjectListView)sender;
                if (olv.SelectedObject == null) { return; }

                LiveStream lv = (LiveStream)olv.SelectedObject;
                LaunchLiveStreamer(lv.Streamurl);
            }
            catch (Exception)
            {


            }
        }


        #endregion

       
    }


}
