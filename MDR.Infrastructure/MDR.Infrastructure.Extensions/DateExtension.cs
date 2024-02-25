namespace MDR.Infrastructure.Extensions;

public static class DateExtensions
{
    /// <summary>
    /// 计算年数 年龄计算有问题，计算方式为，如果大于10岁则显示【15岁】，小于10岁大于1岁显示【5岁11月】，小于1岁大于1月显示【11月15天】，小于1月显示【15天22小时】
    /// </summary>
    /// <param name="dtBirthday"></param>
    /// <returns></returns>
    public static string GetAgeForDate(this DateTime? dtBirthday)
    {
        string result = "";
        DateTime dtNow = DateTime.Now;

        // 如果没有设定出生日期, 返回空
        if (dtBirthday == null)
        {
            return string.Empty;
        }

        var dtBirthdaytmp = dtBirthday.CastTo<DateTime>();
        // 计算分钟
        var intMin = dtNow.Minute - dtBirthdaytmp.Minute;
        if (intMin < 0)
        {
            intMin += 60;
            dtNow = dtNow.AddMinutes(-1);
        }

        // 计算小时
        var intHour = dtNow.Hour - dtBirthdaytmp.Hour;
        if (intHour < 0)
        {
            intHour += 24;
            dtNow = dtNow.AddHours(-1);
        }

        // 计算天数
        var intDay = dtNow.Day - dtBirthdaytmp.Day;
        if (intDay < 0)
        {
            intDay += DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
            dtNow = dtNow.AddMonths(-1);
        }

        // 计算月数
        var intMonth = dtNow.Month - dtBirthdaytmp.Month;
        if (intMonth < 0)
        {
            intMonth += 12;
            dtNow = dtNow.AddYears(-1);
        }

        // 计算年数 年龄计算有问题，计算方式为，如果大于10岁则显示【15岁】，小于10岁大于1岁显示【5岁11月】，小于1岁大于1月显示【11月15天】，小于1月显示【15天22小时】
        var intYear = dtNow.Year - dtBirthdaytmp.Year;
        if (intYear < 0)
            return "";
        if (intYear >= 10)
            result = intYear + "岁";
        else if (intYear >= 1 && intYear < 10)
            result = intYear + "岁" + (intMonth > 0 ? intMonth + "月" : "");
        else if (intYear < 1 && intYear + intMonth >= 1)
            result = (intMonth > 0 ? intMonth + "月" : "") + (intDay > 0 ? intDay + "天" : "");
        else if (intYear + intMonth < 1)
            result = (intDay > 0 ? intDay + "天" : "") + intHour + "小时";

        return result;
    }

    public static string GetAge(this DateTime dtBirthday)
    {
        return GetAgeForDate(dtBirthday);
    }
}