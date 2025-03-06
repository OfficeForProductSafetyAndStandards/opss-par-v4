using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;

namespace Opss.PrimaryAuthorityRegister.Web.UnitTests.Extensions;

public static class RenderedComponentExtensions
{
    public static IElement FindByDataTestId<TComponent>(this IRenderedComponent<TComponent> renderedComponent, string dataTestId)
        where TComponent : IComponent
        => renderedComponent.Find($"[data-testid={dataTestId}]");
}
