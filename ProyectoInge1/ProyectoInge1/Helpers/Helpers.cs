using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ProyectoInge1.Helpers
{
    public static class Helpers
    {
        public static MvcHtmlString CustomLabelFor<TModel, TValue>(
    this HtmlHelper<TModel> html,
    Expression<Func<TModel, TValue>> modelProperty,
    object htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(modelProperty, html.ViewData);
            var labelText = metadata.IsRequired ? string.Format("* {0}", metadata.GetDisplayName()) : metadata.GetDisplayName();
            return html.LabelFor(modelProperty, labelText, htmlAttributes);
        }
    }


}