using System.IO;

namespace Engine.Providers
{
	public class ResourcesProvider
	{
		private static string[] _name;
		private static string[] _surnames;
		private static string[] _countries;

		private static ResourcesProvider _instance;

		public static ResourcesProvider Instance
		{
			get
			{
				if (_instance == null)
					_instance = new ResourcesProvider();
				return _instance;
			}
		}

		public string[] Names
		{
			get
			{
				if (_name != null)
				{
					return _name;
				}
				_name = GetItems("names");

				return _name;
			}
		}

		public string[] Surnames
		{
			get
			{
				if (_surnames != null)
				{
					return _surnames;
				}
				_surnames = GetItems("surnames");
				return _surnames;
			}
		}

		public string[] Countries 
		{
			get 
			{
				if (_countries != null)
				{
					return _countries;
				}
				_countries = GetItems("countries");
				return _countries;
			}
		}

		private static string[] GetItems(string filename)
		{
			try
			{
				var path = Directory.GetCurrentDirectory() + $"/Resources/{filename}.txt";
				var str = File.ReadAllText(path);
				return str.Replace("\r", "").Split('\n');
			}
			catch (FileNotFoundException ex) {
				 throw ex;
			}
		}
	}
}
