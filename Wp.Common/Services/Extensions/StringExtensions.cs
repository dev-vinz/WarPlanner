namespace Wp.Common.Services.Extensions
{
	public static class StringExtensions
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STRING METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Capitalize first letter of string
		/// </summary>
		/// <param name="value">A string</param>
		/// <returns>A capitalized string</returns>
		public static string Capitalize(this string value)
		{
			if (value.Length < 1)
			{
				return value;
			}
			else if (value.Length == 1)
			{
				return value.ToUpper();
			}
			else
			{
				return $"{char.ToUpper(value[0])}{value[1..]}";
			}
		}
	}
}
