namespace GenAnkiData
{
    struct ImportStruct
    {
        public string Clipping;
        public string Note;
        public string Chapter;
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
            Dictionary<string, ImportStruct> linesData = new Dictionary<string, ImportStruct>();
            foreach (var file in fileinfos)
            {
                using (StreamReader streamReader = file.OpenText())
                {
                    string s;
                    int lineIndex = 0;
                    while ((s = streamReader.ReadLine()) != null)
                    {
                        lineIndex++;
                        if (lineIndex == 1)
                        {
                            bookName = s;
                        }
                        else if (lineIndex == 2)
                        {
                            authorName = s;
                        }
                        else if (s.StartsWith("◆")) //新章节
                        {
                            currentChapter = s;
                        }
                        //这里必须要这么写？ s.StartsWith(">>") is bool condition1 || s.StartsWith(">") 会永远进入判断
                        else if (s.StartsWith(">>") is bool condition1 && condition1 || s.StartsWith(">")) //句子
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
                            if (linesData.TryGetValue(s, out  ImportStruct data))
                            {
                                data.Note = data.Note + "\n" + currentNote;
                            }
                            else
                            {
                                ImportStruct value;
                                value.Clipping = s;
                                value.Note = currentNote;
                                value.Chapter = currentChapter;
                                linesData.Add(s, value);
                                if (!string.IsNullOrEmpty(currentNote))
                                {
                                    currentNote = "";
                                }
                            }
                        }
                        else if (lineIndex > 3)
                        {
                            currentNote = s;
                        }
                    }
                }
            }

            File.WriteAllText($"{path}/result.txt",
                string.Join("\n", linesData.ToList().Select(data =>
                    string.Join(",", data.Value.Clipping, data.Value.Note, bookName + " " + data.Value.Chapter,
                        authorName, bookName
                    )).ToList()));
        }
    }
}
