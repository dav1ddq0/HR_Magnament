namespace HR_API.Utils
{
    public class Tools
    {
        public static double IncreaseByRole(List<string> roles)
        {
            if (roles.Contains("Manager"))
                return 0.12;
            if (roles.Contains("Specialist"))
                return 0.08;
            return 0.05;
        }

        public static int GetMonthsDifference(DateTime startDate, DateTime endDate)
        {
            int monthsDifference =
                (endDate.Year - startDate.Year) * 12
                + endDate.Month
                - startDate.Month
                + (endDate.Day < startDate.Day ? -1 : 0);

            return monthsDifference;
        }
    }
}
