namespace DatingAppAPI.Extensions
{
  public static class DateTimeExtentions
  {
    public static int CalculateAge(this DateOnly dob)
    {
      var today = DateOnly.FromDateTime( DateTime.UtcNow );

      var  age =today.Year - today.Year;

      if(dob > today.AddYears(-age))
      {
        return age--;
      }

      return age;
    }
  }
}
