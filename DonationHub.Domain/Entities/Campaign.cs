using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Domain.Common;
using DonationHub.Domain.Enum;
using DonationHub.Domain.ValueObjects;
namespace DonationHub.Domain.Entities
{
    public class Campaign : BaseEntity, ISoftDeletable
    {
        public Campaign()
        {
        }

        public string TitleAr { get; private set; } = default!;
        public string TitleEn { get; private set; } = default!;
        public string DescriptionAr { get; private set; } = default!;
        public string DescriptionEn { get; private set; } = default!;
        public Money GoalAmount { get; private set; }
        public decimal RaisedAmount { get; private set; }
        public DateTimeOffset EndDate { get; private set; }
        public Guid CategoryId { get; private set; }
        public Category Category { get; private set; }
        public string? BannerFileName { get;private set; } = default!;
        public CampaignStatus Status { get;private set; }
        public Guid CreatedByUserId { get;private set; }
        public DateTimeOffset UpdatedAt { get; private set; }
        //Ask1
        public byte[] RowVersion { get; private set; } = [];

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        private IReadOnlyCollection<Donation> Donations = [];

        public static Campaign Create(string titleAr,string titleEn,string descriptionAr
            ,string descriptionEn, Money goalAmount,DateTimeOffset endDate,
        Guid categoryId,Guid createdByUserId,DateTimeOffset time,
        string? bannerFileName = null)
        {
            if (endDate <= time)
                throw new ArgumentException("End time should not be Expired", nameof(endDate));
            
            if (string.IsNullOrWhiteSpace(titleEn) || string.IsNullOrWhiteSpace(titleAr))
                throw new ArgumentException("Titles are required",nameof(titleEn));

            if (createdByUserId == Guid.Empty)
                throw new ArgumentException("User Id Can't be null", nameof(createdByUserId));

            return new Campaign
            {
                TitleAr = titleAr,
                TitleEn = titleEn,
                DescriptionAr = descriptionAr,
                DescriptionEn = descriptionEn,
                GoalAmount = goalAmount,
                RaisedAmount = 0,
                EndDate = endDate,
                CategoryId = categoryId,
                CreatedByUserId = createdByUserId,
                Status = CampaignStatus.Active,
                BannerFileName = bannerFileName,
                UpdatedAt = time
            };

        }
        public void UpdateEndDate(DateTimeOffset time,DateTimeOffset endDate)
        {
            if (Status == CampaignStatus.Completed || Status == CampaignStatus.Expired)
                throw new InvalidOperationException("Can't change the EndDate of completed or expired campaign");

            if (endDate <= time) throw new ArgumentException("the new date should be in the future");

            EndDate = endDate;
            UpdatedAt = time;
        }

        //GoalAmount cannot be lowered below the current RaisedAmount.
        public void UpdateGoalAmount(Money goalAmount,DateTimeOffset time)
        {
            if (Status == CampaignStatus.Completed || Status == CampaignStatus.Expired) 
                throw new InvalidOperationException("Cannot update a completed or expired campaign.");
            if (goalAmount.Amount <= 0)
                throw new ArgumentException("The amount should be greater than 0", nameof(goalAmount));
            if (goalAmount.Amount < RaisedAmount)
                throw new ArgumentException("GoalAmount cannot be lowered below the current RaisedAmount.");

            GoalAmount = goalAmount;
            UpdatedAt= time;

        }

        public void Update(
            string titleAr,
            string titleEn,
            string descriptionAr,
            string descriptionEn,
            Money goalAmount,
            DateTimeOffset endDate,
            Guid categoryId,
            DateTimeOffset time)
        {
            if (Status == CampaignStatus.Completed || Status == CampaignStatus.Expired)
                throw new InvalidOperationException("Cannot update a completed or expired campaign.");

            if (endDate <= time)
                throw new ArgumentException("End date must be in the future.", nameof(endDate));

            if (goalAmount.Amount <= 0)
                throw new ArgumentException("Goal amount must be greater than zero.", nameof(goalAmount));

            if (goalAmount.Amount < RaisedAmount)
                throw new ArgumentException("Goal amount cannot be lower than the raised amount.", nameof(goalAmount));

            if (string.IsNullOrWhiteSpace(titleAr) || string.IsNullOrWhiteSpace(titleEn))
                throw new ArgumentException("Titles are required.");

            if (categoryId == Guid.Empty)
                throw new ArgumentException("Category id is required.", nameof(categoryId));

            TitleAr = titleAr;
            TitleEn = titleEn;
            DescriptionAr = descriptionAr;
            DescriptionEn = descriptionEn;
            GoalAmount = goalAmount;
            EndDate = endDate;
            CategoryId = categoryId;
            UpdatedAt = time;
        }
        public bool CanAcceptDonation(DateTimeOffset time)
        {
            if (Status == CampaignStatus.Active && EndDate > time && !IsDeleted) return true;
            else return false;
        }

        public void AddDonation(decimal money,DateTimeOffset now)
        {
            if (!CanAcceptDonation(now))
                throw new InvalidOperationException("Can't accept donations now");
            
            if(money< 0) throw new ArgumentException("The Donation money should be positive",nameof(money));

            RaisedAmount += money;
            UpdatedAt = now;

            if (RaisedAmount >= GoalAmount.Amount) MakeItCompleted(now);
        }

        public void MakeItCompleted(DateTimeOffset time)
        {
            if (Status == CampaignStatus.Completed || Status == CampaignStatus.Expired) return;

            Status = CampaignStatus.Completed;
            UpdatedAt=time;
        }
        public void MakeItExpired(DateTimeOffset time)
        {
            if (Status == CampaignStatus.Completed || Status == CampaignStatus.Expired) return;
            if (EndDate <= time)
            {
                Status= CampaignStatus.Expired;
                UpdatedAt= time;
            }
        }
        //A campaign that has donations cannot be hard-deleted
        public bool CanBeDeleted() => Donations.Count == 0;
        
    }
}
