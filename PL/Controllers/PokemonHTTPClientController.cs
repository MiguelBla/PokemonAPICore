using Microsoft.AspNetCore.Mvc;

namespace PL.Controllers
{
	public class PokemonHTTPClientController : Controller
	{
		public IActionResult GetAll()
		{
			return View();
		}

		//consumo httpclient
		public JsonResult GetPokemon(int? pag)
		{
			if (pag == null)
			{
				pag = 0;
			}

			ML.Result resultApi = new ML.Result();
			resultApi.Objects = new List<object>();

			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");

				var resposeTask = client.GetAsync("pokemon?limit=20&offset=" + pag);


				resposeTask.Wait();

				var result = resposeTask.Result;
				var json = result.Content.ReadAsStringAsync();

				if (result.IsSuccessStatusCode)
				{
					var readTask = result.Content.ReadAsAsync<ML.PokeAPI>();
					readTask.Wait();
					var resultPokeApi = readTask.Result;

					if (resultPokeApi.Results.Count > 0)
					{
						foreach (ML.Pokemo pokemonApi in resultPokeApi.Results)
						{
							ML.Pokemo pokemon = new ML.Pokemo();

							pokemon.Name = pokemonApi.Name;
							string[] numpokemon = pokemonApi.Url.Split('/');
							pokemon.Url = numpokemon[numpokemon.Length - 2];

							resultApi.Objects.Add(pokemon);
						}
						resultApi.Correct = true;

					}
				}
				else
				{
					resultApi.Correct = false;
					resultApi.ErrorMessage = "No se encontraron registros";
				}


			}
			return Json(resultApi);
		}
	}
}
