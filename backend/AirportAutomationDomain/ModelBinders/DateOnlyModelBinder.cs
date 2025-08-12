using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AirportAutomation.Core.ModelBinders
{
	public class DateOnlyModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
			if (string.IsNullOrEmpty(value))
			{
				bindingContext.Result = ModelBindingResult.Success(null);
				return Task.CompletedTask;
			}

			if (DateOnly.TryParse(value, out var date))
			{
				bindingContext.Result = ModelBindingResult.Success(date);
			}
			else
			{
				bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid date format.");
			}

			return Task.CompletedTask;
		}
	}
}
