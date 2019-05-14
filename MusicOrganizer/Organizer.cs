using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicOrganizer
{
    class Organizer
    {
        private List<String> filtered_paths;
        private List<Destination> destination_list;

        public struct Destination
        {
            public string filename;
            public string path;
            public string fullpath;

        }

        public Organizer()
        {
            this.filtered_paths = new List<string>();
            this.destination_list = new List<Destination>();
        }

        public void moveFilteredFilesToAnotherDirectory(int kbits)
        {
            this.destination_list.Clear();

            foreach (string trackPath in this.filtered_paths)
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(trackPath);
                Destination dest = constructFileDestinationFromSource(kbits, di.Parent.FullName, di.Name);
                destination_list.Add(dest);
                
            }
            moveFiles();
            
        }

        public void moveFiles()
        {

            if(filtered_paths.Count == destination_list.Count)
            {
                int SUM_OF_ALL_FILES = filtered_paths.Count;
                for(int file = 0; file < SUM_OF_ALL_FILES; file++)
                {
                    string src = filtered_paths.ElementAt(file);
                    Destination dest = destination_list.ElementAt(file);

                    try
                    {
                        if (System.IO.File.Exists(dest.fullpath))
                        {
                            System.IO.File.Delete(dest.fullpath);
                        }

                        if (!System.IO.Directory.Exists(dest.path))
                        {
                            System.IO.Directory.CreateDirectory(dest.path);
                        }
                        
                        System.IO.File.Move(src, dest.fullpath);
                    }
                    catch (Exception ex)
                    {
                        string corrupt_data = string.Format("Die Datei {0} konnte nicht verschoben werden", System.IO.Path.GetFileName(src));
                        System.Windows.Forms.MessageBox.Show(corrupt_data);
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        return;
                    }
                }
                Program.displayMessage("Dateien wurden verschoben");
                
            }
            else
            {
                Program.displayMessage("err(src.count != dest.count)");
            }
            
                
            
        }
        
        public Destination constructFileDestinationFromSource(int kbits, string parentDirectory, string trackname)
        {
            //src/ParentDirectory_(kbits)/filename.{mp3|wav|flac}
            Destination destination;
            destination.filename = trackname;
            destination.fullpath = string.Format("{0}_{1}_kbits\\{2}", parentDirectory, kbits, trackname);
            destination.path = string.Format("{0}_{1}_kbits", parentDirectory, kbits);
            return destination;
        }

        public void AddTrack(string filename)
        {
            this.filtered_paths.Add(filename);
        }

        public bool hasFilteredTracks()
        {
            return (filtered_paths.Count > 0);
        }

        public void clear()
        {
            this.filtered_paths.Clear();
            this.destination_list.Clear();
        }

        public int extractBitrateFromFile(string filepath)
        {
            TagLib.File datei = TagLib.File.Create(filepath);
            int bitrate = datei.Properties.AudioBitrate;
            return bitrate;
        }
    }
}
