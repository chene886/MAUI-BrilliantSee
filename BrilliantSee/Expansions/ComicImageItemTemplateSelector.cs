using BrilliantSee.Models;
using BrilliantSee.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.Expansions
{
    public class ComicImageItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? LoadingTemplate { get; set; }

        public DataTemplate? SuccessTemplate { get; set; }

        public DataTemplate? FailedTemplate { get; set; }

        protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
        {
            if (item is ComicImageItem comicImageItem)
            {
                return comicImageItem.State switch
                {
                    ComicImageItemState.Loading => LoadingTemplate,
                    ComicImageItemState.Success => SuccessTemplate,
                    ComicImageItemState.Failed => FailedTemplate,
                    _ => throw new ArgumentException("Invalid state")
                };
            }

            throw new ArgumentException("Item is not a ComicImageItem");
        }
    }
}