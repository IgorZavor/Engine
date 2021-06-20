using Engine.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Tests.Helpers
{
	internal static class SumHelper
	{
		internal static  int GetCompaniesSum(IEnumerable<Company> companies, string sumColumn)
		{
			var sum = 0;

			if (sumColumn.Equals("numberofemployees", StringComparison.OrdinalIgnoreCase))
			{
				sum = companies.Select(c => c.NumberOfEmployees).Sum();
			}
			else if (sumColumn.Equals("id", StringComparison.OrdinalIgnoreCase))
			{
				sum = companies.Select(c => c.Id).Sum();
			}
			else
			{
				sum = companies.Select(c => c.YearFounded).Sum();
			}
			return sum;
		}

		internal static int GetUsersSum(IEnumerable<User> users, string sumColumn)
		{
			var sum = 0;

			if (sumColumn.Equals("age", StringComparison.OrdinalIgnoreCase))
			{
				sum = users.Select(c => c.Age).Sum();
			}
			else if (sumColumn.Equals("id", StringComparison.OrdinalIgnoreCase))
			{
				sum = users.Select(c => c.Id).Sum();
			}
			return sum;
		}

		internal static int GetLogsSum(IEnumerable<Log> logs, string sumColumn)
		{
			var sum = 0;

			if (sumColumn.Equals("id", StringComparison.OrdinalIgnoreCase))
			{
				sum = logs.Select(c => c.Id).Sum();
			}
			else if (sumColumn.Equals("sum", StringComparison.OrdinalIgnoreCase) )
			{
				sum = logs.Select(c => c.Sum).Sum();
			}
			return sum;
		}

	}
}
