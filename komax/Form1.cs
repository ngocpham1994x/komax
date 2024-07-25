﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace komax
{
    public partial class Form1 : Form
    {
        string zetaDDS = "";


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
        
        private void ZetaToOmega(string path)
        {
            string[] lines = File.ReadAllLines(path);

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
                    string tubeLength = parts[6].Trim('"');

                    string output = 
                        $@"        [NewTubeMarkingEnd{tubeMarking}]
                        TubeMarkingLayout={tubeMarkingLayout}
                        TubeLength={tubeLength}
                        TubeMarkingText1=""{tubeMarkingText1}""
                        TubeMarkingText2=""{tubeMarkingText2}""
                        TubeMarkingTextSize=1.5";

                    textBox3.AppendText(output + Environment.NewLine);
                }
                else
                {
                    textBox3.AppendText(line+ Environment.NewLine);
                }
            }

        }

        // Save As button
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            // If the user selects a file and clicks "OK"
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = dialog.SelectedPath;
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
            string exportFilePath = Path.Combine(textBox4.Text, "Article.dds");
            SaveTextBoxContentToDDS(exportFilePath, textBox3.Text);

            MessageBox.Show($"Data Exported: \n {exportFilePath}", "Result", MessageBoxButtons.OK);
        }

        public static void SaveTextBoxContentToDDS(string filePath, string content)
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
            ZetaToOmega(zetaDDS);
        }
    }


}


