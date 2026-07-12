using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode
{
    public sealed class PvfGroup
    {
        public bool PvfIsOpen { get; set; }

        public string? PvfPackFilePath { get; set; }

        public Dictionary<string, PvfFile> FileList { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public ListFileTable ListFileTable { get; } = new();

        public Stringtable Strtable { get; } = new();

        public StringView Strview { get; } = new();

        public string GetFileText(PvfFile file, bool showAniError = true)
        {
            return file.Data == null
                ? string.Empty
                : System.Text.Encoding.UTF8.GetString(file.Data, 0, Math.Min(file.DataLen, file.Data.Length));
        }

        public ResultData<Dictionary<int, LstItem>> GetLstDicTable(PvfFile file)
        {
            return new ResultData<Dictionary<int, LstItem>>
            {
                Data = file.LstEntries
            };
        }
    }

    public sealed class PvfFile
    {
        private string _fileName = string.Empty;

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                FileNameBytes = System.Text.Encoding.UTF8.GetBytes(value);
            }
        }

        public byte[] FileNameBytes { get; set; } = [];

        public byte[]? Data { get; set; }

        public bool IsScriptFile { get; set; }

        public PvfFileType FileType { get; set; }

        public int DataLen { get; set; }

        public uint Checksum { get; set; }

        public bool IsUpdated { get; set; }

        public Dictionary<int, LstItem>? LstEntries { get; set; }

        public PvfFile CloneData()
        {
            return (PvfFile)MemberwiseClone();
        }
    }

    public sealed class ResultData<T>
    {
        public T? Data { get; set; }

        public string? Msg { get; set; }
    }
}

namespace PvfCode.Models.Pvf
{
    public sealed class ListFileTable
    {
        public Dictionary<string, string> LstFilePaths { get; } = new(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, Dictionary<int, LstItem>> CodeDic { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public sealed class Stringtable
    {
        public bool IsStringTableUpdated { get; set; }
    }

    public sealed class StringView
    {
        public object? Files { get; set; }

        public object? Get_pvfstrlist() => Files;
    }

    public sealed class LstItem
    {
        public int ItemCode { get; }

        public string ItemPath { get; }

        public string Header { get; }

        public string FullPath => $"{Header}/{ItemPath.TrimStart('/', '\\')}".ToLowerInvariant().Replace('\\', '/');

        public LstItem(string header, string itemPath, int itemCode)
        {
            Header = header;
            ItemPath = itemPath;
            ItemCode = itemCode;
        }
    }
}

namespace PvfCode.Models.Pvf.Enums
{
    public enum PvfFileType
    {
        Unknown,
        lst
    }
}
