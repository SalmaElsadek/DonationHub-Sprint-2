using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Application.Features.Campaigns.DTOs
{
    public class PublicCampaignDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal GoalAmount { get; set; }
        public string GoalCurrency { get; set; } = default!;
        public decimal RaisedAmount { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public string? BannerFileName { get; set; }
        public string Status { get; set; } = default!;
        public double ProgressPercent { get; set; }
        public int DaysLeft { get; set; }
    }
}
