using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;
using Microsoft.EntityFrameworkCore;

namespace CrmTechTitans.Data
{
    public class CrmInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new CrmContext(serviceProvider.GetRequiredService<DbContextOptions<CrmContext>>()))
            {
                try
                {
                    if (!context.Industries.Any())
                    {

                        context.Industries.AddRange(
                                new Industry { Name = "Oil and Gas Extraction", NAICS = 211 },
                                new Industry { Name = "Mining and Quarrying (Except Oil and Gas)", NAICS = 212 },
                                new Industry { Name = "Support Activities for Mining and Oil and Gas Extraction", NAICS = 213 },
                                new Industry { Name = "Utilities", NAICS = 221 },
                                new Industry { Name = "Construction of Buildings", NAICS = 236 },
                                new Industry { Name = "Heavy and Civil Engineering Construction", NAICS = 237 },
                                new Industry { Name = "Specialty Trade Contractors", NAICS = 238 },
                                new Industry { Name = "Food Manufacturing", NAICS = 311 },
                                new Industry { Name = "Beverage and Tobacco Product Manufacturing", NAICS = 312 },
                                new Industry { Name = "Clothing Manufacturing", NAICS = 315 },
                                new Industry { Name = "Wood Product Manufacturing", NAICS = 321 },
                                new Industry { Name = "Paper Manufacturing", NAICS = 322 },
                                new Industry { Name = "Printing and Related Support Activities", NAICS = 323 },
                                new Industry { Name = "Chemical Manufacturing", NAICS = 325 },
                                new Industry { Name = "Plastics and Rubber Products Manufacturing", NAICS = 326 },
                                new Industry { Name = "Non-Metallic Mineral Product Manufacturing", NAICS = 327 },
                                new Industry { Name = "Primary Metal Manufacturing", NAICS = 331 },
                                new Industry { Name = "Fabricated Metal Product Manufacturing", NAICS = 332 },
                                new Industry { Name = "Machinery Manufacturing", NAICS = 333 },
                                new Industry { Name = "Computer and Electronic Product Manufacturing", NAICS = 334 },
                                new Industry { Name = "Electrical Equipment, Appliance, and Component Manufacturing", NAICS = 335 },
                                new Industry { Name = "Transportation Equipment Manufacturing", NAICS = 336 },
                                new Industry { Name = "Miscellaneous Manufacturing", NAICS = 339 },
                                new Industry { Name = "Farm Product Merchant Wholesalers", NAICS = 411 },
                                new Industry { Name = "Petroleum and Petroleum Products Merchant Wholesalers", NAICS = 412 },
                                new Industry { Name = "Building Material and Supplies Merchant Wholesalers", NAICS = 416 },
                                new Industry { Name = "Machinery, Equipment, and Supplies Merchant Wholesalers", NAICS = 417 },
                                new Industry { Name = "Miscellaneous Merchant Wholesalers", NAICS = 418 },
                                new Industry { Name = "Business to Business Electronic Markets and Agents and Brokers", NAICS = 419 },
                                new Industry { Name = "Motor Vehicle and Parts Dealers", NAICS = 441 },
                                new Industry { Name = "Building Material and Garden Equipment and Supplies Dealers", NAICS = 444 },
                                new Industry { Name = "Gasoline Stations", NAICS = 447 },
                                new Industry { Name = "Air Transportation", NAICS = 481 },
                                new Industry { Name = "Rail Transportation", NAICS = 482 },
                                new Industry { Name = "Water Transportation", NAICS = 483 },
                                new Industry { Name = "Truck Transportation", NAICS = 484 },
                                new Industry { Name = "Pipeline Transportation", NAICS = 486 },
                                new Industry { Name = "Support Activities for Transportation", NAICS = 488 },
                                new Industry { Name = "Couriers and Messengers", NAICS = 492 },
                                new Industry { Name = "Warehousing and Storage", NAICS = 493 },
                                new Industry { Name = "Telecommunications", NAICS = 517 },
                                new Industry { Name = "Data Processing, Hosting, and Related Services", NAICS = 518 },
                                new Industry { Name = "Other Information Services", NAICS = 519 },
                                new Industry { Name = "Credit Intermediation and Related Activities", NAICS = 522 },
                                new Industry { Name = "Securities, Commodity Contracts, and Other Financial Investment and Related Activities", NAICS = 523 },
                                new Industry { Name = "Insurance Carriers and Related Activities", NAICS = 524 },
                                new Industry { Name = "Funds and Other Financial Vehicles", NAICS = 526 },
                                new Industry { Name = "Real Estate", NAICS = 531 },
                                new Industry { Name = "Rental and Leasing Services", NAICS = 532 },
                                new Industry { Name = "Professional, Scientific, and Technical Services", NAICS = 541 },
                                new Industry { Name = "Management of Companies and Enterprises", NAICS = 551 },
                                new Industry { Name = "Administrative and Support Services", NAICS = 561 },
                                new Industry { Name = "Waste Management and Remediation Services", NAICS = 562 },
                                new Industry { Name = "Educational Services", NAICS = 611 },
                                new Industry { Name = "Repair and Maintenance", NAICS = 811 },
                                new Industry { Name = "Religious, Grantmaking, Civic, and Professional Organizations", NAICS = 813 },
                                new Industry { Name = "Federal Government Public Administration", NAICS = 911 },
                                new Industry { Name = "Provincial and Territorial Public Administration", NAICS = 912 },
                                new Industry { Name = "Local Municipal and Regional Public Administration", NAICS = 913 },
                                new Industry { Name = "Industrial Consulting", NAICS = 1000 },
                                new Industry { Name = "Other", NAICS = 0 });
                        context.SaveChanges();
                    }

                    if (!context.Members.Any())
                    {


                        context.Members.AddRange(

                               new Member
                               {

                                   MemberName = "Tech Solutions Inc.",
                                   MembershipType = MembershipType.Associate,
                                   ContactedBy = "John Doe",

                                   CompanySize = CompanySize.Large,
                                   CompanyWebsite = "https://www.techsolutions.com",
                                   MemberSince = new DateTime(2018, 5, 12),
                                   LastContactDate = new DateTime(2024, 1, 10),
                                   Notes = "Key partner for software development.",
                                   MembershipStatus = MembershipStatus.Active
                               },
                                new Member
                                {

                                    MemberName = "Energy Corp",
                                    MembershipType = MembershipType.Localindustrial,
                                    ContactedBy = "Jane Smith",

                                    CompanySize = CompanySize.Enterprise,
                                    CompanyWebsite = "https://www.energycorp.com",
                                    MemberSince = new DateTime(2015, 8, 25),
                                    LastContactDate = new DateTime(2023, 11, 20),
                                    Notes = "Major supplier of renewable energy.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "Green Innovations Ltd.",
                                    MembershipType = MembershipType.ChamberAssociate,
                                    ContactedBy = "Albert Green",

                                    CompanySize = CompanySize.Medium,
                                    CompanyWebsite = "https://www.greeninnovations.com",
                                    MemberSince = new DateTime(2017, 3, 10),
                                    LastContactDate = new DateTime(2024, 1, 5),
                                    Notes = "Innovators in green technology solutions.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "City Logistics",
                                    MembershipType = MembershipType.Localindustrial,
                                    ContactedBy = "Maria Johnson",

                                    CompanySize = CompanySize.Small,
                                    CompanyWebsite = "https://www.citylogistics.com",
                                    MemberSince = new DateTime(2020, 7, 18),
                                    LastContactDate = new DateTime(2023, 12, 22),
                                    Notes = "Local courier and delivery services.",
                                    MembershipStatus = MembershipStatus.Inactive
                                },
                                new Member
                                {

                                    MemberName = "HealthCare Plus",
                                    MembershipType = MembershipType.GovernmentEducationAssociate,
                                    ContactedBy = "Emily Wright",

                                    CompanySize = CompanySize.Enterprise,
                                    CompanyWebsite = "https://www.healthcareplus.com",
                                    MemberSince = new DateTime(2012, 2, 6),
                                    LastContactDate = new DateTime(2023, 10, 30),
                                    Notes = "Leading healthcare services provider.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "Tech Electronics",
                                    MembershipType = MembershipType.NonLocalIndustrial,
                                    ContactedBy = "James Carter",

                                    CompanySize = CompanySize.Large,
                                    CompanyWebsite = "https://www.techelectronics.com",
                                    MemberSince = new DateTime(2016, 9, 14),
                                    LastContactDate = new DateTime(2024, 1, 12),
                                    Notes = "Global leader in electronic components.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "Auto Parts Co.",
                                    MembershipType = MembershipType.Associate,
                                    ContactedBy = "Daniel Lee",

                                    CompanySize = CompanySize.Medium,
                                    CompanyWebsite = "https://www.autopartsco.com",
                                    MemberSince = new DateTime(2019, 6, 22),
                                    LastContactDate = new DateTime(2024, 1, 8),
                                    Notes = "Supplier of auto parts to regional stores.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "Foodies Market",
                                    MembershipType = MembershipType.ChamberAssociate,
                                    ContactedBy = "Linda Miller",

                                    CompanySize = CompanySize.Small,
                                    CompanyWebsite = "https://www.foodiesmarket.com",
                                    MemberSince = new DateTime(2021, 4, 4),
                                    LastContactDate = new DateTime(2023, 12, 15),
                                    Notes = "Specializing in local organic food products.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "Global Transport Ltd.",
                                    MembershipType = MembershipType.Localindustrial,
                                    ContactedBy = "Nina Roberts",

                                    CompanySize = CompanySize.Enterprise,
                                    CompanyWebsite = "https://www.globaltransport.com",
                                    MemberSince = new DateTime(2014, 10, 19),
                                    LastContactDate = new DateTime(2023, 11, 30),
                                    Notes = "Leading provider of international shipping solutions.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "Universal Tech Hub",
                                    MembershipType = MembershipType.Associate,
                                    ContactedBy = "Oliver Thomas",

                                    CompanySize = CompanySize.Large,
                                    CompanyWebsite = "https://www.universaltechhub.com",
                                    MemberSince = new DateTime(2013, 1, 9),
                                    LastContactDate = new DateTime(2023, 12, 10),
                                    Notes = "Cloud services and data hosting provider.",
                                    MembershipStatus = MembershipStatus.Active
                                }

                        );
                        context.SaveChanges();

                    }
                    if (!context.IndustryMembers.Any())
                    {
                        context.IndustryMembers.AddRange(
                            new MemberIndustry { IndustryID = 1, MemberID = 1 },
                            new MemberIndustry { IndustryID = 2, MemberID = 2 },
                            new MemberIndustry { IndustryID = 2, MemberID = 3 },
                             new MemberIndustry { IndustryID = 1, MemberID = 4 },
                            new MemberIndustry { IndustryID = 2, MemberID = 5 },
                            new MemberIndustry { IndustryID = 2, MemberID = 6 },
                             new MemberIndustry { IndustryID = 1, MemberID = 7 },
                            new MemberIndustry { IndustryID = 2, MemberID = 8 },
                            new MemberIndustry { IndustryID = 2, MemberID = 9 },
                             new MemberIndustry { IndustryID = 2, MemberID = 10 }
                        ); context.SaveChanges();
                    }
                    //Seed data for address -BP 01-22-2025
                    if (!context.Addresses.Any())
                    {
                        context.Addresses.AddRange(
                            new Address
                            {
                                Street = "123 Main St",
                                City = "Calgary",
                                Province = Province.Alberta,
                                PostalCode = "T2P 1J9"
                            },
                            new Address
                            {
                                Street = "456 Elm St",
                                City = "Vancouver",
                                Province = Province.BritishColumbia,
                                PostalCode = "V5K 0A1"
                            },
                            new Address
                            {
                                Street = "789 Maple Ave",
                                City = "Toronto",
                                Province = Province.Ontario,
                                PostalCode = "M5H 2N2"
                            },
                            new Address
                            {
                                Street = "101 Pine St",
                                City = "Montreal",
                                Province = Province.Quebec,
                                PostalCode = "H3B 1A7"
                            },
                            new Address
                            {
                                Street = "202 Oak St",
                                City = "Halifax",
                                Province = Province.NovaScotia,
                                PostalCode = "B3J 2K9"
                            }
                        );
                        context.SaveChanges();
                    }

                    //Seed data for Contact (Ayebanimi Edougha)
                    if (!context.Contacts.Any())
                    {
                        context.Contacts.AddRange(
                            new Contact
                            {
                                FirstName = "John",
                                LastName = "Doe",
                                Email = "JohnDoe@gmail.com",
                                Phone = "9058885301",
                                Linkedin = "https://linkedin.com/in/johndoe"
                            },
                            new Contact
                            {
                                FirstName = "Jane",
                                LastName = "Smith",
                                Email = "jane.smith@gmail.com",
                                Phone = "0987654321",
                                Linkedin = "https://linkedin.com/in/janesmith"
                            },
                            new Contact
                            {
                                FirstName = "Albert",
                                LastName = "Green",
                                Email = "albertgreen@greeninnovations.com",
                                Phone = "1122334455",
                                Linkedin = "https://linkedin.com/in/albertgreen"
                            },
                            new Contact
                            {
                                FirstName = "Maria",
                                LastName = "Johnson",
                                Email = "mariajohnson@citylogistics.com",
                                Phone = "2233445566",
                                Linkedin = "https://linkedin.com/in/mariajohnson"
                            },
                            new Contact
                            {
                                FirstName = "Emily",
                                LastName = "Wright",
                                Email = "emilywright@healthcareplus.com",
                                Phone = "3344556677",
                                Linkedin = "https://linkedin.com/in/emilywright"
                            },
                            new Contact
                            {
                                FirstName = "James",
                                LastName = "Carter",
                                Email = "jamescarter@techelectronics.com",
                                Phone = "4455667788",
                                Linkedin = "https://linkedin.com/in/jamescarter"
                            },
                            new Contact
                            {
                                FirstName = "Daniel",
                                LastName = "Lee",
                                Email = "daniellee@autopartsco.com",
                                Phone = "5566778899",
                                Linkedin = "https://linkedin.com/in/daniellee"
                            },
                            new Contact
                            {
                                FirstName = "Linda",
                                LastName = "Miller",
                                Email = "lindamiller@foodiesmarket.com",
                                Phone = "6677889900",
                                Linkedin = "https://linkedin.com/in/lindamiller"
                            },
                            new Contact
                            {
                                FirstName = "Nina",
                                LastName = "Roberts",
                                Email = "ninaroberts@globaltransport.com",
                                Phone = "7788990011",
                                Linkedin = "https://linkedin.com/in/ninaroberts"
                            },
                            new Contact
                            {
                                FirstName = "Oliver",
                                LastName = "Thomas",
                                Email = "oliverthomas@universaltechhub.com",
                                Phone = "8899001122",
                                Linkedin = "https://linkedin.com/in/oliverthomas"
                            }
                            );
                        context.SaveChanges();
                    }

                    //Seed data for MemberContact (Ayebanimi Edougha)
                    if (!context.MemberContacts.Any())
                    {
                        context.MemberContacts.AddRange(
                            new MemberContact { MemberID = 1, ContactID = 1, ContactType = ContactType.VIP },
                            new MemberContact { MemberID = 2, ContactID = 2, ContactType = ContactType.General },
                            new MemberContact { MemberID = 3, ContactID = 3, ContactType = ContactType.VIP },
                            new MemberContact { MemberID = 4, ContactID = 4, ContactType = ContactType.General },
                            new MemberContact { MemberID = 5, ContactID = 5, ContactType = ContactType.VIP },
                            new MemberContact { MemberID = 6, ContactID = 6, ContactType = ContactType.General },
                            new MemberContact { MemberID = 7, ContactID = 7, ContactType = ContactType.VIP },
                            new MemberContact { MemberID = 8, ContactID = 8, ContactType = ContactType.General },
                            new MemberContact { MemberID = 9, ContactID = 9, ContactType = ContactType.VIP },
                            new MemberContact { MemberID = 10, ContactID = 10, ContactType = ContactType.General },
                             new MemberContact { MemberID = 1, ContactID = 10, ContactType = ContactType.General }
                        ); context.SaveChanges();
                    }
                    // Seed data for MemberAddress (Braydon Pew 01-22-2025)
                    if (!context.MemberAddresses.Any())
                    {
                        context.MemberAddresses.AddRange(
                            new MemberAddress { MemberID = 1, AddressID = 1, AddressType = AddressType.Office },
                            new MemberAddress { MemberID = 2, AddressID = 2, AddressType = AddressType.Billing },
                            new MemberAddress { MemberID = 3, AddressID = 3, AddressType = AddressType.Shipping },
                            new MemberAddress { MemberID = 4, AddressID = 4, AddressType = AddressType.Office },
                            new MemberAddress { MemberID = 5, AddressID = 5, AddressType = AddressType.Billing },
                            new MemberAddress { MemberID = 6, AddressID = 1, AddressType = AddressType.Shipping },
                            new MemberAddress { MemberID = 7, AddressID = 2, AddressType = AddressType.Office },
                            new MemberAddress { MemberID = 8, AddressID = 3, AddressType = AddressType.Billing },
                            new MemberAddress { MemberID = 9, AddressID = 4, AddressType = AddressType.Shipping },
                            new MemberAddress { MemberID = 10, AddressID = 5, AddressType = AddressType.Office }
                        );
                        context.SaveChanges();
                    }

                    //Seed data for Opportunity (Jagraj Grewal)

                    if (!context.Opportunities.Any())
                    {
                        context.Opportunities.AddRange(
                           new Opportunity
                           {

                               Title = "New Business Opportunity - Tech Innovations",
                               Status = Status.Qualification,
                               Description = "Exploring new partnership opportunities in the tech sector.",
                               Priority = PriorityType.High
                           },
                            new Opportunity
                            {

                                Title = "Member Acquisition Campaign",
                                Status = Status.Negotiating,
                                Description = "Negotiating with potential members to join our platform.",
                                Priority = PriorityType.Medium
                            },
                            new Opportunity
                            {

                                Title = "Major Investment Round",
                                Status = Status.ClosedNewMember,
                                Description = "Closed with a new member after a successful investment round.",
                                Priority = PriorityType.Urgent
                            },
                            new Opportunity
                            {

                                Title = "Product Launch Collaboration",
                                Status = Status.ClosedNotInterested,
                                Description = "Collaboration proposal for an upcoming product launch rejected.",
                                Priority = PriorityType.Low
                            },
                            new Opportunity
                            {

                                Title = "AI Partnership Proposal",
                                Status = Status.Qualification,
                                Description = "Exploring AI-based partnership opportunities with leading companies.",
                                Priority = PriorityType.High
                            },
                            new Opportunity
                            {

                                Title = "International Expansion",
                                Status = Status.Negotiating,
                                Description = "Negotiating market expansion in European and Asian markets.",
                                Priority = PriorityType.Medium
                            },
                            new Opportunity
                            {

                                Title = "SaaS Product Marketing Campaign",
                                Status = Status.ClosedNewMember,
                                Description = "Successfully launched a SaaS product in North America.",
                                Priority = PriorityType.Urgent
                            },
                            new Opportunity
                            {

                                Title = "New Member Subscription Plan",
                                Status = Status.ClosedNotInterested,
                                Description = "Subscription plan proposal rejected by the client.",
                                Priority = PriorityType.Low
                            },
                            new Opportunity
                            {

                                Title = "Cloud Storage Partnership",
                                Status = Status.Qualification,
                                Description = "Initiating talks with cloud storage providers for strategic partnerships.",
                                Priority = PriorityType.Medium
                            },
                            new Opportunity
                            {

                                Title = "E-Commerce Platform Integration",
                                Status = Status.Negotiating,
                                Description = "Negotiating with e-commerce platforms for integration with our solutions.",
                                Priority = PriorityType.High
                            }
                        );
                        context.SaveChanges();

                    }

                    if (!context.MemberOpportunities.Any())
                    {
                        context.MemberOpportunities.AddRange(
                            new MemberOpportunity { MemberID = 1, OpportunityID = 1 },
                            new MemberOpportunity { MemberID = 2, OpportunityID = 2 },
                            new MemberOpportunity { MemberID = 3, OpportunityID = 3 },
                            new MemberOpportunity { MemberID = 4, OpportunityID = 4 },
                            new MemberOpportunity { MemberID = 5, OpportunityID = 5 },
                            new MemberOpportunity { MemberID = 6, OpportunityID = 6 },
                            new MemberOpportunity { MemberID = 7, OpportunityID = 7 },
                            new MemberOpportunity { MemberID = 8, OpportunityID = 8 },
                            new MemberOpportunity { MemberID = 9, OpportunityID = 9 },
                            new MemberOpportunity { MemberID = 10, OpportunityID = 10 }
                        );
                        context.SaveChanges();
                    }


                }
                catch (Exception ex)
                {
                    // Log the exception details --bp 01-23-2025
                    Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }


        }
    }
}
