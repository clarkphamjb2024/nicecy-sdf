using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PublishedModels;

namespace nicecyprj.Models
{
	public class SearchHotelsModel : SearchHotels
	{
		public SearchHotelsModel(IPublishedContent content) : base(content)
		{
			Data = new List<KSItem>();
			Cities = new List<CityItem>();
			Images = new List<string>();
		}
		public List<KSItem> Data { get; set; }
		public List<CityItem> Cities { get; set; }
		public List<string> Images { get; set; }
	}

	public class KSItem
	{
		public string HotelName { get; set; }
		public string HotelDescription { get; set; }
		public string ImgUrl { get; set; }
		public string Url { get; set; }
		public string Address { get; set; }
		public List<FeaturesModel> Features { get; set; }
		public string MapPhotoUrl { get; set; }
		public List<string> ImgUrls { get; set; }
		public string HTLID { get; set; }
		public int DistrictID { get; set; }
		public string DistrictName { get; set; }
	}

	public class CityItem
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
