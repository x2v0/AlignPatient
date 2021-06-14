// $Id: DoctorDisplayForm.cs 2079 2014-02-11 07:46:07Z onuchin $
// Author: Valeriy Onuchin   29.12.2010

//#define DEBUG
//#define LOCAL_DEBUG


using System.Windows.Forms;
using System.ComponentModel;


// to provide shorthand to clear up ambiguities

namespace DoctorDisplay
{
   public partial class DoctorDisplayForm
   {

      #region Windows Form Designer generated code

      public DoctorDisplayForm()
      {
         InitializeComponent();
      }
      private ToolStrip toolStrip;
      private ToolStripButton buttonLoad;
      private ToolStripButton buttonHelp;
      private ToolStrip toolStripMode;
      private ToolStripLabel labelMode;
      private ToolStripComboBox toolComboMode;
      private ToolStripButton buttonSnap;
      private ToolStripButton buttonSave;
      private ToolStripSeparator toolStripSeparator1;
      private TableLayoutPanel tableLayoutPanel1;
      private Label label1;
      private Label label6;
      private Label labelID;
      private Label label5;
      private Label labelBirthday;
      private Label label4;
      private Label labelSndName;
      private Label labelName;
      private Label labelFamilyName;
      private Label label3;
      private Label label2;
      private TableLayoutPanel tableLayoutPanel2;
      private PictureBox pictureFoto;
      private Label label7;
      private Panel panel1;
      private TableLayoutPanel tableLayoutPanel3;
      private Label label8;
      private Label label9;
      private Label label10;
      private Label label14;
      private Label label16;
      private Label label18;
      private SplitContainer splitContainer1;
      private ToolStripLabel labelExhale;
      private ToolStripButton btnZoom;
      private ToolStripSeparator toolStripSeparator2;
      private ToolStripButton btnZoomOut;
      private ToolStripSeparator toolStripSeparator3;
      private ToolStripSeparator toolStripSeparator4;
      private ToolStripSeparator toolStripSeparator5;
      private ToolStripSeparator toolStripSeparator6;
      private ToolStripProgressBar redLevelBar;
      private ToolStripLabel redLevel;
      private ToolStripLabel toolStripLabel1;
      private BackgroundWorker backgroundWorker1;
      private System.Windows.Forms.Timer fGamePadTimer;
      private IContainer components;
      private ToolStripLabel gamePadLabel;
      private TableLayoutPanel patientDataTable;
      private ToolStripButton buttonSound;

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      [ global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303") ]
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DoctorDisplayForm));
         this.toolStrip = new System.Windows.Forms.ToolStrip();
         this.buttonLoad = new System.Windows.Forms.ToolStripButton();
         this.buttonHelp = new System.Windows.Forms.ToolStripButton();
         this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
         this.redLevelBar = new System.Windows.Forms.ToolStripProgressBar();
         this.redLevel = new System.Windows.Forms.ToolStripLabel();
         this.buttonSound = new System.Windows.Forms.ToolStripButton();
         this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
         this.gamePadLabel = new System.Windows.Forms.ToolStripLabel();
         this.toolStripMode = new System.Windows.Forms.ToolStrip();
         this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
         this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
         this.btnZoom = new System.Windows.Forms.ToolStripButton();
         this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
         this.buttonSnap = new System.Windows.Forms.ToolStripButton();
         this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
         this.buttonSave = new System.Windows.Forms.ToolStripButton();
         this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
         this.labelMode = new System.Windows.Forms.ToolStripLabel();
         this.toolComboMode = new System.Windows.Forms.ToolStripComboBox();
         this.labelExhale = new System.Windows.Forms.ToolStripLabel();
         this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
         this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
         this.splitContainer1 = new System.Windows.Forms.SplitContainer();
         this.patientDataTable = new System.Windows.Forms.TableLayoutPanel();
         this.panel1 = new System.Windows.Forms.Panel();
         this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
         this.label8 = new System.Windows.Forms.Label();
         this.label9 = new System.Windows.Forms.Label();
         this.label10 = new System.Windows.Forms.Label();
         this.label14 = new System.Windows.Forms.Label();
         this.label16 = new System.Windows.Forms.Label();
         this.label18 = new System.Windows.Forms.Label();
         this.pictureFoto = new System.Windows.Forms.PictureBox();
         this.labelFamilyName = new System.Windows.Forms.Label();
         this.labelName = new System.Windows.Forms.Label();
         this.labelSndName = new System.Windows.Forms.Label();
         this.labelBirthday = new System.Windows.Forms.Label();
         this.labelID = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.label6 = new System.Windows.Forms.Label();
         this.label5 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
         this.label7 = new System.Windows.Forms.Label();
         this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
         this.fGamePadTimer = new System.Windows.Forms.Timer(this.components);
         this.toolStrip.SuspendLayout();
         this.toolStripMode.SuspendLayout();
         this.tableLayoutPanel1.SuspendLayout();
         this.splitContainer1.SuspendLayout();
         this.patientDataTable.SuspendLayout();
         this.panel1.SuspendLayout();
         this.tableLayoutPanel3.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureFoto)).BeginInit();
         this.SuspendLayout();
         // 
         // toolStrip
         // 
         this.toolStrip.BackColor = System.Drawing.Color.Silver;
         this.toolStrip.GripMargin = new System.Windows.Forms.Padding(4, 4, 2, 2);
         this.toolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
         this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
         {
            this.buttonLoad, this.buttonHelp, this.toolStripLabel1, this.redLevelBar, this.redLevel, this.buttonSound, this.toolStripSeparator1, this.gamePadLabel
         });
         this.toolStrip.Location = new System.Drawing.Point(0, 0);
         this.toolStrip.Name = "toolStrip";
         this.toolStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
         this.toolStrip.Size = new System.Drawing.Size(1300, 39);
         this.toolStrip.Stretch = true;
         this.toolStrip.TabIndex = 5;
         this.toolStrip.Text = "Служебные функции";
         this.toolStrip.UseWaitCursor = true;
         // 
         // buttonLoad
         // 
         this.buttonLoad.Enabled = false;
         this.buttonLoad.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoad.Image")));
         this.buttonLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.buttonLoad.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
         this.buttonLoad.Name = "buttonLoad";
         this.buttonLoad.Size = new System.Drawing.Size(165, 36);
         this.buttonLoad.Text = Strings.LoadImage;
         this.buttonLoad.ToolTipText = Strings.LoadImageEx;
         this.buttonLoad.Click += new System.EventHandler(this.buttonLoadClick);
         // 
         // buttonHelp
         // 
         this.buttonHelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
         this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
         this.buttonHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.buttonHelp.Margin = new System.Windows.Forms.Padding(0, 1, 5, 2);
         this.buttonHelp.Name = "buttonHelp";
         this.buttonHelp.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
         this.buttonHelp.Size = new System.Drawing.Size(119, 36);
         this.buttonHelp.Text = Strings.Help;
         this.buttonHelp.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
         this.buttonHelp.ToolTipText = Strings.HelpEx;
         this.buttonHelp.Click += new System.EventHandler(this.buttonHelpClick);
         // 
         // toolStripLabel1
         // 
         this.toolStripLabel1.Margin = new System.Windows.Forms.Padding(30, 1, 0, 2);
         this.toolStripLabel1.Name = "toolStripLabel1";
         this.toolStripLabel1.Size = new System.Drawing.Size(99, 36);
         this.toolStripLabel1.Text = Strings.RedLevel;
         // 
         // redLevelBar
         // 
         this.redLevelBar.AutoSize = false;
         this.redLevelBar.AutoToolTip = true;
         this.redLevelBar.BackColor = System.Drawing.Color.Ivory;
         this.redLevelBar.ForeColor = System.Drawing.Color.Green;
         this.redLevelBar.Name = "redLevelBar";
         this.redLevelBar.Size = new System.Drawing.Size(200, 24);
         this.redLevelBar.Step = 1;
         this.redLevelBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
         this.redLevelBar.ToolTipText = Strings.RedLevel;
         this.redLevelBar.Value = 1;
         // 
         // redLevel
         // 
         this.redLevel.AutoSize = false;
         this.redLevel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
         this.redLevel.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
         this.redLevel.Name = "redLevel";
         this.redLevel.Size = new System.Drawing.Size(50, 36);
         this.redLevel.Text = "0";
         this.redLevel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // buttonSound
         // 
         this.buttonSound.AutoSize = false;
         this.buttonSound.BackColor = System.Drawing.Color.GreenYellow;
         this.buttonSound.CheckOnClick = true;
         this.buttonSound.Enabled = false;
         this.buttonSound.Image = ((System.Drawing.Image)(resources.GetObject("buttonSound.Image")));
         this.buttonSound.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.buttonSound.Margin = new System.Windows.Forms.Padding(40, 1, 5, 2);
         this.buttonSound.Name = "buttonSound";
         this.buttonSound.Size = new System.Drawing.Size(80, 36);
         this.buttonSound.Text = Strings.SoundOn;
         this.buttonSound.ToolTipText = Strings.SoundOnEx;
         this.buttonSound.Visible = false;
         this.buttonSound.CheckedChanged += new System.EventHandler(this.soundCheckedChanged);
         // 
         // toolStripSeparator1
         // 
         this.toolStripSeparator1.Name = "toolStripSeparator1";
         this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
         // 
         // gamePadLabel
         // 
         this.gamePadLabel.ForeColor = System.Drawing.Color.Red;
         this.gamePadLabel.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
         this.gamePadLabel.Name = "gamePadLabel";
         this.gamePadLabel.Size = new System.Drawing.Size(144, 36);
         this.gamePadLabel.Text = Strings.GamepadCon;
         this.gamePadLabel.ToolTipText = Strings.GamepadCon;
         this.gamePadLabel.Visible = false;
         // 
         // toolStripMode
         // 
         this.toolStripMode.BackColor = System.Drawing.Color.Silver;
         this.toolStripMode.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
         this.toolStripMode.ImageScalingSize = new System.Drawing.Size(32, 32);
         this.toolStripMode.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
         {
            this.btnZoomOut, this.toolStripSeparator2, this.btnZoom, this.toolStripSeparator4, this.buttonSnap, this.toolStripSeparator5, this.buttonSave, this.toolStripSeparator3, this.labelMode, this.toolComboMode, this.labelExhale, this.toolStripSeparator6
         });
         this.toolStripMode.Location = new System.Drawing.Point(3, 486);
         this.toolStripMode.Name = "toolStripMode";
         this.toolStripMode.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
         this.toolStripMode.Size = new System.Drawing.Size(1294, 39);
         this.toolStripMode.Stretch = true;
         this.toolStripMode.TabIndex = 6;
         this.toolStripMode.Text = Strings.DisplayMode;
         this.toolStripMode.UseWaitCursor = true;
         // 
         // btnZoomOut
         // 
         this.btnZoomOut.Image = global::DoctorDisplay.Strings.zoom_out;
         this.btnZoomOut.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
         this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.btnZoomOut.Margin = new System.Windows.Forms.Padding(10, 1, 10, 2);
         this.btnZoomOut.Name = "btnZoomOut";
         this.btnZoomOut.Size = new System.Drawing.Size(74, 36);
         this.btnZoomOut.Text = Strings.Reset;
         this.btnZoomOut.ToolTipText = Strings.ResetEx;
         this.btnZoomOut.Click += new System.EventHandler(this.OnZoomOutClick);
         // 
         // toolStripSeparator2
         // 
         this.toolStripSeparator2.Name = "toolStripSeparator2";
         this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
         // 
         // btnZoom
         // 
         this.btnZoom.Image = global::DoctorDisplay.Strings.zoom_in;
         this.btnZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.btnZoom.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
         this.btnZoom.Name = "btnZoom";
         this.btnZoom.Size = new System.Drawing.Size(132, 36);
         this.btnZoom.Text = Strings.Zoom;
         this.btnZoom.Click += new System.EventHandler(this.OnZoomClick);
         // 
         // toolStripSeparator4
         // 
         this.toolStripSeparator4.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
         this.toolStripSeparator4.Name = "toolStripSeparator4";
         this.toolStripSeparator4.Size = new System.Drawing.Size(6, 39);
         // 
         // buttonSnap
         // 
         this.buttonSnap.Image = ((System.Drawing.Image)(resources.GetObject("buttonSnap.Image")));
         this.buttonSnap.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.buttonSnap.Margin = new System.Windows.Forms.Padding(10, 1, 10, 2);
         this.buttonSnap.Name = "buttonSnap";
         this.buttonSnap.Size = new System.Drawing.Size(110, 36);
         this.buttonSnap.Text = Strings.Snap;
         this.buttonSnap.ToolTipText = Strings.SnapEx;
         this.buttonSnap.Click += new System.EventHandler(this.buttonSnapClick);
         // 
         // toolStripSeparator5
         // 
         this.toolStripSeparator5.Name = "toolStripSeparator5";
         this.toolStripSeparator5.Size = new System.Drawing.Size(6, 39);
         // 
         // buttonSave
         // 
         this.buttonSave.AutoSize = false;
         this.buttonSave.Enabled = false;
         this.buttonSave.Image = ((System.Drawing.Image)(resources.GetObject("buttonSave.Image")));
         this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.buttonSave.Margin = new System.Windows.Forms.Padding(10, 1, 10, 2);
         this.buttonSave.Name = "buttonSave";
         this.buttonSave.Size = new System.Drawing.Size(98, 36);
         this.buttonSave.Text = Strings.Save;
         this.buttonSave.ToolTipText = Strings.SaveEx;
         this.buttonSave.Click += new System.EventHandler(this.buttonSaveClick);
         // 
         // toolStripSeparator3
         // 
         this.toolStripSeparator3.Name = "toolStripSeparator3";
         this.toolStripSeparator3.Size = new System.Drawing.Size(6, 39);
         // 
         // labelMode
         // 
         this.labelMode.Enabled = false;
         this.labelMode.Margin = new System.Windows.Forms.Padding(10, 1, 0, 0);
         this.labelMode.Name = "labelMode";
         this.labelMode.Size = new System.Drawing.Size(117, 38);
         this.labelMode.Text = Strings.DisplayMode + " :";
         // 
         // toolComboMode
         // 
         this.toolComboMode.AutoCompleteCustomSource.AddRange(new string[]
         {
            Strings.ShowDiff, Strings.DontShowDiff, Strings.ShowTransparent
         });
         this.toolComboMode.AutoToolTip = true;
         this.toolComboMode.BackColor = System.Drawing.SystemColors.Window;
         this.toolComboMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.toolComboMode.DropDownWidth = 140;
         this.toolComboMode.Enabled = false;
         this.toolComboMode.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
         this.toolComboMode.Items.AddRange(new object[]
         {
            Strings.DontShowDiff, Strings.ShowDiff, Strings.ShowTransparent
         });
         this.toolComboMode.Name = "toolComboMode";
         this.toolComboMode.Size = new System.Drawing.Size(145, 39);
         this.toolComboMode.ToolTipText = Strings.DisplayMode;
         this.toolComboMode.SelectedIndexChanged += new System.EventHandler(this.selectedModeChanged);
         // 
         // labelExhale
         // 
         this.labelExhale.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
         this.labelExhale.BackColor = System.Drawing.Color.DarkGray;
         this.labelExhale.Image = global::DoctorDisplay.Strings.off;
         this.labelExhale.Margin = new System.Windows.Forms.Padding(0, 1, 5, 2);
         this.labelExhale.Name = "labelExhale";
         this.labelExhale.Size = new System.Drawing.Size(143, 36);
         this.labelExhale.Text = Strings.BreathDetector;
         this.labelExhale.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
         this.labelExhale.Visible = false;
         // 
         // toolStripSeparator6
         // 
         this.toolStripSeparator6.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
         this.toolStripSeparator6.Name = "toolStripSeparator6";
         this.toolStripSeparator6.Size = new System.Drawing.Size(6, 39);
         // 
         // tableLayoutPanel1
         // 
         this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
         this.tableLayoutPanel1.ColumnCount = 1;
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 0);
         this.tableLayoutPanel1.Controls.Add(this.toolStripMode, 0, 1);
         this.tableLayoutPanel1.Controls.Add(this.patientDataTable, 0, 2);
         this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 39);
         this.tableLayoutPanel1.Name = "tableLayoutPanel1";
         this.tableLayoutPanel1.RowCount = 3;
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 63.85224F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 36.14776F));
         this.tableLayoutPanel1.Size = new System.Drawing.Size(1300, 803);
         this.tableLayoutPanel1.TabIndex = 7;
         this.tableLayoutPanel1.UseWaitCursor = true;
         // 
         // splitContainer1
         // 
         this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer1.Location = new System.Drawing.Point(3, 3);
         this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
         this.splitContainer1.Name = "splitContainer1";
         // 
         // splitContainer1.Panel1
         // 
         this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.GrayText;
         this.splitContainer1.Panel1.UseWaitCursor = true;
         this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.PbOnPaint);
         this.splitContainer1.Panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnVideoMouseMoveF);
         this.splitContainer1.Panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnVideoMouseDownF);
         this.splitContainer1.Panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnVideoMouseUpF);
         this.splitContainer1.Panel1MinSize = 0;
         // 
         // splitContainer1.Panel2
         // 
         this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.GrayText;
         this.splitContainer1.Panel2.UseWaitCursor = true;
         this.splitContainer1.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.PbOnPaint);
         this.splitContainer1.Panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnVideoMouseMoveP);
         this.splitContainer1.Panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnVideoMouseDownP);
         this.splitContainer1.Panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnVideoMouseUpP);
         this.splitContainer1.Size = new System.Drawing.Size(1294, 480);
         this.splitContainer1.SplitterDistance = 645;
         this.splitContainer1.SplitterIncrement = 2;
         this.splitContainer1.TabIndex = 5;
         this.splitContainer1.UseWaitCursor = true;
         // 
         // patientDataTable
         // 
         this.patientDataTable.ColumnCount = 2;
         this.patientDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
         this.patientDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.patientDataTable.Controls.Add(this.panel1, 0, 0);
         this.patientDataTable.Dock = System.Windows.Forms.DockStyle.Fill;
         this.patientDataTable.Location = new System.Drawing.Point(6, 531);
         this.patientDataTable.Name = "patientDataTable";
         this.patientDataTable.RowCount = 1;
         this.patientDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 266F));
         this.patientDataTable.Size = new System.Drawing.Size(1288, 266);
         this.patientDataTable.TabIndex = 7;
         this.patientDataTable.UseWaitCursor = true;
         // 
         // panel1
         // 
         this.panel1.BackColor = System.Drawing.Color.White;
         this.panel1.Controls.Add(this.tableLayoutPanel3);
         this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
         this.panel1.Location = new System.Drawing.Point(1, 1);
         this.panel1.Margin = new System.Windows.Forms.Padding(1);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(332, 264);
         this.panel1.TabIndex = 4;
         this.panel1.UseWaitCursor = true;
         // 
         // tableLayoutPanel3
         // 
         this.tableLayoutPanel3.BackColor = System.Drawing.Color.White;
         this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
         this.tableLayoutPanel3.ColumnCount = 2;
         this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.9697F));
         this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.0303F));
         this.tableLayoutPanel3.Controls.Add(this.label8, 0, 1);
         this.tableLayoutPanel3.Controls.Add(this.label9, 0, 2);
         this.tableLayoutPanel3.Controls.Add(this.label10, 0, 3);
         this.tableLayoutPanel3.Controls.Add(this.label14, 0, 4);
         this.tableLayoutPanel3.Controls.Add(this.label16, 0, 5);
         this.tableLayoutPanel3.Controls.Add(this.label18, 0, 0);
         this.tableLayoutPanel3.Controls.Add(this.pictureFoto, 1, 0);
         this.tableLayoutPanel3.Controls.Add(this.labelFamilyName, 1, 1);
         this.tableLayoutPanel3.Controls.Add(this.labelName, 1, 2);
         this.tableLayoutPanel3.Controls.Add(this.labelSndName, 1, 3);
         this.tableLayoutPanel3.Controls.Add(this.labelBirthday, 1, 4);
         this.tableLayoutPanel3.Controls.Add(this.labelID, 1, 5);
         this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
         this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
         this.tableLayoutPanel3.Name = "tableLayoutPanel3";
         this.tableLayoutPanel3.RowCount = 6;
         this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
         this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
         this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
         this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
         this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
         this.tableLayoutPanel3.Size = new System.Drawing.Size(331, 268);
         this.tableLayoutPanel3.TabIndex = 5;
         this.tableLayoutPanel3.UseWaitCursor = true;
         // 
         // label8
         // 
         this.label8.AutoSize = true;
         this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
         this.label8.Location = new System.Drawing.Point(4, 183);
         this.label8.Name = "label8";
         this.label8.Size = new System.Drawing.Size(82, 16);
         this.label8.TabIndex = 1;
         this.label8.Text = Strings.Surname;
         this.label8.UseWaitCursor = true;
         // 
         // label9
         // 
         this.label9.AutoSize = true;
         this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
         this.label9.Location = new System.Drawing.Point(4, 200);
         this.label9.Name = "label9";
         this.label9.Size = new System.Drawing.Size(82, 16);
         this.label9.TabIndex = 2;
         this.label9.Text = Strings.PatientName;
         this.label9.UseWaitCursor = true;
         // 
         // label10
         // 
         this.label10.AutoSize = true;
         this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
         this.label10.Location = new System.Drawing.Point(4, 217);
         this.label10.Name = "label10";
         this.label10.Size = new System.Drawing.Size(82, 16);
         this.label10.TabIndex = 3;
         this.label10.Text = Strings.FatherName;
         this.label10.UseWaitCursor = true;
         // 
         // label14
         // 
         this.label14.AutoSize = true;
         this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
         this.label14.Location = new System.Drawing.Point(4, 234);
         this.label14.Name = "label14";
         this.label14.Size = new System.Drawing.Size(82, 16);
         this.label14.TabIndex = 8;
         this.label14.Text = Strings.BirthDate;
         this.label14.UseWaitCursor = true;
         // 
         // label16
         // 
         this.label16.AutoSize = true;
         this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
         this.label16.Location = new System.Drawing.Point(4, 251);
         this.label16.Name = "label16";
         this.label16.Size = new System.Drawing.Size(82, 16);
         this.label16.TabIndex = 10;
         this.label16.Text = Strings.PatientID;
         this.label16.UseWaitCursor = true;
         // 
         // label18
         // 
         this.label18.AutoSize = true;
         this.label18.BackColor = System.Drawing.Color.Silver;
         this.label18.Dock = System.Windows.Forms.DockStyle.Fill;
         this.label18.Location = new System.Drawing.Point(1, 1);
         this.label18.Margin = new System.Windows.Forms.Padding(0);
         this.label18.Name = "label18";
         this.label18.Size = new System.Drawing.Size(88, 181);
         this.label18.TabIndex = 12;
         this.label18.UseWaitCursor = true;
         // 
         // pictureFoto
         // 
         this.pictureFoto.BackColor = System.Drawing.Color.Silver;
         this.pictureFoto.Dock = System.Windows.Forms.DockStyle.Fill;
         this.pictureFoto.Image = ((System.Drawing.Image)(resources.GetObject("pictureFoto.Image")));
         this.pictureFoto.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureFoto.InitialImage")));
         this.pictureFoto.Location = new System.Drawing.Point(90, 1);
         this.pictureFoto.Margin = new System.Windows.Forms.Padding(0);
         this.pictureFoto.Name = "pictureFoto";
         this.pictureFoto.Size = new System.Drawing.Size(240, 181);
         this.pictureFoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
         this.pictureFoto.TabIndex = 0;
         this.pictureFoto.TabStop = false;
         this.pictureFoto.UseWaitCursor = true;
         // 
         // labelFamilyName
         // 
         this.labelFamilyName.Location = new System.Drawing.Point(93, 183);
         this.labelFamilyName.Name = "labelFamilyName";
         this.labelFamilyName.Size = new System.Drawing.Size(100, 16);
         this.labelFamilyName.TabIndex = 0;
         this.labelFamilyName.UseWaitCursor = true;
         // 
         // labelName
         // 
         this.labelName.Location = new System.Drawing.Point(93, 200);
         this.labelName.Name = "labelName";
         this.labelName.Size = new System.Drawing.Size(100, 16);
         this.labelName.TabIndex = 0;
         this.labelName.UseWaitCursor = true;
         // 
         // labelSndName
         // 
         this.labelSndName.Location = new System.Drawing.Point(93, 217);
         this.labelSndName.Name = "labelSndName";
         this.labelSndName.Size = new System.Drawing.Size(100, 16);
         this.labelSndName.TabIndex = 0;
         this.labelSndName.UseWaitCursor = true;
         // 
         // labelBirthday
         // 
         this.labelBirthday.Location = new System.Drawing.Point(93, 234);
         this.labelBirthday.Name = "labelBirthday";
         this.labelBirthday.Size = new System.Drawing.Size(100, 15);
         this.labelBirthday.TabIndex = 0;
         this.labelBirthday.UseWaitCursor = true;
         // 
         // labelID
         // 
         this.labelID.Location = new System.Drawing.Point(93, 251);
         this.labelID.Name = "labelID";
         this.labelID.Size = new System.Drawing.Size(100, 15);
         this.labelID.TabIndex = 0;
         this.labelID.UseWaitCursor = true;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.label1.Location = new System.Drawing.Point(4, -64);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(82, 17);
         this.label1.TabIndex = 1;
         this.label1.Text = Strings.Surname;
         // 
         // label6
         // 
         this.label6.Location = new System.Drawing.Point(0, 0);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(100, 23);
         this.label6.TabIndex = 0;
         // 
         // label5
         // 
         this.label5.Location = new System.Drawing.Point(0, 0);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(100, 23);
         this.label5.TabIndex = 0;
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(0, 0);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(100, 23);
         this.label4.TabIndex = 0;
         // 
         // label3
         // 
         this.label3.Location = new System.Drawing.Point(0, 0);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(100, 23);
         this.label3.TabIndex = 0;
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(0, 0);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(100, 23);
         this.label2.TabIndex = 0;
         // 
         // tableLayoutPanel2
         // 
         this.tableLayoutPanel2.BackColor = System.Drawing.Color.White;
         this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
         this.tableLayoutPanel2.ColumnCount = 2;
         this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.9697F));
         this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.0303F));
         this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
         this.tableLayoutPanel2.Name = "tableLayoutPanel2";
         this.tableLayoutPanel2.RowCount = 1;
         this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
         this.tableLayoutPanel2.Size = new System.Drawing.Size(200, 100);
         this.tableLayoutPanel2.TabIndex = 0;
         // 
         // label7
         // 
         this.label7.AutoSize = true;
         this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
         this.label7.Location = new System.Drawing.Point(4, 184);
         this.label7.Name = "label7";
         this.label7.Size = new System.Drawing.Size(82, 17);
         this.label7.TabIndex = 1;
         this.label7.Text = Strings.Surname;
         // 
         // backgroundWorker1
         // 
         this.backgroundWorker1.WorkerReportsProgress = true;
         this.backgroundWorker1.WorkerSupportsCancellation = true;
         this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDoWork);
         this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgwProgressChanged);
         // 
         // fGamePadTimer
         // 
         this.fGamePadTimer.Interval = 20;
         // 
         // DoctorDisplayForm
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.BackColor = System.Drawing.Color.Silver;
         this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
         this.ClientSize = new System.Drawing.Size(1300, 842);
         this.Controls.Add(this.tableLayoutPanel1);
         this.Controls.Add(this.toolStrip);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.Name = "DoctorDisplayForm";
         this.Text = Strings.ProgDescription;
         this.UseWaitCursor = true;
         this.Load += new System.EventHandler(this.DoctorDisplayForm_Load);
         this.Shown += new System.EventHandler(this.mainShown);
         this.Closing += new System.ComponentModel.CancelEventHandler(this.DoctorDisplayForm_Closing);
         this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
         this.toolStrip.ResumeLayout(false);
         this.toolStrip.PerformLayout();
         this.toolStripMode.ResumeLayout(false);
         this.toolStripMode.PerformLayout();
         this.tableLayoutPanel1.ResumeLayout(false);
         this.tableLayoutPanel1.PerformLayout();
         this.splitContainer1.ResumeLayout(false);
         this.patientDataTable.ResumeLayout(false);
         this.panel1.ResumeLayout(false);
         this.tableLayoutPanel3.ResumeLayout(false);
         this.tableLayoutPanel3.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureFoto)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      protected override void Dispose(bool disposing)
      {
         if (disposing) {
            if (components != null) {
               components.Dispose();
            }
         }
         base.Dispose(disposing);
      }

      #endregion

      static DoctorDisplayForm()
      {
         RegisterRtpFilters();
         RegisterCheckPosFilter();
      }
   }
}
