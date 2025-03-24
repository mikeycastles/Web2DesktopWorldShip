using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace WebAppShipping
{
    public partial class frmMain : MaterialForm
    {
        private cService _cService = new cService();
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _notifyMenu;

        public frmMain()
        {
            try
            {
                InitializeComponent();
                var materialSkinManager = MaterialSkinManager.Instance;
                materialSkinManager.AddFormToManage(this);
                // Optionally, set theme and color scheme:
                // materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
                // materialSkinManager.ColorScheme = new ColorScheme(Primary.Grey700, Primary.Grey700, Primary.Grey700, Accent.LightBlue200, TextShade.WHITE);

                // Create a context menu for the notify icon.
                _notifyMenu = new ContextMenuStrip();
                _notifyMenu.Items.Add("Restore", null, (s, e) => RestoreFromTray());
                _notifyMenu.Items.Add("Exit", null, (s, e) => ExitApplication());

                // Create the notify icon.
                _notifyIcon = new NotifyIcon
                {
                    Icon = this.Icon, // Use the form's icon or specify another icon.
                    ContextMenuStrip = _notifyMenu,
                    Text = "My Application",
                    Visible = true
                };

                // Optionally, handle double-click to restore the window.
                _notifyIcon.DoubleClick += (s, e) => RestoreFromTray();

                // Handle the resize event to hide when minimized.
                this.Resize += MainForm_Resize;
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                // Optionally show a message or rethrow.
                throw;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            try
            {
                // When the window is minimized, hide it and show a balloon tip.
                if (this.WindowState == FormWindowState.Minimized)
                {
                    HideToTray();
                }
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
            }
        }

        private void HideToTray()
        {
            try
            {
                this.Hide();
                // Optionally, show a balloon tip indicating that the app is still running.
                _notifyIcon.ShowBalloonTip(1000, "My Application",
                    "The application is still running. Double-click the tray icon to restore.",
                    ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
            }
        }

        private void RestoreFromTray()
        {
            try
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
            }
        }

        private void ExitApplication()
        {
            try
            {
                _notifyIcon.Visible = false;
                Application.Exit();
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                // Prevent the form from closing when the user clicks the close button; hide it instead.
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    HideToTray();
                }
                base.OnFormClosing(e);
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                frmSettings frmSettings = new frmSettings();
                frmSettings.ShowDialog();
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
            }
        }

        private async void btnDailyInvoices_Click(object sender, EventArgs e)
        {
            try
            {
                btnDailyInvoices.Enabled = false;

                // Process shipped orders and get the tracking export message.
                string shippedResult = await PrintavoApiClient.ProcessShippedOrdersAsync();
                if (!string.IsNullOrEmpty(shippedResult))
                {
                    MessageBox.Show(shippedResult, "Tracking Numbers Exported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Import orders and get the orders imported message.
                string ordersResult = await cPrintavo.GetOrders(dtDate.Value.ToShortDateString());
                MessageBox.Show(ordersResult, "Orders Imported", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                MessageBox.Show("Error processing invoices: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDailyInvoices.Enabled = true;
            }
        }


        private void btnStartService_Click(object sender, EventArgs e)
        {
            try
            {
                if (_cService.IsRunning)
                {
                    _cService.Stop();
                }
                else
                {
                    _cService.Start();
                }
                UpdateButtonText();
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                // Optionally rethrow or handle the exception.
                throw;
            }
        }

        /// <summary>
        /// Updates the button text based on the service state.
        /// </summary>
        private void UpdateButtonText()
        {
            try
            {
                btnStartService.Text = _cService.IsRunning ? "Stop Service" : "Start Service";
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
            }
        }
    }
}
