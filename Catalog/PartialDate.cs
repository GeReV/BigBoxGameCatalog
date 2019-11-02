using System;

namespace Catalog
{
    public struct PartialDate
    {
        public PartialDate(DateTime date, PartialDateMask dateMask)
        {
            Date = date;
            DateMask = dateMask;
        }

        public DateTime Date { get; set; }
        public PartialDateMask DateMask { get; set; }

        public override string ToString()
        {
            switch (DateMask)
            {
                case PartialDateMask.Year:
                    return Date.ToString("yyyy");
                case PartialDateMask.YearMonth:
                    return Date.ToString("yyyy-MM");
                case PartialDateMask.YearMonthDay:
                    return Date.ToString("yyyy-MM-dd");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}