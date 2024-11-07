using nicecyprj.Models;
using nicecyprj.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;
using static nicecyprj.Utils.HTLChannelManager;

namespace nicecyprj.Controllers
{
	public class SearchHotelsController : RenderMvcController
	{
		/// <summary>
		/// ap: Destinations
		/// dt: Datetime
		/// ps: Touris
		/// sc: Service HTL/Nicecy
		/// ?ap=SGN.HAN&dt=27-10-2024.29-10-2024&ps=1.0.0&sc=ECONOMY
		/// </summary>
		/// <param name="model"></param>
		/// <param name="service"></param>
		/// <param name="ap"></param>
		/// <param name="dt"></param>
		/// <param name="ps"></param>
		/// <param name="sc"></param>
		/// <returns></returns>
		public async Task<ActionResult> Index(SearchHotels model, string ap = null, string dt = null, int ps	= 0, string sc = "")
		{
			// Receive home page Id from umbraco console
			const int homePageId = 1095;
			var home = UmbracoContext.Content.GetById(homePageId);
			if (home == null)
			{
				return CurrentTemplate(model);
			}

			// Get list of available cities
			var cities = ((Umbraco.Core.Models.PublishedContent.PublishedContentWrapped)home).ChildrenOfType("City").ToList();

			// Get all available hotels
			var ksHotels = new List<KSItem>();
			var random = new Random();
			foreach (var item in cities)
			{
				var city = (City)item;
				if (city.FeaturedDistricts != null && city.FeaturedDistricts.Any())
				{
					foreach (var district in city.FeaturedDistricts)
					{
						var districtEntry = (District)district;
						if (districtEntry != null && districtEntry.FeaturedHotels != null && districtEntry.FeaturedHotels.Any()) 
						{
							var hotels = districtEntry.FeaturedHotels.ToList();
							foreach (var hotel in hotels)
							{
								var hotelEntry = (Hotel)hotel;
								var features = hotelEntry.Features.ToList().Select(f => new FeaturesModel() { FeatureName = f.FeatureName, Icon = f.Icon.Url() });

								ksHotels.Add(new KSItem() 
								{
									HotelName = hotelEntry.Name,
									ImgUrl = hotelEntry.Photos.ToList()[random.Next(3)].Url(),
									ImgUrls = hotelEntry.Photos.ToList().Select(p =>p.Url()).ToList(),
									HotelDescription = hotelEntry.HotelShortDescription,
									Address = hotelEntry.MapDescription,
									HTLID = hotelEntry.HotelId_Htl,
									Features = features.ToList(),
									Url = hotelEntry.Url(),
									DistrictID = districtEntry.DistrictId,
									DistrictName = districtEntry.Name
								});
							}
						}
					}
				}
			}

			var ksModel = new SearchHotelsModel(model);
			ksModel.Data = ksHotels;

			var SearchHotelEntry = (SearchHotels)model;
			ksModel.Images = SearchHotelEntry.SearchHotelImages.ToList().Select(img => img.Url()).ToList();

			const string HTLChannel = "HTLCHANNEL";
			if (sc != HTLChannel) 
			{
				return CurrentTemplate(ksModel);
			}

			if (sc == HTLChannel && !string.IsNullOrEmpty(dt) && !string.IsNullOrEmpty(ap) && ps > 0)
			{
				try
				{
					var districtIds = ap.Split('.').ToList();
					var hotelHTLIds = ksHotels.Where(hotel => districtIds.Contains(hotel.DistrictID.ToString())).ToList().Select(x => x.HTLID);
					var htlChannelManager = new HTLChannelManager();

					var htlClientAPI = await htlChannelManager.GetAuthenticatedHttpClientForAPIRequest();

					if (htlClientAPI != null)
					{
						var request = new HotelAvailabilityRequest()
						{
							ap = ap,
							dt = dt,
							ps = ps,
							hts = hotelHTLIds.ToList()
						};

						var availableHotels = await htlChannelManager.GetHotelAvailability(htlClientAPI, request);

						if (availableHotels != null && availableHotels.result && availableHotels.data != null && availableHotels.data.hotels.Any())
						{
							var htlHotelIds = availableHotels.data.hotels.Select(g => g.hotel_id).ToList();
							ksModel.Data = ksModel.Data.Where(hotel => htlHotelIds.Contains(hotel.HTLID) && districtIds.Contains(hotel.DistrictID.ToString())).ToList();

							var start_date = request.dt.Split('-')[0];
							var end_date = request.dt.Split('-')[1];

							foreach (var jHotel in ksModel.Data)
							{
								jHotel.Url = jHotel.Url + $"?id={jHotel.HTLID}&check_in={DateTime.Parse(start_date).ToString("dd MMM yyyy")}&check_out={DateTime.Parse(end_date).ToString("dd MMM yyyy")}&filter_adult={ps}&lang=vi";
							}
							return CurrentTemplate(ksModel);
						}

						ksModel.Data = new List<KSItem>();
						return CurrentTemplate(ksModel);
					}
				}
				catch (Exception)
				{
					ksModel.Data = new List<KSItem>();
					return CurrentTemplate(ksModel);
				}

			}
			return CurrentTemplate(ksModel);
		}
	}
}