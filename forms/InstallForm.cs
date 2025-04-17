using System;
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
                ContextMenuInstaller.InstallContextMenu();
                MessageBox.Show("Context menu entries installed.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Run(new LabelManagerForm());
                Application.Exit();
            };

            uninstallButton.Click += (s, e) =>
            {
                ContextMenuInstaller.UninstallContextMenu();
                MessageBox.Show("Context menu entries removed.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            };

            // Add controls to form
            this.Controls.Add(installButton);
            this.Controls.Add(uninstallButton);
        }
    }
}