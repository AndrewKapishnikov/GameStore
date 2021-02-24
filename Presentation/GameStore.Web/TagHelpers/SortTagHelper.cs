using GameStore.Web.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;


namespace GameStore.Web.TagHelpers
{
    public class SortTagHelper : TagHelper
    {
        public SortGameStates CurrentSortGame { get; set; } 
        public SortOrderStates CurrentSortOrder { get; set; }
        public SortUserStates CurrentSortUser { get; set; }
        public string Action { get; set; }  
        public string Controller { get; set; }
        public bool Up { get; set; }   

        private IUrlHelperFactory urlHelperFactory;
        public SortTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "a";
            string url = urlHelper.Action(Action, Controller, PageUrlValues);
            output.Attributes.SetAttribute("href", url);

            if (Action == "games")
            {
                if (CurrentSortGame.ToString().ToLower() == PageUrlValues["sort"].ToString())
                {
                    CreateTagHelper();
                }
            }
            else if(Action == "orders")
            {
                if (CurrentSortOrder.ToString().ToLower() == PageUrlValues["sort"].ToString())
                {
                    CreateTagHelper();
                }
            }
            else
            {
                if (CurrentSortUser.ToString().ToLower() == PageUrlValues["sort"].ToString())
                {
                     CreateTagHelper();
                }
            }

            void CreateTagHelper()
            {
                TagBuilder tag = new TagBuilder("i");
                tag.AddCssClass("glyphicon");

                if (Up == true)
                    tag.AddCssClass("glyphicon-chevron-up");
                else
                    tag.AddCssClass("glyphicon-chevron-down");

                output.PreContent.AppendHtml(tag);
            }

        }






    }
}
