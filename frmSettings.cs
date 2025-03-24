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
            DataTable dt = AccessHelper.ExecuteQuery("SELECT * FROM Settings");
            if (dt.Rows.Count > 0)
            {
               txtEmail.Text = dt.Rows[0][1].ToString();
                txtToken.Text =  dt.Rows[0][2].ToString();
                if (dt.Rows[0][3].ToString().Length > 0)
                   txtVerbiage.Text = dt.Rows[0][3].ToString();
            }
                //materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
                //materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue500, Primary.Blue700, Primary.Blue200, Accent.LightBlue200, TextShade.WHITE);
            }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Save?", "Save Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                dynamic settings = new ExpandoObject();
                var settingsDict = (IDictionary<string, object>)settings;

                // Step 2: Add dynamic properties based on form fields
                settingsDict["Email"] = txtEmail.Text;
                settingsDict["Password"] = txtToken.Text;
                settingsDict["Verbiage"] = txtVerbiage.Text;

                // Step 3: Convert dynamic object to (string, object)[] format
                var values = settingsDict.Select(kv => (kv.Key, kv.Value)).ToArray();

                // Step 3: Insert or Update User in SQLite
                AccessHelper.ExecuteNonQuery("DELETE FROM SETTINGS");
                AccessHelper.InsertOrUpdate("Settings", "Email", settingsDict["Email"], values);

            }
            MessageBox.Show("Settings Saved Succesfully!", "Save Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
