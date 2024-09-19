using Newtonsoft.Json;
using nicecyprj.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;

namespace nicecyprj.Controllers
{
	public class SearchHotelsController : RenderMvcController
	{
		public ActionResult Index(SearchHotels model, string diem_den)
		{
			// Receive home page Id from umbraco console
			const int homePageId = 1095;
			var homeContent = UmbracoContext.Content.GetById(homePageId);
			if (homeContent == null)
			{
				return CurrentTemplate(model);
			}
			var ksModel = new SearchHotelsModel(model);
			
			var cityId = 0;

			// Get list of available cities
			var listOfCity = ((Umbraco.Core.Models.PublishedContent.PublishedContentWrapped)homeContent).ChildrenOfType("City").ToList();
			ksModel.Cities = listOfCity.Select(c => new CityItem() 
			{
				Id = c.Id,
				Name = c.Name,
			}).ToList();

			if (int.TryParse(diem_den, out var cityIdParsed))
			{
				var city = ksModel.Cities.Where(c => c.Id == cityIdParsed).FirstOrDefault();
				if (city != null)
				{
					cityId = city.Id;
				}
			}

			if (cityId != 0)
			{
				listOfCity = listOfCity.Where(x => x.Id == cityId).ToList();
			}

			var ksItems = new List<KSItem>();
			foreach (var city in listOfCity)
			{
				// Get list of available districts in particular city
				var listOfDistrict = city.ChildrenOfType("District").ToList();
				foreach (var district in listOfDistrict)
				{
					// Get list of available hotel in particular district
					var listOfHotel = district.ChildrenOfType("Hotel").ToList();


					foreach (var hotel in listOfHotel)
					{
						// Get values in hotel
						var htlIsVisible = hotel.Value<bool>("IsVisible");
						if (!htlIsVisible)
						{
							continue;
						}
						var htlName = hotel.Value<string>("HsdTitle");
						var htlUrl = ((Umbraco.Core.Models.PublishedContent.PublishedContentWrapped)hotel).Url();
						var htlDescription = hotel.Value<string>("HsdDescription");
						var htlPictures = ((Umbraco.Web.PublishedModels.Hotel)hotel).HsdPicture;
						var htlPresentPicture = htlPictures.FirstOrDefault().Url();
						var mapPhoto = ((Umbraco.Web.PublishedModels.Hotel)hotel).MapPhoto;
						var mapPhotoUrl = mapPhoto.Url();
						var htlAddress = hotel.Value<string>("MapDescription");

						Hotel ht = hotel as Umbraco.Web.PublishedModels.Hotel;
						var features = ht.Features.Select (ft => new FeaturesModel() { FeatureName = ft.FeatureName, Icon = ft.Icon.Url() }).ToList();
					
						List<string> ImgUrls = new List<string>();
						foreach (var picture in htlPictures)
						{
							ImgUrls.Add(picture.Url());
						}
						ksItems.Add(new KSItem()
						{
							HotelDescription = htlDescription,
							HotelName = htlName,
							Url = htlUrl,
							ImgUrl = htlPresentPicture,
							Address = htlAddress,
							Features = features,
							MapPhotoUrl = mapPhotoUrl,
							ImgUrls = ImgUrls
						});
					}
				}
			}

			ksModel.Data = ksItems;

			return CurrentTemplate(ksModel);
		}
	}
}