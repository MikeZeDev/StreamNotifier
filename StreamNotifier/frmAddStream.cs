using StreamNotifier.Interfaces;
using StreamNotifier.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace StreamNotifier
{
    public partial class frmAddStream : Form
    {

        #region Properties

        private bool urlvalidated = false;
        private bool streamexisting = false;
        private string URL = "";
        private string SERVICE = "";
        private LiveStream LS;

        private BackgroundWorker bwCheckurl;
        private BackgroundWorker bwCheckStream;
        delegate void SetEnabled(bool value);
        delegate void SetSpinnerEnabled(bool value);
         
        Action DoClearCombo;
        Action DoFillCombo;
        Action DoUpdateUI;

        List<string> Apis = new List<string>();

        
  
        #endregion

        #region Constructors
        public frmAddStream()
        {
            InitializeComponent();
            InitBackgroundWorkers();
            UpdateUI();
        }
        #endregion


        #region Workers
        private void InitBackgroundWorkers()
        {
            bwCheckurl = new BackgroundWorker();
            bwCheckurl.DoWork += bwCheckurl_DoWork;
            bwCheckurl.RunWorkerCompleted += bwCheckurl_Finished;

            bwCheckStream = new BackgroundWorker();
            bwCheckStream.DoWork += bwCheckStream_DoWork;
            bwCheckStream.RunWorkerCompleted += bwCheckStream_Finished;


            DoUpdateUI = UpdateUI;
            DoClearCombo = ClearCombo;
            DoFillCombo = FillCombo;
        }


        private void bwCheckurl_DoWork(object sender, DoWorkEventArgs e)
        {
            urlvalidated = false;
            streamexisting = false;
            LS = null;

            FormEnable(false);
            SpinnerEnable(true);
            ClearCombo();

            Apis.Clear();
            Apis = AppSettings.WhoCanHandleThisUrl(textBox1.Text);

        }
        private void bwCheckurl_Finished(object sender, RunWorkerCompletedEventArgs e)
        {
            FormEnable(true);
            SpinnerEnable(false);
            FillCombo();

            int i = Apis.Count;
            Apis.Clear();
            urlvalidated = (i > 0);

            UpdateUI();

            //Si on a uniquement un seul plugin reconnu pour l'url, alors on regarde tout de suite si le stream existe (l'url valide ne veut pas dire qu'un channel/user existe)
            //pour cela on va instancier un Livestream et tenter de récupérer les informations
              
          
            if (i == 1)
            {
                bwCheckStream.RunWorkerAsync();
            }
            if (i > 1 )
            {
                throw new NotImplementedException("Plusieurs plugins permettent de gérer ce stream. Ce comportement n'est pas encore géré :/");
            }



        }


        private void bwCheckStream_DoWork(object sender, DoWorkEventArgs e)
        {
            FormEnable(false);
            SpinnerEnable(true);
            LS = AppSettings.ActivateFromUrl(SERVICE, URL);
            LS.GetStreamInfos();

            streamexisting = (String.IsNullOrEmpty(LS.Streamurl)) ? false : true; 
            if (!streamexisting) { LS = null; }

        }

        private void bwCheckStream_Finished(object sender, RunWorkerCompletedEventArgs e)
        {
            FormEnable(true);
            SpinnerEnable(false);
            UpdateUI();
        }

        #endregion


        #region Delegates

        private void FormEnable(bool value)
        {

            if (this.InvokeRequired)
            {
                SetEnabled d = new SetEnabled(FormEnable);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                this.Enabled = value;
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
                loadingCircleToolStripMenuItem1.LoadingCircleControl.Active = value;
                loadingCircleToolStripMenuItem1.LoadingCircleControl.Visible = value;

            }
        }
        private void UpdateUI()
        {
            if (InvokeRequired)
            {
                this.Invoke(DoUpdateUI, new object[] {  });
            }
            else
            {
                comboBox1.Enabled = comboBox1.Items.Count > 1;
                if (comboBox1.Items.Count > 0) { comboBox1.SelectedIndex = 0; }

                BtnADD.Enabled = (urlvalidated && streamexisting ) && (comboBox1.SelectedIndex > -1);

                if ( (urlvalidated && streamexisting) && (LS != null))
                {

                    if (LS.Picture == null)
                    {
                        pictureBox1.Image = AppSettings.StreamsLogos.Images[AppSettings.Default_picturekey];
                    }
                    else pictureBox1.Load(LS.Picture);
                    labelStreamDescription.Text = LS.Title;
                    linkLabel1.Text = LS.Streamurl;
                }
                else
                {

                }


            }
        }
        private void FillCombo()
        {
            if (InvokeRequired)
            {
                this.Invoke(DoFillCombo, new object[] { });
            }
            else
            {
               foreach (string s in Apis)
                {
                    comboBox1.Items.Add(s);
                }
            }
        }
        private void ClearCombo()
        {
            if (InvokeRequired)
            {
                this.Invoke(DoClearCombo, new object[] { });
            }
            else
            {
                comboBox1.Items.Clear();
            }
        }




        #endregion

        #region UI
        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            urlvalidated = false;
            streamexisting = false;

            URL = textBox1.Text;
            SERVICE = "";

            Apis.Clear();
            ClearCombo();
            UpdateUI();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            bwCheckurl.RunWorkerAsync();
        }

    
        private void button2_Click(object sender, EventArgs e)
        {
            AppSettings.AddStream(LS);
            Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SERVICE = comboBox1.Text;
        }


        #endregion

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}
