namespace StreamNotifier
{
    partial class frmAddStream
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnADD = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.loadingCircleToolStripMenuItem1 = new MRG.Controls.UI.LoadingCircleToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.panelStreamInfos = new System.Windows.Forms.Panel();
            this.labelStreamDescription = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.statusStrip1.SuspendLayout();
            this.panelStreamInfos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(83, 30);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(431, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(534, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Check";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(209, 69);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(305, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Url : ";
            // 
            // BtnADD
            // 
            this.BtnADD.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BtnADD.Enabled = false;
            this.BtnADD.Location = new System.Drawing.Point(187, 252);
            this.BtnADD.Name = "BtnADD";
            this.BtnADD.Size = new System.Drawing.Size(133, 34);
            this.BtnADD.TabIndex = 5;
            this.BtnADD.Text = "OK";
            this.BtnADD.UseVisualStyleBackColor = true;
            this.BtnADD.Click += new System.EventHandler(this.button2_Click);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button2.Location = new System.Drawing.Point(326, 252);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(133, 34);
            this.button2.TabIndex = 6;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadingCircleToolStripMenuItem1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 290);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(656, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // loadingCircleToolStripMenuItem1
            // 
            // 
            // loadingCircleToolStripMenuItem1
            // 
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.AccessibleName = "loadingCircleToolStripMenuItem1";
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Active = false;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Color = System.Drawing.Color.DarkGray;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.InnerCircleRadius = 8;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Location = new System.Drawing.Point(1, 2);
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Name = "loadingCircleToolStripMenuItem1";
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.NumberSpoke = 10;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.OuterCircleRadius = 10;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.RotationSpeed = 100;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Size = new System.Drawing.Size(28, 20);
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.SpokeThickness = 4;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.TabIndex = 1;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Text = "loadingCircleToolStripMenuItem1";
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Visible = false;
            this.loadingCircleToolStripMenuItem1.Name = "loadingCircleToolStripMenuItem1";
            this.loadingCircleToolStripMenuItem1.Size = new System.Drawing.Size(28, 20);
            this.loadingCircleToolStripMenuItem1.Text = "loadingCircleToolStripMenuItem1";
            this.loadingCircleToolStripMenuItem1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(90, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Detected Service : ";
            // 
            // panelStreamInfos
            // 
            this.panelStreamInfos.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelStreamInfos.Controls.Add(this.panel1);
            this.panelStreamInfos.Controls.Add(this.labelStreamDescription);
            this.panelStreamInfos.Controls.Add(this.pictureBox1);
            this.panelStreamInfos.Location = new System.Drawing.Point(12, 96);
            this.panelStreamInfos.Name = "panelStreamInfos";
            this.panelStreamInfos.Size = new System.Drawing.Size(632, 147);
            this.panelStreamInfos.TabIndex = 9;
            // 
            // labelStreamDescription
            // 
            this.labelStreamDescription.Location = new System.Drawing.Point(100, 13);
            this.labelStreamDescription.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelStreamDescription.Name = "labelStreamDescription";
            this.labelStreamDescription.Size = new System.Drawing.Size(523, 80);
            this.labelStreamDescription.TabIndex = 3;
            this.labelStreamDescription.Text = "sdfg42ds2g4.5fdsg354fd5g.f4jdhgbfuhjsdgfjhydsefkuj;sgvegfdjyqvcvfjhdvesh edfvgcjh" +
    "ysvjgsvdscgeyhehgjczhyevyzhevczyegvcyhzuecvzikvgeczyevgczhyecvzevczevcezjvcehvzy" +
    "cz";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(14, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(80, 80);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 113);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(628, 30);
            this.panel1.TabIndex = 4;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel1.Location = new System.Drawing.Point(0, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(628, 30);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://twitch.tv/Kappa";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmAddStream
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 312);
            this.Controls.Add(this.panelStreamInfos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.BtnADD);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmAddStream";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add a new Stream";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelStreamInfos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnADD;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private MRG.Controls.UI.LoadingCircleToolStripMenuItem loadingCircleToolStripMenuItem1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelStreamInfos;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label labelStreamDescription;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}