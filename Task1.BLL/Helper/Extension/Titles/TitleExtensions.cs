using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.TitleDTOs;
using Task1.DAL.Entities;

namespace Task1.BLL.Helper.Extension.Titles
{
    public static class TitleExtensions
    {
        public static IEnumerable<Title> ApplyFilter(this IEnumerable<Title> titles, GetTitleDTO filters)
        {
            return titles.Where(x =>
                (filters.TitleId == null || x.TitleId.Equals(filters.TitleId)) &&

                (filters.Title1 == null || x.Title1.IndexOf(filters.Title1, StringComparison.OrdinalIgnoreCase) >= 0) &&

                (filters.Type == null || x.Type.IndexOf(filters.Type, StringComparison.OrdinalIgnoreCase) >= 0) &&

                (filters.PubId == null || x.PubId.Equals(filters.PubId)) &&

                (!filters.FromPrice.HasValue || x.Price >= filters.FromPrice) &&
                (!filters.ToPrice.HasValue || x.Price <= filters.ToPrice) &&

                (!filters.FromAdvance.HasValue || x.Advance >= filters.FromAdvance) &&
                (!filters.ToAdvance.HasValue || x.Advance <= filters.ToAdvance) &&

                (!filters.FromRoyalty.HasValue || x.Royalty >= filters.FromRoyalty) &&
                (!filters.ToRoyalty.HasValue || x.Royalty <= filters.ToRoyalty) &&

                (!filters.FromYtdSales.HasValue || x.YtdSales >= filters.FromYtdSales) &&
                (!filters.ToYtdSales.HasValue || x.YtdSales <= filters.ToYtdSales) &&

                (filters.Notes == null || x.Notes.IndexOf(filters.Notes, StringComparison.OrdinalIgnoreCase) >= 0) &&

                (!filters.FromPubdate.HasValue || x.Pubdate.Date >= filters.FromPubdate.Value.Date) &&
                (!filters.ToPubdate.HasValue || x.Pubdate.Date <= filters.ToPubdate.Value.Date)
            );
        }
    }

}
