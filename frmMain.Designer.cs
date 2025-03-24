
using System.Threading;

namespace WebAppShipping
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnSettings = new MaterialSkin.Controls.MaterialFlatButton();
            this.btnStartService = new MaterialSkin.Controls.MaterialFlatButton();
            this.btnDailyInvoices = new MaterialSkin.Controls.MaterialFlatButton();
            this.dtDate = new System.Windows.Forms.DateTimePicker();
            this.gpManul = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gpManul.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSettings
            // 
            this.btnSettings.AutoSize = true;
            this.btnSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSettings.Depth = 0;
            this.btnSettings.Location = new System.Drawing.Point(52, 330);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnSettings.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Primary = false;
            this.btnSettings.Size = new System.Drawing.Size(76, 36);
            this.btnSettings.TabIndex = 0;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnStartService
            // 
            this.btnStartService.AutoSize = true;
            this.btnStartService.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnStartService.Depth = 0;
            this.btnStartService.Location = new System.Drawing.Point(31, 28);
            this.btnStartService.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnStartService.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnStartService.Name = "btnStartService";
            this.btnStartService.Primary = false;
            this.btnStartService.Size = new System.Drawing.Size(112, 36);
            this.btnStartService.TabIndex = 0;
            this.btnStartService.Text = "Start Service";
            this.btnStartService.UseVisualStyleBackColor = true;
            this.btnStartService.Click += new System.EventHandler(this.btnStartService_Click);
            // 
            // btnDailyInvoices
            // 
            this.btnDailyInvoices.AutoSize = true;
            this.btnDailyInvoices.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDailyInvoices.Depth = 0;
            this.btnDailyInvoices.Location = new System.Drawing.Point(27, 64);
            this.btnDailyInvoices.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnDailyInvoices.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnDailyInvoices.Name = "btnDailyInvoices";
            this.btnDailyInvoices.Primary = false;
            this.btnDailyInvoices.Size = new System.Drawing.Size(109, 36);
            this.btnDailyInvoices.TabIndex = 0;
            this.btnDailyInvoices.Text = "Pull Invoices";
            this.btnDailyInvoices.UseVisualStyleBackColor = true;
            this.btnDailyInvoices.Click += new System.EventHandler(this.btnDailyInvoices_Click);
            // 
            // dtDate
            // 
            this.dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtDate.Location = new System.Drawing.Point(27, 35);
            this.dtDate.Name = "dtDate";
            this.dtDate.Size = new System.Drawing.Size(114, 20);
            this.dtDate.TabIndex = 1;
            // 
            // gpManul
            // 
            this.gpManul.Controls.Add(this.dtDate);
            this.gpManul.Controls.Add(this.btnDailyInvoices);
            this.gpManul.Location = new System.Drawing.Point(12, 176);
            this.gpManul.Name = "gpManul";
            this.gpManul.Size = new System.Drawing.Size(171, 111);
            this.gpManul.TabIndex = 2;
            this.gpManul.TabStop = false;
            this.gpManul.Text = "Manual Operation";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnStartService);
            this.groupBox1.Location = new System.Drawing.Point(12, 73);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(171, 87);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Automation Settings";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(198, 396);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gpManul);
            this.Controls.Add(this.btnSettings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Web App Shipping";
            this.gpManul.ResumeLayout(false);
            this.gpManul.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialFlatButton btnSettings;
        private MaterialSkin.Controls.MaterialFlatButton btnStartService;
        private MaterialSkin.Controls.MaterialFlatButton btnDailyInvoices;
        private System.Windows.Forms.DateTimePicker dtDate;
        private System.Windows.Forms.GroupBox gpManul;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

