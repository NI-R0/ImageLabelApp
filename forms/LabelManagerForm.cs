using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ImageLabelApp.forms
{
    public class LabelManagerForm : Form
    {
        private readonly string imagePath;
        private ListBox labelListBox;
        private TextBox newLabelTextBox;
        private Button addButton;
        private Button removeButton;
        private Button saveButton;

        public LabelManagerForm(string imagePath)
        {
            this.imagePath = imagePath;
            InitializeComponents();
            LoadLabels();
            // Remove this message box - it might be causing issues
            // MessageBox.Show("Window loaded", "Success", MessageBoxButtons.OK);
        }

        private void InitializeComponents()
        {
            // Set form properties for proper display
            this.Text = "Label Manager";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // Initialize controls
            this.labelListBox = new ListBox { Top = 10, Left = 10, Width = 360, Height = 150, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            this.newLabelTextBox = new TextBox { Top = 170, Left = 10, Width = 260, Anchor = AnchorStyles.Left | AnchorStyles.Right };
            this.addButton = new Button { Text = "Add", Top = 170, Left = 280, Width = 90, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            this.removeButton = new Button { Text = "Remove Selected", Top = 200, Left = 10, Width = 360, Anchor = AnchorStyles.Left | AnchorStyles.Right };
            this.saveButton = new Button { Text = "Save & Close", Top = 230, Left = 10, Width = 360, Anchor = AnchorStyles.Left | AnchorStyles.Right };

            // Set up event handlers
            this.addButton.Click += (s, e) =>
            {
                string newLabel = this.newLabelTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(newLabel) && !this.labelListBox.Items.Contains(newLabel))
                {
                    this.labelListBox.Items.Add(newLabel);
                    this.newLabelTextBox.Clear();
                }
            };

            this.removeButton.Click += (s, e) =>
            {
                if (this.labelListBox.SelectedItem != null)
                    this.labelListBox.Items.Remove(this.labelListBox.SelectedItem);
            };

            this.saveButton.Click += (s, e) =>
            {
                LabelDatabase.ClearLabels(this.imagePath);
                foreach (var label in this.labelListBox.Items)
                {
                    LabelDatabase.AddLabel(this.imagePath, label.ToString());
                    ShortcutManager.CreateShortcut(this.imagePath, label.ToString());
                }
                this.Close();
            };

            // Add controls to form
            this.Controls.Add(this.labelListBox);
            this.Controls.Add(this.newLabelTextBox);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.saveButton);
        }

        private void LoadLabels()
        {
            try
            {
                var labels = LabelDatabase.GetAllLabels();
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