using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace PeliculasAPI.Utilities
{
    public class TypeBinder: IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string propertyName = bindingContext.ModelName;
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(propertyName);

            if (value != ValueProviderResult.None)
            {
                try
                {
                    Type destinationType = bindingContext.ModelMetadata.ModelType;
                    Object? deserializedValue = JsonSerializer.Deserialize(value.FirstValue!,
                        destinationType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    bindingContext.Result = ModelBindingResult.Success(deserializedValue);
                }
                catch
                {
                    bindingContext.ModelState.AddModelError(propertyName, "El valor dado no es del tipo adecuado");
                }
            }

            return Task.CompletedTask;
        }
    }
}
