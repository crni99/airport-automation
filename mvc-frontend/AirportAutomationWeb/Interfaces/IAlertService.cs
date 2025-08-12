using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AirportAutomation.Web.Interfaces
{
	public interface IAlertService
	{
		void SetAlertMessage(ITempDataDictionary tempData, string message, bool isSuccess);
	}
}
