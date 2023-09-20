using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace MaxArhivator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderDialog.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Выберите папку для архивации.", "Ошибка");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Zip files (*.zip)|*.zip";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream fsOut = File.Create(saveDialog.FileName))
                    using (ZipOutputStream zipStream = new ZipOutputStream(fsOut))
                    {
                        zipStream.SetLevel(9); // Максимальный уровень сжатия (от 0 до 9)

                        string sourceDirectory = textBox1.Text;
                        string[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);

                        byte[] buffer = new byte[4096];

                        foreach (string file in files)
                        {
                            FileInfo fileInfo = new FileInfo(file);

                            string entryName = file.Substring(sourceDirectory.Length + 1); // Получаем относительный путь

                            ZipEntry newEntry = new ZipEntry(entryName);
                            newEntry.DateTime = DateTime.Now;

                            zipStream.PutNextEntry(newEntry);

                            using (FileStream fs = File.OpenRead(file))
                            {
                                StreamUtils.Copy(fs, zipStream, buffer);
                            }
                            zipStream.CloseEntry();
                        }
                    }

                    MessageBox.Show("Архивация прошла успешно.", "Выполнено");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при создании архива: {ex.Message}", "Ошибка");
                }
            }
        }
    }
}
