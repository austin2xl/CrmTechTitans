using CrmTechTitans.Models;
using CrmTechTitans.Models.JoinTables;
using Microsoft.EntityFrameworkCore;

namespace CrmTechTitans.Data
{
    public class CrmContext : DbContext
    {
        public CrmContext(DbContextOptions<CrmContext> options)
            : base(options)
        {
        }
        public DbSet<Member> Members { get; set; }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Opportunity> Opportunities { get; set; }

        public DbSet<Interaction> Interactions { get; set; }

        public DbSet<Industry> Industries { get; set; }

        public DbSet<MemberIndustry> IndustryMembers { get; set; }

        public DbSet<MemberAddress> MemberAddresses { get; set; }

        public DbSet<MemberContact> MemberContacts { get; set; }

        public DbSet<MemberOpportunity> MemberOpportunities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the many-to-many relationship
            modelBuilder.Entity<MemberIndustry>()
                .HasKey(im => new { im.IndustryID, im.MemberID });

            modelBuilder.Entity<MemberIndustry>()
                .HasOne(im => im.Industry)
                .WithMany(i => i.IndustryMembers)
                .HasForeignKey(im => im.IndustryID);

            modelBuilder.Entity<MemberIndustry>()
                .HasOne(im => im.Member)
                .WithMany(m => m.IndustryMembers)
                .HasForeignKey(im => im.MemberID);

            // Configure the many-to-many relationship for MemberAddress
            modelBuilder.Entity<MemberAddress>()
                .HasKey(ma => new { ma.MemberID, ma.AddressID });

            modelBuilder.Entity<MemberAddress>()
                .HasOne(ma => ma.Member)
                .WithMany(m => m.MemberAddresses)
                .HasForeignKey(ma => ma.MemberID);

            modelBuilder.Entity<MemberAddress>()
                .HasOne(ma => ma.Address)
                  .WithMany(m => m.MemberAddresses)
                .HasForeignKey(ma => ma.AddressID);

            //Configure Many-to-Many relationship for MemberContact

            modelBuilder.Entity<MemberContact>()
              .HasKey(mc => new { mc.ContactID, mc.MemberID });

            modelBuilder.Entity<MemberContact>()
                .HasOne(mc => mc.Contact)
                .WithMany(c => c.MemberContacts)
                .HasForeignKey(mc => mc.ContactID);

            modelBuilder.Entity<MemberContact>()
                .HasOne(mc => mc.Member)
                .WithMany(m => m.MemberContacts)
                .HasForeignKey(mc => mc.MemberID);

            modelBuilder.Entity<MemberContact>()
            .Property(mc => mc.ContactType)
            .HasConversion<int>();

            //Configure Many-to-Many relationship for MemberContact

            modelBuilder.Entity<MemberOpportunity>()
              .HasKey(mc => new { mc.OpportunityID, mc.MemberID });

            modelBuilder.Entity<MemberOpportunity>()
                .HasOne(mc => mc.Opportunity)
                .WithMany(c => c.MemberOpportunities)
                .HasForeignKey(mc => mc.OpportunityID);

            modelBuilder.Entity<MemberOpportunity>()
                .HasOne(mc => mc.Member)
                .WithMany(m => m.MemberOpportunities)
                .HasForeignKey(mc => mc.MemberID);
        }


    }


}
