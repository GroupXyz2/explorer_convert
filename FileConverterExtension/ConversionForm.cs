using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FileConverterExtension
{
    public class ConversionForm : Form
    {
        private readonly string _sourceFilePath;
        private ComboBox _formatComboBox;
        private Button _convertButton;
        private Button _cancelButton;
        private CheckBox _replaceOriginalCheckBox;
        private ProgressBar _progressBar;
        private Label _statusLabel;

        public ConversionForm(string filePath)
        {
            _sourceFilePath = filePath;
            InitializeComponent();
            LoadSupportedFormats();
        }

        private void InitializeComponent()
        {
            this.Text = "Convert File with FFmpeg";
            this.Size = new Size(450, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var sourceLabel = new Label
            {
                Text = $"File: {Path.GetFileName(_sourceFilePath)}",
                Location = new Point(20, 20),
                AutoSize = true,
                MaximumSize = new Size(410, 0)
            };
            this.Controls.Add(sourceLabel);

            var formatLabel = new Label
            {
                Text = "Convert to format:",
                Location = new Point(20, 60),
                AutoSize = true
            };
            this.Controls.Add(formatLabel);

            _formatComboBox = new ComboBox
            {
                Location = new Point(20, 85),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(_formatComboBox);

            _replaceOriginalCheckBox = new CheckBox
            {
                Text = "Replace original file after conversion",
                Location = new Point(20, 120),
                AutoSize = true,
                Checked = true
            };
            this.Controls.Add(_replaceOriginalCheckBox);

            _progressBar = new ProgressBar
            {
                Location = new Point(20, 150),
                Width = 400,
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };
            this.Controls.Add(_progressBar);

            _statusLabel = new Label
            {
                Location = new Point(20, 150),
                AutoSize = true,
                Text = ""
            };
            this.Controls.Add(_statusLabel);

            _convertButton = new Button
            {
                Text = "Convert",
                Location = new Point(250, 180),
                Width = 80
            };
            _convertButton.Click += ConvertButton_Click;
            this.Controls.Add(_convertButton);

            _cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(340, 180),
                Width = 80
            };
            _cancelButton.Click += (s, e) => this.Close();
            this.Controls.Add(_cancelButton);
        }

        private void LoadSupportedFormats()
        {
            var extension = Path.GetExtension(_sourceFilePath).ToLower();
            
            if (IsVideoFormat(extension))
            {
                _formatComboBox.Items.AddRange(new object[] 
                { 
                    "mp4", "mkv", "avi", "mov", "webm", "flv", "wmv", "m4v", "mpg", "mpeg",
                    "-- Audio Extract --",
                    "mp3", "aac", "flac", "wav", "ogg", "m4a", "opus"
                });
            }
            else if (IsAudioFormat(extension))
            {
                _formatComboBox.Items.AddRange(new object[] 
                { 
                    "mp3", "aac", "flac", "wav", "ogg", "m4a", "wma", "opus", "alac"
                });
            }
            else if (IsImageFormat(extension))
            {
                _formatComboBox.Items.AddRange(new object[] 
                { 
                    "jpg", "png", "webp", "bmp", "gif", "tiff"
                });
            }
            else
            {
                _formatComboBox.Items.AddRange(new object[] 
                { 
                    "mp4", "mkv", "mp3", "wav", "jpg", "png"
                });
            }

            if (_formatComboBox.Items.Count > 0)
            {
                _formatComboBox.SelectedIndex = 0;
            }
        }

        private bool IsVideoFormat(string extension)
        {
            var videoFormats = new[] { ".mp4", ".mkv", ".avi", ".mov", ".webm", ".flv", ".wmv", ".m4v", ".mpg", ".mpeg" };
            return Array.Exists(videoFormats, fmt => fmt == extension);
        }

        private bool IsAudioFormat(string extension)
        {
            var audioFormats = new[] { ".mp3", ".aac", ".flac", ".wav", ".ogg", ".m4a", ".wma", ".opus", ".alac" };
            return Array.Exists(audioFormats, fmt => fmt == extension);
        }

        private bool IsImageFormat(string extension)
        {
            var imageFormats = new[] { ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".gif", ".tiff" };
            return Array.Exists(imageFormats, fmt => fmt == extension);
        }

        private async void ConvertButton_Click(object? sender, EventArgs e)
        {
            if (_formatComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a target format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string targetFormat = _formatComboBox.SelectedItem.ToString()!;
            
            if (targetFormat.StartsWith("--"))
            {
                MessageBox.Show("Please select a valid format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            bool replaceOriginal = _replaceOriginalCheckBox.Checked;

            _formatComboBox.Enabled = false;
            _replaceOriginalCheckBox.Enabled = false;
            _convertButton.Enabled = false;
            _progressBar.Visible = true;
            _statusLabel.Text = "Converting...";

            try
            {
                var converter = new FFmpegConverter();
                bool success = await converter.ConvertFileAsync(_sourceFilePath, targetFormat, replaceOriginal);

                if (success)
                {
                    MessageBox.Show("Conversion completed successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Conversion failed. Please check if FFmpeg is installed and in PATH.", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ResetUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetUI();
            }
        }

        private void ResetUI()
        {
            _formatComboBox.Enabled = true;
            _replaceOriginalCheckBox.Enabled = true;
            _convertButton.Enabled = true;
            _progressBar.Visible = false;
            _statusLabel.Text = "";
        }
    }
}
