using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MyDiary
{
    public class Database
    {
        static DateTime startDate = new DateTime(2022, 1, 1); //çok önemli! başlangıç.
        static string databasePath = Application.StartupPath + @"\appDatas\database.xlsx";
        static string DatabasePath
        {
            get
            {
                if (!File.Exists(databasePath))
                    return ControlCreate();
                return databasePath;
            }
        }  //database olarak kullanılacak pathi döner, dosya yoksa oluşturucu metota uğrar oluşturup döner.

        private static string ControlCreate()
        {
            string path = Path.GetDirectoryName(databasePath); //excel path
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path = path + @"\database.xlsx";
                if (!File.Exists(path))
                {
                    using (XLWorkbook DFile = new XLWorkbook())
                    {
                        DFile.Worksheets.Add("database");
                        IXLWorksheet DSheet = DFile.Worksheet("database");
                        IXLRange range = DSheet.RangeUsed();
                        IXLColumn column = DSheet.Column("A");

                        for (int i = 0; i < 1000; i++)
                            column.Cell(i + 1).Value = startDate.AddDays(i);

                        DFile.SaveAs(databasePath);
                    } //database tablosu oluşturma
                    using (XLWorkbook DFile = new XLWorkbook(databasePath))
                    {
                        DFile.Worksheets.Add("settings");
                        IXLWorksheet DSheet = DFile.Worksheet("settings");
                        IXLRange range = DSheet.RangeUsed();
                        IXLColumn column = DSheet.Column("A");
                        IXLColumn column2 = DSheet.Column("B");

                        string[] colors = { "green", "blue", "yellow", "pink", "surprise" };
                        for (int i = 0; i < colors.Length; i++)
                        {
                            column.Cell(i + 1).Value = colors[i];
                            column2.Cell(i + 1).Value = "checked";
                        }

                        DFile.Save();
                    } //settings tablosu oluşturma
                }
            }
            catch (Exception)
            { }

            return path;
        }  //database pathinde dosya yoksa bu metotla oluşturulur.
        private static int FindIndex(DateTime dt)
        {
            int indeks = 1;
            if (dt.Year == 2022)
                indeks = dt.DayOfYear - startDate.DayOfYear;
            else
                for (int i = 0; i < 1000; i++)
                    if (startDate.AddDays(i) == dt)
                    {
                        indeks = i;
                        break;
                    }

            return indeks + 1;
        }  //databasede aranan tarihin olduğu satır numarasını dönen değer üretir.

        public static List<Mission>[] GetAll(DateTime date)
        {
            List<Mission>[] days = { new List<Mission>(), new List<Mission>(), new List<Mission>(), new List<Mission>(), new List<Mission>(), new List<Mission>(), new List<Mission>() };

            /* başlangıç gününden itibaren 7 günün işlerini veritabanından çekip listelere atar. */
            try
            {
                using (XLWorkbook DFile = new XLWorkbook(DatabasePath))
                {
                    IXLWorksheet worksheet = DFile.Worksheet("database");
                    IXLRange range = worksheet.RangeUsed();
                    int indeks = FindIndex(date);
                    for (int i = 0; i < days.Length; i++)
                    {
                        IXLRow row = worksheet.Row(indeks);
                        for (int j = 1; j < 5; j++)
                        {
                            if (row.Cell(j + 1).Value == null)
                                continue;

                            string cell = row.Cell(j + 1).Value.ToString();
                            if (String.IsNullOrEmpty(cell))
                                continue;

                            Mission miss = new Mission();
                            cell = cell.TrimEnd();
                            miss.isComplete = cell.Contains("_OqE_") ? true : false;
                            miss.mission = miss.isComplete ? cell.Replace("_OqE_", "") : cell;
                            miss.missionDate = Convert.ToDateTime(row.Cell(1).Value.ToString());
                            days[i].Add(miss);
                        }
                        indeks++;
                    }
                    DFile.Save();
                }
            }
            catch (Exception)
            { }

            return days;
        }    //tabloya yüklemek üzere yedi günlük görevler listesini döndürür.
        public static void SetAll(DateTime date, List<Mission>[] days)
        {
            try
            {
                using (XLWorkbook DFile = new XLWorkbook(DatabasePath))
                {
                    IXLWorksheet worksheet = DFile.Worksheet("database");
                    IXLRange range = worksheet.RangeUsed();
                    int indeks = FindIndex(date);
                    for (int i = 0; i < days.Length; i++)
                    {
                        IXLRow row = worksheet.Row(indeks);
                        for (int j = 0; j < days[i].Count; j++)
                        {
                            row.Cell(j + 2).Value = days[i][j].mission;
                            row.Cell(j + 2).Value = (days[i][j].isComplete) ? days[i][j].mission + "_OqE_" : days[i][j].mission;
                        }
                        indeks++;
                    }
                    DFile.Save();
                }
            }
            catch (Exception)
            { }
        }   //burada görevler bellekten databaseye yükleme yapılacak
        public static List<string> GetColors()
        {
            List<string> colors = new List<string>();

            using (XLWorkbook DFile = new XLWorkbook(DatabasePath))
            {
                IXLWorksheet worksheet = DFile.Worksheet("settings");
                IXLRange range = worksheet.RangeUsed();
                int indeks = 1;
                while (true)
                {
                    IXLRow row = worksheet.Row(indeks);
                    if (String.IsNullOrEmpty(row.Cell(2).Value.ToString()))
                        break;

                    string eklenecek = (row.Cell(2).Value.ToString() == "checked") ? row.Cell(1).Value.ToString() : "";
                    if (eklenecek.Length != 0)
                        colors.Add(eklenecek);
                    indeks++;
                }
                DFile.Save();
            } 
            return colors;
        }  //Veritabanından true(aktif) olan renkleri alır.
        public static void SetColors(List<string> colors)
        {
            using (XLWorkbook DFile = new XLWorkbook(DatabasePath))
            {
                IXLWorksheet worksheet = DFile.Worksheet("settings");
                IXLRange range = worksheet.RangeUsed();
                int indeks = 1;
                while (true)
                {
                    IXLRow row = worksheet.Row(indeks);
                    if (String.IsNullOrEmpty(row.Cell(2).Value.ToString()))
                        break;

                    if (colors.Contains(row.Cell(1).Value.ToString()))
                        row.Cell(2).Value = "checked";
                    else
                        row.Cell(2).Value = "unchecked";

                    indeks++;
                }
                DFile.Save();
            }
        }  //Verilen renk listesini veritabanına girer.
    }
    public struct Mission
    {
        /*
         * Excel veri tabanında birinci kolonda tarihler, iki,üç,dört,beşinci kolonlarda görevler bulunacak.
         * Her görev satırının sonunda "_OqE_" deyimi ile complete edilip edilmediği denetlenecek. Tamamlanan görevlere veritabanında bu ifade eklenecek.
         */
        public string mission;
        public bool isComplete;
        public DateTime missionDate;
    }
}
