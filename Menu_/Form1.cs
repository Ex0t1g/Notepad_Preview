using System.Diagnostics;
using System.Drawing.Design;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Menu_
{
    public partial class Form1 : Form
    {
        private Stack<string> undoStack = new Stack<string>();
        private Stack<string> redoStack = new Stack<string>();
        private string clipboardText;
        private float currentZoomFactor = 1.0f;
        private const float ZOOM_STEP = 1.1f;
        private bool isSaved = false;
        private string currentFileName = "Untitled.txt";
        public Form1()
        {
            InitializeComponent();
            undoStack = new Stack<string>();
            redoStack = new Stack<string>();
            UpdateFormTitle();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            if (textBox1.Text == "")
            {
                this.Text = $"{currentFileName} *";
                UpdateFormTitle();
            }
            toolStripStatusLabel1.Text = "Создан новый документ";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new();
            dialog.Filter = "Text Files (*.txt)|*.txt|All file(*.*)|*.*";
            dialog.Title = "Choose a file";

            if (dialog.ShowDialog() != DialogResult.OK) return;
            //using StreamReader  reader = File.OpenText(dialog.FileName);

            //textBox1.Text = reader.ReadToEnd();

            textBox1.Text = File.ReadAllText(dialog.FileName);
        }

        private void referenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string helpFilePath = Path.Combine(Application.StartupPath, "help.html");

            if (File.Exists(helpFilePath))
            {
                Process.Start("explorer.exe", helpFilePath);
            }
            else
            {
                MessageBox.Show("Help file is not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void SaveFileAs()
        {
            using SaveFileDialog dialogSave = new();
            dialogSave.Filter = "Text Files(*.txt) | *.txt | All file(*.*) | *.*";
            dialogSave.Title = "Choose where you want to save";
            dialogSave.FileName = "Untitled";

            if (dialogSave.ShowDialog() != DialogResult.OK)
            {
                currentFileName = dialogSave.FileName;
                File.WriteAllText(dialogSave.FileName, textBox1.Text);
                isSaved = true;
                UpdateFormTitle();

            }

        }
        private void SaveFile()
        {
            if (currentFileName == "Untitled.txt")
            {
                SaveFileAs();
                UpdateFormTitle();
                isSaved = true;

            }
            else
            {
                File.WriteAllText(currentFileName, textBox1.Text);
                UpdateFormTitle();
                isSaved = true;
            }
        }
        private void UpdateFormTitle()
        {
            if (isSaved)
            {
                this.Text = currentFileName;
            }
            else
            {
                this.Text = $"{currentFileName} *";
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using FontDialog dialogFont = new();

            if (dialogFont.ShowDialog() != DialogResult.OK) return;

            textBox1.Font = dialogFont.Font;
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using ColorDialog colorDialog = new();
            if (colorDialog.ShowDialog() != DialogResult.OK) return;
            textBox1.ForeColor = colorDialog.Color;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 1)
            {
                redoStack.Push(undoStack.Pop());

                textBox1.Text = undoStack.Peek();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                undoStack.Push(textBox1.Text);

                textBox1.Text = redoStack.Pop();
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            undoStack.Push(textBox1.Text);
            redoStack.Clear();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1.SelectionLength > 0)
            {
                clipboardText = textBox1.SelectedText;
                textBox1.SelectedText = "";
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1.SelectionLength > 0)
            {
                textBox1.SelectedText = "";

                undoStack.Push(textBox1.Text);
                redoStack.Clear();
            }
        }

        private void increaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentZoomFactor *= ZOOM_STEP;
            UpdateZoomFactor();
        }

        private void reduceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentZoomFactor /= ZOOM_STEP;
            UpdateZoomFactor();
        }

        private void returnItAsItWasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentZoomFactor = 10.0f;
            UpdateZoomFactor();
        }
        private void UpdateZoomFactor()
        {
            currentZoomFactor = Math.Max(0.01f, Math.Min(100.0f, currentZoomFactor));

            textBox1.Font = new System.Drawing.Font(textBox1.Font.FontFamily, 12.0f * currentZoomFactor, textBox1.Font.Style);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!isSaved)
            {
                DialogResult result = MessageBox.Show(
                    "You have unsaved changes. Do you want to save before closing?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

            base.OnFormClosing(e);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void timeAndDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertTimeAndDate();
        }
        private void InsertTimeAndDate()
        {
            string timeAndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int selectionStart = textBox1.SelectionStart;
            textBox1.Text = textBox1.Text.Insert(selectionStart, timeAndDate);
            textBox1.Select(selectionStart + timeAndDate.Length, 0);
        }
        private void SelectAll()
        {
            textBox1.SelectAll();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void Copy()
        {
            textBox1.Copy();
        }

        private void Paste()
        {
            textBox1.Paste();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();
        }
    }
}