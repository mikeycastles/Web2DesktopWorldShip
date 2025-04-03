
namespace WebAppShipping
{
    partial class frmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtToken = new System.Windows.Forms.TextBox();
            this.btnSave = new MaterialSkin.Controls.MaterialFlatButton();
            this.txtVerbiage = new System.Windows.Forms.TextBox();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.lblURL = new MaterialSkin.Controls.MaterialLabel();
            this.chkTracking = new MaterialSkin.Controls.MaterialCheckBox();
            this.SuspendLayout();
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel1.Location = new System.Drawing.Point(23, 92);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(110, 19);
            this.materialLabel1.TabIndex = 0;
            this.materialLabel1.Text = "Email Address:";
            // 
            // materialLabel2
            // 
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel2.Location = new System.Drawing.Point(54, 137);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(79, 19);
            this.materialLabel2.TabIndex = 0;
            this.materialLabel2.Text = "Password:";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(140, 90);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(314, 20);
            this.txtEmail.TabIndex = 0;
            // 
            // txtToken
            // 
            this.txtToken.Location = new System.Drawing.Point(139, 136);
            this.txtToken.Name = "txtToken";
            this.txtToken.PasswordChar = '*';
            this.txtToken.Size = new System.Drawing.Size(314, 20);
            this.txtToken.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.Depth = 0;
            this.btnSave.Location = new System.Drawing.Point(408, 266);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnSave.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSave.Name = "btnSave";
            this.btnSave.Primary = false;
            this.btnSave.Size = new System.Drawing.Size(46, 36);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtVerbiage
            // 
            this.txtVerbiage.Location = new System.Drawing.Point(139, 178);
            this.txtVerbiage.Name = "txtVerbiage";
            this.txtVerbiage.Size = new System.Drawing.Size(314, 20);
            this.txtVerbiage.TabIndex = 1;
            this.txtVerbiage.Text = "Your Tracking #(\'s)  : ";
            // 
            // materialLabel3
            // 
            this.materialLabel3.AutoSize = true;
            this.materialLabel3.Depth = 0;
            this.materialLabel3.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel3.Location = new System.Drawing.Point(16, 179);
            this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel3.Name = "materialLabel3";
            this.materialLabel3.Size = new System.Drawing.Size(117, 19);
            this.materialLabel3.TabIndex = 0;
            this.materialLabel3.Text = "Notes Verbiage:";
            // 
            // txtURL
            // 
            this.txtURL.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtURL.Location = new System.Drawing.Point(140, 217);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(314, 20);
            this.txtURL.TabIndex = 1;
            this.txtURL.Text = "https://www.printavo.com/api/version/";
            // 
            // lblURL
            // 
            this.lblURL.AutoSize = true;
            this.lblURL.Depth = 0;
            this.lblURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblURL.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblURL.Location = new System.Drawing.Point(17, 217);
            this.lblURL.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new System.Drawing.Size(76, 18);
            this.lblURL.TabIndex = 0;
            this.lblURL.Text = "Base URL";
            // 
            // chkTracking
            // 
            this.chkTracking.AutoSize = true;
            this.chkTracking.Depth = 0;
            this.chkTracking.Font = new System.Drawing.Font("Roboto", 10F);
            this.chkTracking.Location = new System.Drawing.Point(140, 256);
            this.chkTracking.Margin = new System.Windows.Forms.Padding(0);
            this.chkTracking.MouseLocation = new System.Drawing.Point(-1, -1);
            this.chkTracking.MouseState = MaterialSkin.MouseState.HOVER;
            this.chkTracking.Name = "chkTracking";
            this.chkTracking.Ripple = true;
            this.chkTracking.Size = new System.Drawing.Size(200, 30);
            this.chkTracking.TabIndex = 4;
            this.chkTracking.Text = "Don\'t Write Back Tracking #";
            this.chkTracking.UseVisualStyleBackColor = true;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 317);
            this.Controls.Add(this.chkTracking);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtURL);
            this.Controls.Add(this.txtVerbiage);
            this.Controls.Add(this.txtToken);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblURL);
            this.Controls.Add(this.materialLabel3);
            this.Controls.Add(this.materialLabel2);
            this.Controls.Add(this.materialLabel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSettings";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtToken;
        private MaterialSkin.Controls.MaterialFlatButton btnSave;
        private System.Windows.Forms.TextBox txtVerbiage;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private System.Windows.Forms.TextBox txtURL;
        private MaterialSkin.Controls.MaterialLabel lblURL;
        private MaterialSkin.Controls.MaterialCheckBox chkTracking;
    }
}