using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace nicecyprj.Utils
{
	public class HTLChannelManager
	{
		public HTLChannelManager()
		{

		}

		public async Task<HttpClient> GetAuthenticatedHttpClientForAPIRequest()
		{
			var httpClient = new HttpClient()
			{
				BaseAddress = new Uri(ConfigurationManager.AppSettings["HTLChanel_URL"])
			};
			httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

			using (var tokenAccessClient = new HttpClient())
			{
				tokenAccessClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", ConfigurationManager.AppSettings["HTLChanel_GetTokenApi_Cred"]);

				HttpResponseMessage response = await tokenAccessClient.PostAsync(ConfigurationManager.AppSettings["HTLChanel_URL"] + "token", null);

				var result = await response.Content.ReadAsAsync<TokenResponse>()
					.ConfigureAwait(false);

				if (result == null || (result != null && !result.result))
				{
					return null;
				}

				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.data.access_token);
			}
			return httpClient;
		}

		// sample request :: https://localhost:44366/danh-sach-khach-san?ap=1&dt=11/01/2024.11/09/2024&ps=2&sc=HTLCHANNEL

		public async Task<HotelAvailabilityHTLResponse> GetHotelAvailability(HttpClient client, HotelAvailabilityRequest request)
		{
			var start_date = request.dt.Split('-')[0];
			var end_date = request.dt.Split('-')[1];

			var htlRequest = new HotelAvailabilityHTLRequest()
			{
				start_date = DateTime.Parse(start_date).ToString("yyyy-MM-dd"),
				end_date = DateTime.Parse(end_date).ToString("yyyy-MM-dd"),
				travelers = new List<Travelers>()
				{
					new Travelers
					{
						 adults = request.ps,
						 children = 0
					}
				},
				currency = "VND",
				hotels = request.hts.Select(id => new Hotels() { hotel_id = id, partner_code = "" }).ToList()
			};

			var content = new StringContent(JsonConvert.SerializeObject(htlRequest), Encoding.UTF8, "application/json");

			try
			{
				var response = await client.PostAsync(ConfigurationManager.AppSettings["HTLChanel_URL"] + "hotelAvailability", content);

				if (response.IsSuccessStatusCode)
				{
					var responseData = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<HotelAvailabilityHTLResponse>(responseData);
				}
				else
				{
					var errorResponse = await response.Content.ReadAsStringAsync();
					throw new Exception($"Error: {response.StatusCode}, Details: {errorResponse}");
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to get hotel availability", ex);
			}
		}

		public class TokenResponse
		{
			public bool result { get; set; }
			public TokenData data { get; set; }
		}

		public class TokenData
		{
			public string access_token { get; set; }
		}


		public class HotelAvailabilityRequest
		{
			public string ap { get; set; }
			public string dt { get; set; }
			public int ps { get; set; }
			public List<string> hts { get; set; }

		}

		public class HotelAvailabilityHTLRequest
		{
			public string start_date { get; set; }
			public string end_date { get; set; }
			public string currency { get; set; }

			public List<Travelers> travelers { get; set; }
			public List<Hotels> hotels { get; set; }

		}

		public class HotelAvailabilityHTLResponse
		{
			public bool result { get; set; }
			public HotelAvailabilityHTLData data { get; set; }
		}

		public class HotelAvailabilityHTLData
		{
			public List<Hotels> hotels { get; set; }
		}

		public class Travelers
		{
			public int adults { get; set; }
			public int children { get; set; }
		}

		public class Hotels
		{
			public string hotel_id { get; set; }
			public string partner_code { get; set; }
		}
	}
}