using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LabelApp.forms
{
    public class LabelManagerForm : Form
    {
        private ListBox labelListBox;
        private TextBox newLabelTextBox;
        private Button addButton;
        private Button removeButton;

        public LabelManagerForm()
        {
            InitializeComponents();
            LoadLabels();
        }

        private void InitializeComponents()
        {
            this.Text = "Label Manager";
            this.Size = new Size(400, 280);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = SystemFonts.MessageBoxFont;

            this.labelListBox = new ListBox 
            { 
                Top = 10, 
                Left = 10, 
                Width = 360, 
                Height = 150, 
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right 
            };
            this.newLabelTextBox = new TextBox 
            { 
                Top = 170, 
                Left = 10, 
                Width = 260, 
                Anchor = AnchorStyles.Left | AnchorStyles.Right 
            };
            this.addButton = new Button 
            { 
                Text = "Add", 
                Top = 170, 
                Left = 280, 
                Width = 90, 
                Anchor = AnchorStyles.Top | AnchorStyles.Right 
            };
            this.removeButton = new Button 
            { 
                Text = "Remove Selected",
                Top = 200, Left = 10, 
                Width = 360,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            this.addButton.Click += (s, e) =>
            {
                string newLabel = this.newLabelTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(newLabel) && !this.labelListBox.Items.Contains(newLabel) && !DatabaseHandler.LabelExists(newLabel))
                {
                    DatabaseHandler.CreateNewLabel(newLabel);
                    ContextMenuManager.AddLabelEntries(newLabel);
                    this.labelListBox.Items.Add(newLabel);
                    this.newLabelTextBox.Clear();
                }
            };

            this.removeButton.Click += (s, e) =>
            {
                if (this.labelListBox.SelectedItem != null)
                {
                    string labelName = this.labelListBox.SelectedItem.ToString().Trim();
                    ContextMenuManager.RemoveLabelEntries(labelName);
                    DatabaseHandler.DeleteExistingLabel(labelName);
                    this.labelListBox.Items.Remove(this.labelListBox.SelectedItem);
                }
            };


            this.Controls.Add(this.labelListBox);
            this.Controls.Add(this.newLabelTextBox);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.removeButton);
        }

        private void LoadLabels()
        {
            try
            {
                var labels = DatabaseHandler.GetAllLabels();
                foreach (var label in labels)
                    this.labelListBox.Items.Add(label);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading labels: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}