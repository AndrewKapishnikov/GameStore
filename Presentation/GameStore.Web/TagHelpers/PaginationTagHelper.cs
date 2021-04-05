using GameStore.Web.Models.AdminPanelModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;


namespace GameStore.Web.TagHelpers
{
    public class PaginationTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public PaginationTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        //We can get the presentation context through dependency injection through attributes
        [ViewContext]
        //To avoid binding to tag attributes
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string PageController { get; set; }
        public PaginationViewModel PageModel { get; set; }
        public string PageAction { get; set; }
  
        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";

            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");

            TagBuilder currentItem = CreateTag(PageModel.PageNumber, urlHelper, false, false);

            if (PageModel.HasHome)
            {
                TagBuilder home = CreateTag(PageModel.PageNumber - 3, urlHelper, true, false);
                tag.InnerHtml.AppendHtml(home);
            }

            if (PageModel.HasPreviousPreviousPage)
            {
                TagBuilder prevPrevItem = CreateTag(PageModel.PageNumber - 2, urlHelper, false, false);
                tag.InnerHtml.AppendHtml(prevPrevItem);
            }
            if (PageModel.HasPreviousPage)
            {
                TagBuilder prevItem = CreateTag(PageModel.PageNumber - 1, urlHelper, false, false);
                tag.InnerHtml.AppendHtml(prevItem);
            }

            tag.InnerHtml.AppendHtml(currentItem);
           
            if (PageModel.HasNextPage)
            {
                TagBuilder nextItem = CreateTag(PageModel.PageNumber + 1, urlHelper, false, false);
                tag.InnerHtml.AppendHtml(nextItem);
            }
            if (PageModel.HasNextNextPage)
            {
                TagBuilder nextNextItem = CreateTag(PageModel.PageNumber + 2, urlHelper, false, false);
                tag.InnerHtml.AppendHtml(nextNextItem);
            }
            if(PageModel.HasEnd)
            {
                TagBuilder end = CreateTag(PageModel.PageNumber + 3, urlHelper, false, true);
                tag.InnerHtml.AppendHtml(end);
            }
            output.Content.AppendHtml(tag);
        }

        TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper, bool home, bool end )
        {
            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("a");
            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            if (home)
            {
                PageUrlValues["page"] = pageNumber;
                link.Attributes["href"] = urlHelper.Action(PageAction, PageController, PageUrlValues);
                // link.InnerHtml.Append("<");
                link.AddCssClass("glyphicon-chevron-padding");
                TagBuilder i = new TagBuilder("i");
                i.AddCssClass("glyphicon");
                i.AddCssClass("glyphicon-chevron-left");
                link.InnerHtml.AppendHtml(i);
            }
            else if (end)
            {
                PageUrlValues["page"] = pageNumber;
                link.Attributes["href"] = urlHelper.Action(PageAction, PageController, PageUrlValues);
                // link.InnerHtml.Append(">");
                link.AddCssClass("glyphicon-chevron-padding");
                TagBuilder i = new TagBuilder("i");
                i.AddCssClass("glyphicon");
                i.AddCssClass("glyphicon-chevron-right");
                link.InnerHtml.AppendHtml(i);
            }
            else
            {
                if (pageNumber == this.PageModel.PageNumber)
                {
                    item.AddCssClass("active");
                    link.InnerHtml.Append(pageNumber.ToString());
                }
                else
                {
                    PageUrlValues["page"] = pageNumber;
                    link.Attributes["href"] = urlHelper.Action(PageAction, PageController, PageUrlValues);
                    link.InnerHtml.Append(pageNumber.ToString());
                }

            }
            item.InnerHtml.AppendHtml(link);
            return item;
        }
    }
}
