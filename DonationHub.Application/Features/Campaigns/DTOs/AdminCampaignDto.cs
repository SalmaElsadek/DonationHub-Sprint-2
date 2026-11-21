using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Application.Features.Campaigns.DTOs
{
    public class AdminCampaignDto
    {
        public Guid Id { get; set; }
        public string TitleAr { get; set; } = default!;
        public string TitleEn { get; set; } = default!;
        public string DescriptionAr { get; set; } = default!;
        public string DescriptionEn { get; set; } = default!;
        public decimal GoalAmount { get; set; }
        public string GoalCurrency { get; set; } = default!;
        public decimal RaisedAmount { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public Guid CategoryId { get; set; }
        public string? BannerFileName { get; set; }
        public string Status { get; set; } = default!;
        public Guid CreatedByUserId { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = [];
        public double ProgressPercent { get; set; }
        public int DaysLeft { get; set; }

    }
}
