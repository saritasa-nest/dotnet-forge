using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Saritasa.NetForge.Demo.Admin;

/// <summary>
/// Footer for admin panel.
/// </summary>
public partial class AdminFooter : ComponentBase, IAsyncDisposable
{
    private IJSObjectReference? module;

    [Inject]
    private IJSRuntime Js { get; set; } = null!;

    /// <summary>
    /// Visitors count.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public int VisitorsCount { get; set; }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module = await Js.InvokeAsync<IJSObjectReference>("import", "./Admin/AdminFooter.razor.js");
            await module.InvokeVoidAsync("incrementVisitsCount");
        }

    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (module is not null)
        {
            try
            {
                await module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Required in Blazor Server but can be removed in WebAssembly.
            }
        }
    }
}