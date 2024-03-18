//88002509639 - Yandex phone number
//7736207543 - Yandex INN number
//0445a64bf7ac9b2b9031e3fe8d62b7d503b8dcb6 - API key Dadata
//371524df1b754a7d9fd0466847e6ec7a3c8d0c7b - secret key Dadata

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

class People{
	public string PhoneNumber { get; set; }
	public string Operator { get; set; }
	public string FullName { get; set; }
	public string INN { get; set; }
	public string Country { get; set; }
	public string Gender { get; set; }


	public override string ToString()
	{
		return $"{FullName}:\n Phone  -=-  {PhoneNumber}\n Operator  -=-  {Operator}\n INN  -=-  {INN}\n Country  -=-  {Country}\n Gender  -=-  {Gender}";
	}
}
class People_list
{
	public List<People> People { get; set; }

	public void append(People people)
	{
		this.People.Add(people);
	}


	public People_list()
	{
		People = new List<People>();
	}

	public void displayAll()
	{
		foreach (var item in People)
		{
			Console.WriteLine(item);
			Console.WriteLine();
		}
	}

	static public async Task write_to_file(List<People> peopleList)
	{
		string path = "people_list.txt";

		using (StreamWriter writer = new StreamWriter(path, true))
		{
			await writer.WriteLineAsync("List of People:");
			foreach (var person in peopleList)
			{
				await writer.WriteLineAsync($"Name: {person.FullName}");
				await writer.WriteLineAsync($"Phone Number: {person.PhoneNumber}");
				await writer.WriteLineAsync($"Operator: {person.Operator}");
				await writer.WriteLineAsync($"INN: {person.INN}");
				await writer.WriteLineAsync($"Country: {person.Country}");
				await writer.WriteLineAsync($"Gender: {person.Gender}");
				await writer.WriteLineAsync();
			}
		}
	}

}
class Request
{
	public string Query { get; set; }
}

class PrettyName
{
	public string Result { get; set; }
	public string Gender { get; set; }
}

class NumberInfo
{
	public string Phone { get; set; }
	public string Provider { get; set; }
	public string Country { get; set; }
}

class InnInfo
{
	public List<Suggestions> Suggestions { get; set; }
}

class Suggestions
{
	public Data Data { get; set; }
}

class Data
{
	public Management Management { get; set; }
}

class Management
{
	public string Name { get; set; }
}

class Program{
	static HttpClient httpClient = new HttpClient();
	
	static async Task Main(string[] args)
	{
		Encoding utf8 = new UTF8Encoding(true);

		httpClient.DefaultRequestHeaders.Add("Authorization", "Token 0445a64bf7ac9b2b9031e3fe8d62b7d503b8dcb6");
		httpClient.DefaultRequestHeaders.Add("X-Secret", "371524df1b754a7d9fd0466847e6ec7a3c8d0c7b");
		People_list peopleList = new People_list();
		string GH="1";

		while (true)
		{
			if (GH == "1")
			{
				Console.WriteLine("Write Phone Number: ");
				string Phone_n = Console.ReadLine();
				People personToAdd = new People();

				string[] number = { Phone_n };
				var numberResponce = await httpClient.PostAsJsonAsync("https://cleaner.dadata.ru/api/v1/clean/phone", number);
				var numberResult = await numberResponce.Content.ReadFromJsonAsync<List<NumberInfo>>();


				foreach (var item in numberResult)
				{
					personToAdd.PhoneNumber = item.Phone;
					personToAdd.Country = item.Country;
					personToAdd.Operator = item.Provider;
				}


				Console.WriteLine("Введите инн");
				string inn = Console.ReadLine();
				var response = await httpClient.PostAsJsonAsync("http://suggestions.dadata.ru/suggestions/api/4_1/rs/findById/party", new Request { Query = inn });

				personToAdd.INN = inn;
				var result = await response.Content.ReadFromJsonAsync<InnInfo>();

				string[] names = { result.Suggestions[0].Data.Management.Name };
				var response2 = await httpClient.PostAsJsonAsync("https://cleaner.dadata.ru/api/v1/clean/name", names);
				var resul2t = await response2.Content.ReadFromJsonAsync<List<PrettyName>>();

				foreach (var item in resul2t)
				{
					personToAdd.FullName = item.Result;
					personToAdd.Gender = item.Gender;
				}

				peopleList.append(personToAdd);
				await People_list.write_to_file(peopleList.People);
				peopleList.displayAll();
				Console.WriteLine("Continue or stop (1 or 2): ");
				GH = Console.ReadLine();
			}
			else
			{
				return;
			}
		}
		Console.ReadLine();
	}
}