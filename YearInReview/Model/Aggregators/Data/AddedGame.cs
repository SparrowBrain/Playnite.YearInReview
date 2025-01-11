using System;
using Playnite.SDK.Models;

namespace YearInReview.Model.Aggregators.Data
{
    public class AddedGame
    {
        public DateTime AddedDate { get; set; }

        public Game Game { get; set; }
    }
}