using System;
using System.Collections.Generic;
using System.Linq;

namespace PassExt.Models
{
    public class StartSequence
    {
        public StartSequence()
        {
            Elements = new List<SeqElem>();
            DayStr = "0000000";
        }

        public string ID { get; set; }
        public DateTime FirstDate { get; set; }
        public string strDate { get { return this.FirstDate.ToString("dd.MM.yyyy"); } }
        public List<SeqElem> Elements { get; set; }
        public string DayStr { get; set; }
        public int DaysCount
        {
            get
            {
                int i = 0;
                foreach (char item in this.DayStr.ToArray())
                {
                    if (item == '1')
                    {
                        i++;
                    }
                }
                return i;
            }
        }
        //КЛАСС ЭЛЕМЕНТОВ ПОСЛЕДОВАТЕЛЬНОСТИ
        public class SeqElem
        {
            public int Number { get; set; }
            public DateTime ElemDate { get; set; }
            public string strDate { get { return this.ElemDate.ToString("dd.MM.yyyy"); } }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
        }
        //МЕТОДЫ
        public List<int[]> GetSeqMatrix()
        {
            List<int[]> matrix = new List<int[]>();
            char[] Week = this.DayStr.ToArray();
            int i = 0;
            int CurSeqElem = 0; //Номер текущего элемента графика
            do
            {
                int DayIndex = 0; //Индекс дня текущей недели
                int ShedDayNumber = 0; //Номер дня в который есть поездки (относительно количества дней, в которые есть поездки)
                matrix.Add(new int[7] { 0, 0, 0, 0, 0, 0, 0 });
                foreach (char Day in Week)
                {
                    if (Day == '1')
                    {
                        ShedDayNumber++;
                        CurSeqElem++;
                        matrix[i][DayIndex] = CurSeqElem;
                        if (CurSeqElem == this.Elements.Count) CurSeqElem = 0;
                    }
                    DayIndex++;
                }
                i++;
            } while (matrix.Count < 2 || !matrix[0].SequenceEqual(matrix[matrix.Count - 1]));
            matrix.RemoveAt(matrix.Count - 1);
            return matrix;
        }
        public SeqElem GetSeqTime(DateTime ForDate)
        {
            int Days = Math.Abs((ForDate - this.FirstDate).Days);
            List<int[]> matrix = this.GetSeqMatrix();
            int Row = ((Days / 7) % matrix.Count);
            SeqElem res = this.Elements.Where(elem => elem.Number == (matrix[Row])[Functions.OrderDayOfWeek(ForDate.DayOfWeek)]).First();
            return res;
        }
    }
}