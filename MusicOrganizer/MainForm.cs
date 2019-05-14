using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
namespace MusicOrganizer
{
    public partial class MainForm : Form
    {
        private Organizer organizer;
        private int SELECTED_BITRATE { get; set; }
        private string SELECTED_AUDIOEXTENSION { get; set; }
        private string SELECTED_DIRECTORY { get; set; }

        private int SELECTED_TREE_DEEPTH = 3;

        

        public MainForm()
        {
            InitializeComponent();
            organizer = new Organizer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.Text = "MusicOrganizer";
            this.label1.Text = "kbits/s:";

            //Button zum organisieren der Dateien
            this.button1.Text = "Verschieben";
            this.button1.MouseClick += new MouseEventHandler(this.button1_Click);

            //Button für die Auswahl des Verzeichnisses
            this.button2.Text = "Verzeichnis wählen";
            this.button2.MouseClick += new MouseEventHandler(this.treeView_selectDirectory);


            // Kbits auswählen
            this.numericUpDown1.Minimum = 100;
            this.numericUpDown1.Maximum = 1000;
            this.SELECTED_BITRATE = int.Parse(numericUpDown1.Value.ToString());
            this.numericUpDown1.ValueChanged += new EventHandler(this.changeFilterOfBitrateAndUpate);
            //ComboBox für Musikformate
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox1.SelectedIndex = 0;
            this.comboBox1.SelectedIndexChanged += new EventHandler(this.changeFileExtensionAndUpdate);
            this.SELECTED_AUDIOEXTENSION = comboBox1.Text;
            
            //Checkbox für Unterverzeichnisse
            this.checkBox1.Text = "Unterverzeichnisse einbeziehen";
            this.checkBox1.Hide();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.treeView1.Nodes.Count == 0)
            {
                MessageBox.Show("Vorher ein Verzeichnis wählen");
            }
            else if(!organizer.hasFilteredTracks())
            {
                MessageBox.Show("Wählen Sie einen anderen Filter");
            }
            else
            {
                this.organizer.moveFilteredFilesToAnotherDirectory(SELECTED_BITRATE);
                updateTreeView();
            }
        }
    
        private void treeView_selectDirectory(object sender, EventArgs e)
        {
                FolderBrowserDialog objDialog = new FolderBrowserDialog();
                objDialog.ShowNewFolderButton = false;
                objDialog.Description = "Verzeichnis mit Musik auswählen";
                objDialog.SelectedPath = @"C:\";       // Vorgabe Pfad (und danach der gewählte Pfad)
                DialogResult objResult = objDialog.ShowDialog(this);
                if (objResult == DialogResult.OK)
                {
                    this.SELECTED_DIRECTORY = objDialog.SelectedPath;
                    this.ListDirectory(this.treeView1, objDialog.SelectedPath, SELECTED_TREE_DEEPTH);
                    
                }
        }

       
        private void ListDirectory(TreeView treeView, string path, int deepth)
        {
            treeView.Nodes.Clear();
            this.organizer.clear();
            var stack = new Stack<TreeNode>();
            var rootDirectory = new DirectoryInfo(path);
            var node = new TreeNode(rootDirectory.Name) { Tag = rootDirectory };
            stack.Push(node);

            try
            {
                while (stack.Count > 0)
                {
                    var currentNode = stack.Pop();
                    var directoryInfo = (DirectoryInfo)currentNode.Tag;

                    foreach (var directory in directoryInfo.GetDirectories())
                    {
                        var childDirectoryNode = new TreeNode(directory.Name) { Tag = directory };
                        currentNode.Nodes.Add(childDirectoryNode);
                        if (stack.Count <= deepth)
                        {
                            stack.Push(childDirectoryNode);
                        }

                    }
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        if (file.Extension.Equals(SELECTED_AUDIOEXTENSION))
                        {
                            int bitrate = this.organizer.extractBitrateFromFile(file.FullName);

                           TreeNode fileNode = new TreeNode(file.Name + " (" + bitrate + " kbit/s)");
                           if(bitrate == this.SELECTED_BITRATE)
                            {
                                fileNode.ForeColor = System.Drawing.Color.Green;

                                //file to organizer
                                this.organizer.AddTrack(file.FullName);
                            }

                            currentNode.Nodes.Add(fileNode);
                        }
                    }
                }
            }catch(System.UnauthorizedAccessException)
            {
                MessageBox.Show("Unauthorisierter Zugriff auf Systemverzeichnisse!");
                treeView.Nodes.Clear();
                return;
            }
            
            treeView.Nodes.Add(node);
        }




        private void changeFileExtensionAndUpdate(object sender, EventArgs e)
        {
            this.SELECTED_AUDIOEXTENSION = comboBox1.Text;
            
            if (SELECTED_DIRECTORY != null)
            {
                updateTreeView();
            }
            
            
        }

        private void changeFilterOfBitrateAndUpate(object sender, EventArgs e)
        {
            this.SELECTED_BITRATE = int.Parse(numericUpDown1.Value.ToString());
            if (SELECTED_DIRECTORY != null)
            {
                updateTreeView();
            }
        }

        private void updateTreeView()
        {
            this.ListDirectory(this.treeView1, SELECTED_DIRECTORY, SELECTED_TREE_DEEPTH);
        }

    }
}

