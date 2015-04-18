using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.PhotoSelectionViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class PhotoSelectionViewModel : BaseViewModel
	{
		public League League
		{
			get;
			set;
		}

		public List<string> Photos
		{
			get;
			set;
		}

		async public Task GetPhotos(string keyword)
		{
			Photos = await FlikrService.Instance.SearchPhotos(keyword);
			SetPropertyChanged("Photos");
		}
	}
}