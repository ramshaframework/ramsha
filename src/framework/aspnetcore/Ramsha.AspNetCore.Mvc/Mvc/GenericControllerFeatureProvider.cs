using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Ramsha.AspNetCore.Mvc;


public class GenericPageRouteModelConvention : IPageRouteModelConvention
{
    private readonly TypeInfo[] _pageTypes;

    public GenericPageRouteModelConvention(params TypeInfo[] pageTypes)
    {
        _pageTypes = pageTypes;
    }

    public void Apply(PageRouteModel model)
    {
        foreach (var pageType in _pageTypes)
        {

            ConfigurePageRoute(model, pageType);

        }
    }

    private void ConfigurePageRoute(PageRouteModel model, TypeInfo pageType)
    {
        var area = GetArea(pageType);
        var pageName = GetPageName(pageType);

        model.Selectors.Clear();

        model.Selectors.Add(new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel
            {
                Template = $"{area}/{pageName}/{{action=Index}}/{{id?}}",
            }
        });

    }

    private string GetArea(TypeInfo pageType)
    {
        return pageType.GetCustomAttribute<AreaAttribute>()?.RouteValue ?? "Identity";
    }

    private string GetPageName(TypeInfo pageType)
    {
        return pageType.Name.Replace("Page", "").Replace("`", "").ToLower();
    }
}

public class GenericControllerFeatureProvider(TypeInfo[] controllerTypes) : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        foreach (var controllerType in controllerTypes)
        {
            feature.Controllers.Add(controllerType);
        }
    }
}
