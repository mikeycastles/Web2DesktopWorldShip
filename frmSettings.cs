using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace WebAppShipping
{
    public partial class frmSettings : MaterialForm
    {
        public frmSettings()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            try
            {
                var settings = new AppSettings();
                txtEmail.Text = settings.Email;
                txtToken.Text = settings.Password;
                txtVerbiage.Text = settings.Verbiage;
                txtURL.Text = settings.BaseUrl;
                chkTracking.Checked = settings.Tracking;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Save?", "Save Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var settings = new AppSettings
                    {
                        Email = txtEmail.Text,
                        Password = txtToken.Text,
                        Verbiage = txtVerbiage.Text,
                        BaseUrl = txtURL.Text,
                        Tracking = chkTracking.Checked
                    };

                    if (settings.Save())
                    {
                        MessageBox.Show("Settings saved successfully!", "Save Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to save settings. Please check the logs.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while saving: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
