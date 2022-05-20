using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApplication1
{
    struct importStruct
    {
        public string clipping;
        public string note;
        public string chapter;
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var path = Directory.GetCurrentDirectory();
            var fi = new DirectoryInfo(path);
            var fileinfos = fi.GetFiles("*anki.txt", SearchOption.AllDirectories);
            string bookName = "";
            string authorName = "";
            string currentChapter = "";
            string currentNote = "";
            Dictionary<string, importStruct> linesData = new Dictionary<string, importStruct>();
            foreach (var file in fileinfos)
            {
                using (StreamReader sr = file.OpenText())
                {
                    string s;
                    int line = 0;
                    while ((s = sr.ReadLine()) != null)
                    {
                        line++;
                        if (line == 1)
                        {
                            bookName = s;
                        }
                        else if (line == 2)
                        {
                            authorName = s;
                        }
                        else if (s.StartsWith("◆"))
                        {
                            currentChapter = s;
                        }
                        else if (s.StartsWith(">>") is bool condition1 && condition1 || s.StartsWith(">"))
                        {
                            if (condition1)
                            {
                                s = s.Substring(2);
                            }
                            else
                            {
                                s = s.Substring(1);
                            }

                            s = s.Trim();
                            if (!linesData.ContainsKey(s))
                            {
                                importStruct @struct;
                                @struct.clipping = s;
                                @struct.note = currentNote;
                                @struct.chapter = currentChapter;
                                linesData.Add(s, @struct);
                                if (!string.IsNullOrEmpty(currentNote))
                                {
                                    currentNote = "";
                                }
                            }
                        }
                        else if (line > 3)
                        {
                            currentNote = s;
                        }
                    }
                }
            }

            File.WriteAllText($"{path}/result.txt",
                string.Join("\n", linesData.ToList().Select(data => 
                    string.Join(",", data.Value.clipping, data.Value.note, bookName + " " + data.Value.chapter, authorName, bookName
                    )).ToList()));
        }
    }
}
