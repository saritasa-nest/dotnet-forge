namespace Saritasa.NetForge.Blazor.MVVM.ViewModels;

/// <summary>
/// Validation entity view model.
/// </summary>
public abstract class ValidationEntityViewModel : BaseViewModel
{
    /// <summary>
    /// General error message.
    /// </summary>
    public string GeneralError { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of field error models in the view model.
    /// </summary>
    public List<FieldErrorModel> FieldErrorModels { get; set; } = [];
}
