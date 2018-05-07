using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
using System.Text;
using System.Web.Routing;
using System.Data;
using System.IO;

namespace EmployeeManagement.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString CustomCheckbox(this HtmlHelper htmlHelper, string ColumnName, string Value, object htmlAttributes = null)
        {
            //Creating input control using TagBuilder class.
            TagBuilder checkbox = new TagBuilder("input");

            //Setting its type attribute to checkbox to render checkbox control.
            checkbox.Attributes.Add("type", "checkbox");

            //Setting the name and id attribute.
            checkbox.Attributes.Add("name", ColumnName);
            checkbox.Attributes.Add("id", ColumnName);

            checkbox.Attributes.Add("data-pastvalue", Value);//Value is Past Value           


            //Adding the checked attibute based on parameter received.
            if (Value != null && Value.Trim().ToLower() == "true")
                checkbox.Attributes.Add("checked", "checked");

            //Merging the other attributes passed in the attribute.
            checkbox.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            return MvcHtmlString.Create(checkbox.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString Sorter(this HtmlHelper htmlHelper, string ColumnName, string displayText, int resultCount, string actionName, string controllerName = "", AjaxOptions ajaxOptions = null, object routeValues = null, object htmlAttributes = null, bool isAddClickAttr = false)
        {
            if (resultCount > 0)
            {
                string sortOrder = "";
                var builder = new TagBuilder("a");
                var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
                string url = actionName;
                if (routeValues != null)
                {
                    var rValues = new System.Web.Routing.RouteValueDictionary(routeValues);
                    rValues.Add("fieldName", ColumnName);
                    if (string.IsNullOrEmpty(rValues["sortOrder"].ToString()))
                        rValues["sortOrder"] = "desc";
                    else
                        sortOrder = rValues["sortOrder"].ToString();
                    rValues["sortOrder"] = rValues["sortOrder"].ToString() == "asc" ? "desc" : "asc";

                    if (controllerName == "")
                    {
                        url = urlHelper.Action(actionName, rValues);
                    }
                    else
                    {
                        url = urlHelper.Action(actionName, controllerName, rValues);
                    }

                    if (isAddClickAttr)
                    {
                        builder.Attributes.Add("onclick", "SortingChange(this,'" + url + "','" + ColumnName + "','" + rValues["sortOrder"] + "')");
                    }
                    else
                    {
                        builder.MergeAttribute("href", url);
                        builder.MergeAttribute("href", "#");
                    }
                }

                if (htmlAttributes != null)
                    builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
                if (ajaxOptions != null)
                    builder.MergeAttributes(ajaxOptions.ToUnobtrusiveHtmlAttributes());

                builder.InnerHtml = displayText;
                if (!string.IsNullOrEmpty(sortOrder))
                {
                    var innerTag = new TagBuilder("i");
                    innerTag.MergeAttribute("class", sortOrder == "asc" ? "glyphicon glyphicon-arrow-up icon-sml" : "glyphicon glyphicon-arrow-down icon-sml");
                    builder.InnerHtml = displayText + " " + innerTag.ToString(TagRenderMode.Normal);
                }

                return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
            }
            else
            {
                return MvcHtmlString.Create(displayText);
            }
        }

        public static MvcHtmlString PagerDropdown(this HtmlHelper htmlHelper, int pagesize, string actionName, string controllerName = "", AjaxOptions ajaxOptions = null, object routeValues = null, object htmlAttributes = null, int staticPageSize = 0)
        {
            var builder = new TagBuilder("select");
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            string value = pagesize.ToString();

            string url = actionName;
            if (routeValues != null)
            {
                var rValues = new System.Web.Routing.RouteValueDictionary(routeValues);
                if (controllerName == "")
                    url = urlHelper.Action(actionName, rValues);
                else
                    url = urlHelper.Action(actionName, controllerName, rValues);
            }
            else
            {
                if (controllerName != "")
                    url = urlHelper.Action(actionName, controllerName);
            }
            if (htmlAttributes != null)
                builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            if (ajaxOptions != null)
                builder.MergeAttributes(ajaxOptions.ToUnobtrusiveHtmlAttributes());
            StringBuilder options = new StringBuilder();


            options = options.Append("<option value='10' " + ((!string.IsNullOrEmpty(value) && value == "10") ? "selected=selected" : "") + ">10</option>");
            options = options.Append("<option value='20' " + ((!string.IsNullOrEmpty(value) && value == "20") ? "selected=selected" : "") + ">20</option>");
            options = options.Append("<option value='30' " + ((!string.IsNullOrEmpty(value) && value == "30") ? "selected=selected" : "") + ">30</option>");
            options = options.Append("<option value='50' " + ((!string.IsNullOrEmpty(value) && value == "50") ? "selected=selected" : "") + ">50</option>");
            options = options.Append("<option value='100' " + ((!string.IsNullOrEmpty(value) && value == "100") ? "selected=selected" : "") + ">100</option>");
            builder.InnerHtml = options.ToString();

            builder.Attributes["onchange"] = "pagerDropdownChange(this,'" + url + "' , " + value + ")";//pass old page value on drop down change
            builder.Attributes["class"] = "pagerDropdown form-control input-sm input-xsmall input-inline";
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString PagerDropdownForThumbnail(this HtmlHelper htmlHelper, int pagesize, string actionName, string controllerName = "", AjaxOptions ajaxOptions = null, object routeValues = null, object htmlAttributes = null, int staticPageSize = 0)
        {
            var builder = new TagBuilder("select");
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            string value = pagesize.ToString();

            string url = actionName;
            if (routeValues != null)
            {
                var rValues = new System.Web.Routing.RouteValueDictionary(routeValues);
                if (controllerName == "")
                    url = urlHelper.Action(actionName, rValues);
                else
                    url = urlHelper.Action(actionName, controllerName, rValues);
            }
            else
            {
                if (controllerName != "")
                    url = urlHelper.Action(actionName, controllerName);
            }
            if (htmlAttributes != null)
                builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            if (ajaxOptions != null)
                builder.MergeAttributes(ajaxOptions.ToUnobtrusiveHtmlAttributes());
            StringBuilder options = new StringBuilder();


            options = options.Append("<option value='10' " + ((!string.IsNullOrEmpty(value) && value == "10") ? "selected=selected" : "") + ">10</option>");
            options = options.Append("<option value='20' " + ((!string.IsNullOrEmpty(value) && value == "20") ? "selected=selected" : "") + ">20</option>");
            options = options.Append("<option value='30' " + ((!string.IsNullOrEmpty(value) && value == "30") ? "selected=selected" : "") + ">30</option>");
            options = options.Append("<option value='50' " + ((!string.IsNullOrEmpty(value) && value == "50") ? "selected=selected" : "") + ">50</option>");
            options = options.Append("<option value='100' " + ((!string.IsNullOrEmpty(value) && value == "100") ? "selected=selected" : "") + ">100</option>");
            builder.InnerHtml = options.ToString();

            builder.Attributes["onchange"] = "pagerThumbViewDropdownChange(this,'" + url + "' , " + value + ")";//pass old page value on drop down change
            builder.Attributes["class"] = "pagerDropdown form-control input-sm input-xsmall input-inline";
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString PagerDropdownForInfiniteScroll(this HtmlHelper htmlHelper, int pagesize, string actionName, string controllerName = "")
        {
            var builder = new TagBuilder("select");
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            string value = pagesize.ToString();

            string url = actionName;

            if (controllerName != "")
                url = urlHelper.Action(actionName, controllerName);
            StringBuilder options = new StringBuilder();
            options = options.Append("<option value='12' " + ((!string.IsNullOrEmpty(value) && value == "12") ? "selected=selected" : "") + ">12</option>");
            options = options.Append("<option value='24' " + ((!string.IsNullOrEmpty(value) && value == "24") ? "selected=selected" : "") + ">24</option>");
            options = options.Append("<option value='36' " + ((!string.IsNullOrEmpty(value) && value == "36") ? "selected=selected" : "") + ">36</option>");
            options = options.Append("<option value='50' " + ((!string.IsNullOrEmpty(value) && value == "50") ? "selected=selected" : "") + ">50</option>");
            options = options.Append("<option value='100' " + ((!string.IsNullOrEmpty(value) && value == "100") ? "selected=selected" : "") + ">100</option>");
            builder.InnerHtml = options.ToString();

            builder.Attributes["onchange"] = "pagerInfiniteScrollDropdownChange(this)";

            builder.Attributes["class"] = "pagerDropdown form-control input-sm input-xsmall input-inline";
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }
    }
}


//For Common PagedListRenderOptions of PagedList.MVC Paging
public class MyCustomRenderOptions : PagedList.Mvc.PagedListRenderOptions
{
    public MyCustomRenderOptions()
        : base()
    {
        DisplayLinkToFirstPage = PagedList.Mvc.PagedListDisplayMode.Always;
        DisplayLinkToLastPage = PagedList.Mvc.PagedListDisplayMode.Always;
        DisplayLinkToPreviousPage = PagedList.Mvc.PagedListDisplayMode.Always;
        DisplayLinkToNextPage = PagedList.Mvc.PagedListDisplayMode.Always;
        MaximumPageNumbersToDisplay = 5;
        Display = PagedList.Mvc.PagedListDisplayMode.IfNeeded;
    }
}
public class MyCustomNextPrevNumberRenderOptions : PagedList.Mvc.PagedListRenderOptions
{
    public MyCustomNextPrevNumberRenderOptions()
        : base()
    {
        DisplayLinkToFirstPage = PagedList.Mvc.PagedListDisplayMode.Never;
        DisplayLinkToLastPage = PagedList.Mvc.PagedListDisplayMode.Never;
        DisplayLinkToPreviousPage = PagedList.Mvc.PagedListDisplayMode.Always;
        DisplayLinkToNextPage = PagedList.Mvc.PagedListDisplayMode.Always;
        MaximumPageNumbersToDisplay = 5;
        DisplayEllipsesWhenNotShowingAllPageNumbers = false;
        Display = PagedList.Mvc.PagedListDisplayMode.IfNeeded;
    }
}
public class MyCustomFirstLastNumberRenderOptions : PagedList.Mvc.PagedListRenderOptions
{
    public MyCustomFirstLastNumberRenderOptions()
        : base()
    {
        DisplayLinkToFirstPage = PagedList.Mvc.PagedListDisplayMode.Always;
        DisplayLinkToLastPage = PagedList.Mvc.PagedListDisplayMode.Always;
        DisplayLinkToPreviousPage = PagedList.Mvc.PagedListDisplayMode.Never;
        DisplayLinkToNextPage = PagedList.Mvc.PagedListDisplayMode.Never;
        MaximumPageNumbersToDisplay = 5;
        DisplayEllipsesWhenNotShowingAllPageNumbers = false;
        Display = PagedList.Mvc.PagedListDisplayMode.IfNeeded;
    }
}
public class MyThumbViewRenderOptions : PagedList.Mvc.PagedListRenderOptions
{
    public MyThumbViewRenderOptions()
        : base()
    {
        DisplayLinkToFirstPage = PagedList.Mvc.PagedListDisplayMode.Never;
        DisplayLinkToLastPage = PagedList.Mvc.PagedListDisplayMode.Never;
        DisplayLinkToPreviousPage = PagedList.Mvc.PagedListDisplayMode.Always;
        DisplayLinkToNextPage = PagedList.Mvc.PagedListDisplayMode.Always;
        MaximumPageNumbersToDisplay = 0;
        Display = PagedList.Mvc.PagedListDisplayMode.IfNeeded;
        DisplayLinkToIndividualPages = false;

        LinkToPreviousPageFormat = "<i class='glyphicon glyphicon-circle-arrow-left'></i>";
        LinkToNextPageFormat = "<i class='glyphicon glyphicon-circle-arrow-right'></i>";

    }
}

