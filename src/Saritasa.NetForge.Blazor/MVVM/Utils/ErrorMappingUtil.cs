using System.ComponentModel.DataAnnotations;
using Saritasa.NetForge.Blazor.MVVM.ViewModels;

namespace Saritasa.NetForge.Blazor.MVVM.Utils;

/// <summary>
/// Error utility class for mapping validation errors.
/// </summary>
public static class ErrorMappingUtil
{
    /// <summary>
    /// Maps validation errors to the corresponding fields in the provided list of models.
    /// </summary>
    /// <param name="models">The list of <see cref="FieldErrorModel"/> objects where the errors need to be mapped.</param>
    /// <param name="errors">The list of <see cref="ValidationResult"/> containing validation errors.</param>
    public static void MappingErrorToCorrectField(this List<FieldErrorModel> models, List<ValidationResult> errors)
    {
        // Clear the error on the previous validation.
        models.ForEach(e => e.ErrorMessage = string.Empty);

        foreach (var result in errors)
        {
            foreach (var member in result.MemberNames)
            {
                var errorViewModel = models.FirstOrDefault(e => e.Property.Name == member);
                if (errorViewModel is null)
                {
                    continue;
                }

                errorViewModel.ErrorMessage = result.ErrorMessage!;
            }
        }
    }
}
