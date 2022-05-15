using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace TOGIRRO_ControlTesting
{
    //========================================================================================================================================
    //===Вспомогательный класс для чтения INI файлов==========================================================================================
    //========================================================================================================================================
    #region
    class INIReader
    {
        private readonly string Path;
        private readonly string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        //Импорты из системных библиотек
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        //Конструктор
        public INIReader(string IniPath = null) => Path = new FileInfo(IniPath ?? EXE + ".ini").FullName;

        //Прочитать ключ
        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        //Записать ключ
        public void Write(string Key, string Value, string Section = null) => WritePrivateProfileString(Section ?? EXE, Key, Value, Path);

        //Удалить ключ
        public void DeleteKey(string Key, string Section = null) => Write(Key, null, Section ?? EXE);

        //Удалить секцию
        public void DeleteSection(string Section = null) => Write(null, null, Section ?? EXE);

        //Проверка на наличие ключа
        public bool KeyExists(string Key, string Section = null) => Read(Key, Section).Length > 0;
    }
    #endregion
    //========================================================================================================================================
}