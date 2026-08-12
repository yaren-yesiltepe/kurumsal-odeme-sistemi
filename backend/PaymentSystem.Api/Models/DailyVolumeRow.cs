namespace PaymentSystem.Api.Models;

public class DailyVolumeRow
{
    public DateTime TxnDate { get; set; }
    public decimal TotalAmount { get; set; }
    public int TxnCount { get; set; }
    public decimal? MovingAvg7Day { get; set; }
    public decimal RunningTotal { get; set; }
    public decimal? PrevDayAmount { get; set; }
    public decimal? DayOverDayChangePct { get; set; }
}
