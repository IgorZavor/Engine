using Engine.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Tests.Helpers
{
	internal static class FilterHelper
	{
		internal static IEnumerable<Company> GetFilteredCompanies(List<Company> companies, string filterColumn, List<string> filterValues)
		{
			IEnumerable<Company> filteredCompanies = null;
			if (filterColumn.Equals("country", StringComparison.OrdinalIgnoreCase))
			{
				filteredCompanies = companies.Where(c => filterValues.Contains(c.Country));
			}
			else if (filterColumn.Equals("name", StringComparison.OrdinalIgnoreCase))
			{
				filteredCompanies = companies.Where(c => filterValues.Contains(c.Name));
			}
			else if (filterColumn.Equals("numberofemployees", StringComparison.OrdinalIgnoreCase))
			{
				filteredCompanies = companies.Where(c => filterValues.Contains(c.NumberOfEmployees.ToString()));
			}
			else if (filterColumn.Equals("yearfounded", StringComparison.OrdinalIgnoreCase))
			{
				filteredCompanies = companies.Where(c => filterValues.Contains(c.YearFounded.ToString()));
			}
			else 
			{
				filteredCompanies = companies.Where(c => filterValues.Contains(c.Id.ToString()));
			}
			return filteredCompanies;
		}

		internal static IEnumerable<User> GetFilteredUsers(List<User> users, string filterColumn, List<string> filterValues)
		{
			IEnumerable<User> filtered = null;
			if (filterColumn.Equals("country", StringComparison.OrdinalIgnoreCase))
			{
				filtered = users.Where(c => filterValues.Contains(c.Country));
			}
			else if (filterColumn.Equals("name", StringComparison.OrdinalIgnoreCase))
			{
				filtered = users.Where(c => filterValues.Contains(c.Name));
			}
			else if (filterColumn.Equals("surname", StringComparison.OrdinalIgnoreCase))
			{
				filtered = users.Where(c => filterValues.Contains(c.Surname));
			}
			else if (filterColumn.Equals("age", StringComparison.OrdinalIgnoreCase))
			{
				filtered = users.Where(c => filterValues.Contains(c.Age.ToString()));
			}
			else 
			{
				filtered = users.Where(c => filterValues.Contains(c.Id.ToString()));
			}
			return filtered;
		}

		internal static IEnumerable<Log> GetFilteredLogs(List<Log> logs, string filterColumn, List<string> filterValues)
		{
			IEnumerable<Log> filtered = null;
			if (filterColumn.Equals("author", StringComparison.OrdinalIgnoreCase))
			{
				filtered = logs.Where(c => filterValues.Contains(c.Author));
			}
			else if (filterColumn.Equals("dateTime", StringComparison.OrdinalIgnoreCase))
			{
				filtered = logs.Where(c => filterValues.Contains(c.DateTime));
			}
			else if (filterColumn.Equals("sum", StringComparison.OrdinalIgnoreCase))
			{
				filtered = logs.Where(c => filterValues.Contains(c.Sum.ToString()));
			}
			else if (filterColumn.Equals("filter", StringComparison.OrdinalIgnoreCase))
			{
				filtered = logs.Where(c => filterValues.Contains(c.Filter.ToString()));
			}
			else 
			{
				filtered = logs.Where(c => filterValues.Contains(c.Id.ToString()));
			}
			return filtered;
		}


	}
}
