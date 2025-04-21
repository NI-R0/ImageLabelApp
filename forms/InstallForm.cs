using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ImageLabelApp.forms
{
    public class InstallForm : Form
    {
        public InstallForm()
        {
            this.Text = "ImageLabelApp Installer";
            this.Size = new Size(300, 150);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = SystemFonts.MessageBoxFont;

            var installButton = new Button
            {
                Text = "Install ImageLabelApp",
                Width = 250,
                Height = 30,
                Top = 20,
                Left = 20,
                AutoSize = false,
                UseVisualStyleBackColor = true
            };

            var uninstallButton = new Button
            {
                Text = "Uninstall ImageLabelApp",
                Width = 250,
                Height = 30,
                Top = 60,
                Left = 20,
                AutoSize = false,
                UseVisualStyleBackColor = true
            };

            installButton.Click += (s, e) =>
            {
                this.Hide();
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    Arguments = "--install-context-menu",
                    UseShellExecute = true,
                    Verb = "runas"
                };

                try
                {
                    Process.Start(psi);
                    this.Close();
                }
                catch
                {
                    this.Show();
                    MessageBox.Show("Installation canceled.", "Information",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            uninstallButton.Click += (s, e) =>
            {
                this.Hide();
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    Arguments = "--uninstall-context-menu",
                    UseShellExecute = true,
                    Verb = "runas"
                };

                try
                {
                    Process.Start(psi);
                    this.Close();
                }
                catch
                {
                    this.Show();
                    MessageBox.Show("Uninstallation canceled.", "Information",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            // Add controls to form
            this.Controls.Add(installButton);
            this.Controls.Add(uninstallButton);
        }
    }
}