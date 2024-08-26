using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace komax
{
    public partial class Form1 : Form
    {
        string zetaDDS = "";
        int TUBELENGTH = 35;


        public Form1()
        {
            InitializeComponent();
        }

        // Browse button
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "DDS Files (*.dds) | *.dds| All Files (*.*)| *.*";

            // If the user selects a file and clicks "OK"
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                zetaDDS = dialog.FileName;
                textBox1.Text = dialog.FileName;

                string fileContent = ReadFile(zetaDDS);
                textBox2.Text = fileContent;
            }
        }

        private string ReadFile(string path)
        {
            try
            {
                return File.ReadAllText(path);

                //string[] lines = File.ReadAllLines(path);
                //richTextBox1.Clear();

                //foreach (string line in lines)
                //{
                //    if (line.Contains("TubeMarking="))
                //    {
                //        int start = richTextBox1.TextLength;
                //        richTextBox1.AppendText(line + Environment.NewLine);
                //        int end = richTextBox1.TextLength;

                //        richTextBox1.Select(start, end - start);
                //        richTextBox1.SelectionBackColor = System.Drawing.Color.Yellow;
                //    }
                //    else
                //    {
                //        richTextBox1.AppendText(line + Environment.NewLine);
                //    }
                //}

                //richTextBox1.Select(0, 0); // Reset selection
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}");
                return string.Empty;
            }
        }
        
        private string ZetaToOmega(string path)
        {
            string[] lines = File.ReadAllLines(path);
            string everything = "";


            foreach (string line in lines)
            {
                if (line.Contains("TubeMarking="))
                {
                    string[] parts = line.Split(',');

                    // Extract the relevant parts
                    string tubeMarking = parts[0].Split('=')[1];
                    string tubeMarkingLayout = parts[1].Trim('"');
                    string tubeMarkingText1 = parts[2].Trim('"');
                    string tubeMarkingText2 = parts[4].Trim('"');
                    //string tubeLength = parts[6].Trim('"');

                    string output =
                        $@"        [NewTubeMarkingEnd{tubeMarking}]
            TubeMarkingLayout={tubeMarkingLayout}
            TubeLength={TUBELENGTH}
            TubeMarkingText1=""{tubeMarkingText1}""
            TubeMarkingText2=""{tubeMarkingText2}""
            TubeMarkingTextSize=1.5";

                    everything += output + Environment.NewLine;
                }
                else if (line.Contains("FontKey=") || line.Contains("BundlingSide=") || line.Contains("BundlingPostProcess="))
                {
                    continue;
                }
                else if (line.Contains("TerminalKey="))
                {
                    string output = Environment.NewLine +
                         "\t\tFontKey=Default" + Environment.NewLine +
                         "\t\tBundlingSide=1" + Environment.NewLine +
                         "\t\tBundlingPostProcess=1";

                    everything += line + output + Environment.NewLine;
                }
                else
                {
                    everything += line + Environment.NewLine;
                }
            }

            return everything;
        }

        // Save As button
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            // If the user selects a file and clicks "OK"
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = dialog.SelectedPath + "\\Article.dds";
            }
        }

        // EXIT button
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure to exit the application?", "Exit Application", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // Export button
        private void button3_Click(object sender, EventArgs e)
        {
            // First check that the user chose a file path
            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show($"User did not choose a valid file path. Did not export to file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create the full file path for the exported file
            string exportFilePath = Path.Combine(textBox4.Text);
            SaveTextBoxContentToDDS(exportFilePath, textBox3.Text);

            MessageBox.Show($"Data Exported: \n {exportFilePath}", "Result", MessageBoxButtons.OK);
        }

        private void SaveTextBoxContentToDDS(string filePath, string content)
        {
            try
            {
                // Save the content to a DDS file (plain text with .dds extension)
                File.WriteAllText(filePath, content);

                MessageBox.Show("Content saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving content: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Convert button
        private void button5_Click(object sender, EventArgs e)
        {
            string output = ZetaToOmega(zetaDDS);
            textBox3.Text = output;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

}

