using System;
using System.Windows.Forms;
using P;


namespace PatientDisplay
{

#region Windows Form Designer generated code    

   partial class PatientForm {

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303")]
      private void InitializeComponent()
      {
      this.components = new System.ComponentModel.Container();
      this.fGamePadTimer = new System.Windows.Forms.Timer(this.components);
      this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
      this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
      this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
      this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
      this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
      this.fPictureBox = new System.Windows.Forms.PictureBox();
      this.toolStrip = new System.Windows.Forms.ToolStrip();
      this.redLevel = new System.Windows.Forms.ToolStripLabel();
      this.redLevelBar = new System.Windows.Forms.ToolStripProgressBar();
      this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
      ((System.ComponentModel.ISupportInitialize)(this.fPictureBox)).BeginInit();
      this.toolStrip.SuspendLayout();
      this.SuspendLayout();

      // 
      // BottomToolStripPanel
      // 
      this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
      this.BottomToolStripPanel.Name = "BottomToolStripPanel";
      this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
      this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
      // 
      // TopToolStripPanel
      // 
      this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
      this.TopToolStripPanel.Name = "TopToolStripPanel";
      this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
      this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
      // 
      // RightToolStripPanel
      // 
      this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
      this.RightToolStripPanel.Name = "RightToolStripPanel";
      this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
      this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
      // 
      // LeftToolStripPanel
      // 
      this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
      this.LeftToolStripPanel.Name = "LeftToolStripPanel";
      this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
      this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
      // 
      // ContentPanel
      // 
      this.ContentPanel.AutoScroll = true;
      this.ContentPanel.Size = new System.Drawing.Size(642, 458);
      // 
      // fPictureBox
      // 
      this.fPictureBox.BackColor = System.Drawing.Color.Black;
      this.fPictureBox.Location = new System.Drawing.Point(1, 27);
      this.fPictureBox.Name = "fPictureBox";
      this.fPictureBox.Size = new System.Drawing.Size(1024, 600);
      this.fPictureBox.TabIndex = 1;
      this.fPictureBox.TabStop = false;
      // 
      // toolStrip
      // 
      this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.redLevel,
            this.redLevelBar,
            this.toolStripLabel1});
      this.toolStrip.Location = new System.Drawing.Point(0, 0);
      this.toolStrip.Name = "toolStrip";
      this.toolStrip.Size = new System.Drawing.Size(642, 25);
      this.toolStrip.TabIndex = 2;
      this.toolStrip.Text = "toolStrip1";
      // 
      // redLevel
      // 
      this.redLevel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.redLevel.ForeColor = System.Drawing.SystemColors.ControlText;
      this.redLevel.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
      this.redLevel.Name = "redLevel";
      this.redLevel.Size = new System.Drawing.Size(13, 22);
      this.redLevel.Text = "0";
      // 
      // redLevelBar
      // 
      this.redLevelBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.redLevelBar.ForeColor = System.Drawing.Color.Green;
      this.redLevelBar.Margin = new System.Windows.Forms.Padding(1, 2, 10, 1);
      this.redLevelBar.Name = "redLevelBar";
      this.redLevelBar.Size = new System.Drawing.Size(100, 22);
      this.redLevelBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
      this.redLevelBar.Value = 1;
      // 
      // toolStripLabel1
      // 
      this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.toolStripLabel1.ForeColor = System.Drawing.SystemColors.ControlText;
      this.toolStripLabel1.Name = "toolStripLabel1";
      this.toolStripLabel1.Size = new System.Drawing.Size(98, 22);
      this.toolStripLabel1.Text = "уровень красного";
      // 
      // PatientForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(642, 508);
      this.ControlBox = false;
      this.Controls.Add(this.toolStrip);
      this.Controls.Add(this.fPictureBox);
      this.ForeColor = System.Drawing.SystemColors.Control;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "PatientForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "Контроль положения пациента";
      this.Load += new System.EventHandler(this.PatientDisplayForm_Load);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
      this.Closing += new System.ComponentModel.CancelEventHandler(this.PatientDisplayForm_Closing);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
      ((System.ComponentModel.ISupportInitialize)(this.fPictureBox)).EndInit();
      this.toolStrip.ResumeLayout(false);
      this.toolStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

      }

      private Timer fGamePadTimer;
      private System.ComponentModel.IContainer components;
      private ToolStripPanel BottomToolStripPanel;
      private ToolStripPanel TopToolStripPanel;
      private ToolStripPanel RightToolStripPanel;
      private ToolStripPanel LeftToolStripPanel;
      private ToolStripContentPanel ContentPanel;
      private PictureBox fPictureBox;
      private ToolStrip toolStrip;
      private ToolStripProgressBar redLevelBar;
      private ToolStripLabel redLevel;
      private ToolStripLabel toolStripLabel1;
   }

#endregion Windows Form Designer generated code

}