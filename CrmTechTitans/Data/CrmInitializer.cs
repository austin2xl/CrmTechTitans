using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    context.Database.Migrate(); // Ensure the latest migrations are applied

                    // Seed Industries
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
                            new Industry { Name = "Other", NAICS = 0 }
                        );
                        context.SaveChanges();
                    }

                    // Seed Membership Types
                    if (!context.MembershipTypes.Any())
                    {
                        context.MembershipTypes.AddRange(

                            new MembershipType { ID = 1, Name = "Associate" },
                            new MembershipType { ID = 2, Name = "Local" },
                            new MembershipType { ID = 3, Name = "Chamber" },
                            new MembershipType { ID = 4, Name = "Government Education" },
                            new MembershipType { ID = 5, Name = "Non-Local" },
                            new MembershipType { ID = 6, Name = "Industrial" },
                            new MembershipType { ID = 7, Name = "Other" }
                            );
                        context.SaveChanges();
                    }

                    // Fetch all Membership Types from the database
                    var membershipTypes = context.MembershipTypes.ToDictionary(m => m.Name, m => m);

                    // Helper function to assign multiple membership types
                    List<MemberMembershipType> AssignMembershipTypes(Member member, params string[] membershipTypeNames)
                    {
                        var assignedTypes = new List<MemberMembershipType>();

                        foreach (var typeName in membershipTypeNames)
                        {
                            if (membershipTypes.TryGetValue(typeName, out var membershipType))
                            {
                                assignedTypes.Add(new MemberMembershipType
                                {
                                    Member = member,
                                    MembershipType = membershipType
                                });
                            }
                            else
                            {
                                Console.WriteLine($"MembershipType '{typeName}' not found for Member '{member.MemberName}'.");
                            }
                        }

                        return assignedTypes;
                    }

                    // Seed Members
                    if (!context.Members.Any())
                    {
                        var members = new List<Member>
                        {
                            new Member
                            {
                                MemberName = "Tech Solutions Inc.",
                                ContactedBy = "John Doe",
                                CompanySize = CompanySize.TwoHundredOneToThousand,
                                CompanyWebsite = "https://www.techsolutions.com",
                                MemberSince = new DateTime(2018, 5, 12),
                                LastContactDate = new DateTime(2024, 1, 10),
                                Notes = "Key partner for software development.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Energy Corp",
                                ContactedBy = "Jane Smith",
                                CompanySize = CompanySize.ThousandPlus,
                                CompanyWebsite = "https://www.energycorp.com",
                                MemberSince = new DateTime(2015, 8, 25),
                                LastContactDate = new DateTime(2023, 11, 20),
                                Notes = "Major supplier of renewable energy.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "Green Innovations Ltd.",
                                ContactedBy = "Albert Green",
                                CompanySize = CompanySize.FiftyOneToTwoHundred,
                                CompanyWebsite = "https://www.greeninnovations.com",
                                MemberSince = new DateTime(2017, 3, 10),
                                LastContactDate = new DateTime(2024, 1, 5),
                                Notes = "Innovators in green technology solutions.",
                                MembershipStatus = MembershipStatus.Cancelled
                            },
                            new Member
                            {
                                MemberName = "City Logistics",
                                ContactedBy = "Maria Johnson",
                                CompanySize = CompanySize.ElevenToFifty,
                                CompanyWebsite = "https://www.citylogistics.com",
                                MemberSince = new DateTime(2020, 7, 18),
                                LastContactDate = new DateTime(2023, 12, 22),
                                Notes = "Local courier and delivery services.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "HealthCare Plus",
                                ContactedBy = "Emily Wright",
                                CompanySize = CompanySize.ThousandPlus,
                                CompanyWebsite = "https://www.healthcareplus.com",
                                MemberSince = new DateTime(2012, 2, 6),
                                LastContactDate = new DateTime(2023, 10, 30),
                                Notes = "Leading healthcare services provider.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Tech Electronics",
                                ContactedBy = "James Carter",
                                CompanySize = CompanySize.TwoHundredOneToThousand,
                                CompanyWebsite = "https://www.techelectronics.com",
                                MemberSince = new DateTime(2016, 9, 14),
                                LastContactDate = new DateTime(2024, 1, 12),
                                Notes = "Global leader in electronic components.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "Auto Parts Co.",
                                ContactedBy = "Daniel Lee",
                                CompanySize = CompanySize.FiftyOneToTwoHundred,
                                CompanyWebsite = "https://www.autopartsco.com",
                                MemberSince = new DateTime(2019, 6, 22),
                                LastContactDate = new DateTime(2024, 1, 8),
                                Notes = "Supplier of auto parts to regional stores.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "Foodies Market",
                                ContactedBy = "Linda Miller",
                                CompanySize = CompanySize.ElevenToFifty,
                                CompanyWebsite = "https://www.foodiesmarket.com",
                                MemberSince = new DateTime(2021, 4, 4),
                                LastContactDate = new DateTime(2023, 12, 15),
                                Notes = "Specializing in local organic food products.",
                                MembershipStatus = MembershipStatus.Cancelled
                            },
                            new Member
                            {
                                MemberName = "Global Transport Ltd.",
                                ContactedBy = "Nina Roberts",
                                CompanySize = CompanySize.ThousandPlus,
                                CompanyWebsite = "https://www.globaltransport.com",
                                MemberSince = new DateTime(2014, 10, 19),
                                LastContactDate = new DateTime(2023, 11, 30),
                                Notes = "Leading provider of international shipping solutions.",
                                MembershipStatus = MembershipStatus.Cancelled
                            },
                            new Member
                            {
                                MemberName = "Universal Tech Hub",
                                ContactedBy = "Oliver Thomas",
                                CompanySize = CompanySize.TwoHundredOneToThousand,
                                CompanyWebsite = "https://www.universaltechhub.com",
                                MemberSince = new DateTime(2013, 1, 9),
                                LastContactDate = new DateTime(2023, 12, 10),
                                Notes = "Cloud services and data hosting provider.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Green Energy Solutions",
                                ContactedBy = "Michael Green",
                                CompanySize = CompanySize.TwoHundredOneToThousand,
                                CompanyWebsite = "https://www.greenenergysolutions.com",
                                MemberSince = new DateTime(2015, 3, 18),
                                LastContactDate = new DateTime(2023, 11, 25),
                                Notes = "Pioneers in renewable energy technologies.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Bright Future Education",
                                ContactedBy = "Sarah Johnson",
                                CompanySize = CompanySize.FiftyOneToTwoHundred,
                                CompanyWebsite = "https://www.brightfutureedu.com",
                                MemberSince = new DateTime(2018, 7, 9),
                                LastContactDate = new DateTime(2024, 1, 10),
                                Notes = "Providing innovative educational tools and resources.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "Oceanic Travels",
                                ContactedBy = "David Brown",
                                CompanySize = CompanySize.ElevenToFifty,
                                CompanyWebsite = "https://www.oceanictravels.com",
                                MemberSince = new DateTime(2020, 5, 12),
                                LastContactDate = new DateTime(2023, 12, 20),
                                Notes = "Specializing in luxury sea travel experiences.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "NextGen Robotics",
                                ContactedBy = "Alex Smith",
                                CompanySize = CompanySize.ThousandPlus,
                                CompanyWebsite = "https://www.nextgenrobotics.com",
                                MemberSince = new DateTime(2014, 8, 22),
                                LastContactDate = new DateTime(2024, 1, 5),
                                Notes = "Innovators in robotics and automation.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "Urban Fashion Hub",
                                ContactedBy = "Jessica White",
                                CompanySize = CompanySize.FiftyOneToTwoHundred,
                                CompanyWebsite = "https://www.urbanfashionhub.com",
                                MemberSince = new DateTime(2017, 11, 30),
                                LastContactDate = new DateTime(2023, 12, 18),
                                Notes = "Trendsetters in urban and streetwear fashion.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Sky High Drones",
                                ContactedBy = "Ryan Adams",
                                CompanySize = CompanySize.ElevenToFifty,
                                CompanyWebsite = "https://www.skyhighdrones.com",
                                MemberSince = new DateTime(2022, 2, 14),
                                LastContactDate = new DateTime(2024, 1, 15),
                                Notes = "Experts in commercial and recreational drone technology.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "Pure Water Tech",
                                ContactedBy = "Olivia Martinez",
                                CompanySize = CompanySize.TwoHundredOneToThousand,
                                CompanyWebsite = "https://www.purewatertech.com",
                                MemberSince = new DateTime(2013, 6, 5),
                                LastContactDate = new DateTime(2023, 11, 30),
                                Notes = "Leaders in water purification and filtration systems.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Global Logistics Inc.",
                                ContactedBy = "Ethan Wilson",
                                CompanySize = CompanySize.ThousandPlus,
                                CompanyWebsite = "https://www.globallogisticsinc.com",
                                MemberSince = new DateTime(2011, 4, 19),
                                LastContactDate = new DateTime(2024, 1, 20),
                                Notes = "Comprehensive logistics and supply chain solutions.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "Creative Minds Agency",
                                ContactedBy = "Sophia Garcia",
                                CompanySize = CompanySize.FiftyOneToTwoHundred,
                                CompanyWebsite = "https://www.creativemindsagency.com",
                                MemberSince = new DateTime(2019, 9, 25),
                                LastContactDate = new DateTime(2023, 12, 10),
                                Notes = "Full-service marketing and creative agency.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Eco Home Builders",
                                ContactedBy = "Noah Clark",
                                CompanySize = CompanySize.ElevenToFifty,
                                CompanyWebsite = "https://www.ecohomebuilders.com",
                                MemberSince = new DateTime(2021, 3, 8),
                                LastContactDate = new DateTime(2024, 1, 18),
                                Notes = "Sustainable and eco-friendly home construction.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "Tech Innovate",
                                ContactedBy = "Ava Rodriguez",
                                CompanySize = CompanySize.TwoHundredOneToThousand,
                                CompanyWebsite = "https://www.techinnovate.com",
                                MemberSince = new DateTime(2016, 12, 1),
                                LastContactDate = new DateTime(2023, 11, 28),
                                Notes = "Driving innovation in software and hardware solutions.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Summit Adventures",
                                ContactedBy = "Liam Hernandez",
                                CompanySize = CompanySize.FiftyOneToTwoHundred,
                                CompanyWebsite = "https://www.summitadventures.com",
                                MemberSince = new DateTime(2018, 4, 17),
                                LastContactDate = new DateTime(2024, 1, 14),
                                Notes = "Organizing extreme and adventure travel experiences.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "Fresh Bites Catering",
                                ContactedBy = "Isabella Martinez",
                                CompanySize = CompanySize.ElevenToFifty,
                                CompanyWebsite = "https://www.freshbitescatering.com",
                                MemberSince = new DateTime(2020, 7, 22),
                                LastContactDate = new DateTime(2023, 12, 25),
                                Notes = "Gourmet catering services for all occasions.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Bright Star Media",
                                ContactedBy = "Mason Taylor",
                                CompanySize = CompanySize.ThousandPlus,
                                CompanyWebsite = "https://www.brightstarmedia.com",
                                MemberSince = new DateTime(2012, 10, 11),
                                LastContactDate = new DateTime(2024, 1, 22),
                                Notes = "Leading media production and broadcasting company.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "SafeGuard Insurance",
                                ContactedBy = "Charlotte Lewis",
                                CompanySize = CompanySize.TwoHundredOneToThousand,
                                CompanyWebsite = "https://www.safeguardinsurance.com",
                                MemberSince = new DateTime(2014, 5, 29),
                                LastContactDate = new DateTime(2023, 11, 20),
                                Notes = "Comprehensive insurance solutions for businesses and individuals.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Prime Fitness",
                                ContactedBy = "William Walker",
                                CompanySize = CompanySize.FiftyOneToTwoHundred,
                                CompanyWebsite = "https://www.primefitness.com",
                                MemberSince = new DateTime(2017, 8, 14),
                                LastContactDate = new DateTime(2024, 1, 16),
                                Notes = "State-of-the-art fitness centers and wellness programs.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "Golden Harvest Foods",
                                ContactedBy = "Amelia Young",
                                CompanySize = CompanySize.ElevenToFifty,
                                CompanyWebsite = "https://www.goldenharvestfoods.com",
                                MemberSince = new DateTime(2019, 2, 3),
                                LastContactDate = new DateTime(2023, 12, 12),
                                Notes = "Producers of high-quality, organic food products.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Apex Automotive",
                                ContactedBy = "Benjamin Allen",
                                CompanySize = CompanySize.ThousandPlus,
                                CompanyWebsite = "https://www.apexautomotive.com",
                                MemberSince = new DateTime(2013, 1, 15),
                                LastContactDate = new DateTime(2024, 1, 24),
                                Notes = "Manufacturers of high-performance automotive parts.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "ClearView Analytics",
                                ContactedBy = "Mia King",
                                CompanySize = CompanySize.TwoHundredOneToThousand,
                                CompanyWebsite = "https://www.clearviewanalytics.com",
                                MemberSince = new DateTime(2015, 7, 7),
                                LastContactDate = new DateTime(2023, 11, 15),
                                Notes = "Advanced data analytics and business intelligence solutions.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "BlueWave Technologies",
                                ContactedBy = "Ethan Scott",
                                CompanySize = CompanySize.FiftyOneToTwoHundred,
                                CompanyWebsite = "https://www.bluewavetech.com",
                                MemberSince = new DateTime(2018, 10, 9),
                                LastContactDate = new DateTime(2024, 1, 19),
                                Notes = "Innovative solutions in IoT and smart devices.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "Harmony Music School",
                                ContactedBy = "Elijah Harris",
                                CompanySize = CompanySize.FiftyOneToTwoHundred,
                                CompanyWebsite = "https://www.harmonymusicschool.com",
                                MemberSince = new DateTime(2019, 11, 22),
                                LastContactDate = new DateTime(2023, 12, 15),
                                Notes = "Music education and instrument sales.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Coastal Shipping Lines",
                                ContactedBy = "Lily Thomas",
                                CompanySize = CompanySize.ElevenToFifty,
                                CompanyWebsite = "https://www.coastalshippinglines.com",
                                MemberSince = new DateTime(2020, 4, 9),
                                LastContactDate = new DateTime(2024, 1, 8),
                                Notes = "Regional shipping and logistics company.",
                                MembershipStatus = MembershipStatus.OutStanding
                            },
                            new Member
                            {
                                MemberName = "Digital Marketing Pro",
                                ContactedBy = "Ryan Cooper",
                                CompanySize = CompanySize.ThousandPlus,
                                CompanyWebsite = "https://www.digitalmarketingpro.com",
                                MemberSince = new DateTime(2013, 9, 30),
                                LastContactDate = new DateTime(2023, 12, 22),
                                Notes = "Full-service digital marketing agency.",
                                MembershipStatus = MembershipStatus.GoodStanding
                            },
                            new Member
                            {
                                MemberName = "Quality Construction Group",
                                ContactedBy = "Jackson Hill",
                                CompanySize = CompanySize.TwoHundredOneToThousand,
                                CompanyWebsite = "https://www.qualityconstructiongroup.com",
                                MemberSince = new DateTime(2014, 10, 15),
                                LastContactDate = new DateTime(2024, 1, 12),
                                Notes = "Commercial and residential construction services.",
                                MembershipStatus = MembershipStatus.OutStanding
                            }
                        };

                        context.Members.AddRange(members);
                        context.SaveChanges();

                        // Assign multiple Membership Types to each member
                        var memberMembershipTypes = new List<MemberMembershipType>();

                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[0], "Associate", "Chamber"));
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[1], "Industrial", "Non-Local"));
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[2], "Chamber", "Government Education"));
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[3], "Local"));
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[4], "Government Education", "Other"));
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[5], "Associate", "Local Industrial"));
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[6], "Chamber", "Industrial"));
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[7], "Government Education", "Other"));
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[8], "Local Industrial", "Associate"));
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[9], "Associate", "Non-Local"));
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[10], "Associate", "Chamber")); // Green Energy Solutions
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[11], "Industrial", "Non-Local")); // Bright Future Education
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[12], "Chamber", "Government Education")); // Oceanic Travels
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[13], "Local")); // NextGen Robotics
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[14], "Government Education", "Other")); // Urban Fashion Hub
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[15], "Associate", "Local")); // Sky High Drones
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[16], "Chamber", "Industrial")); // Pure Water Tech
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[17], "Government Education", "Other")); // Global Logistics Inc.
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[18], "Local", "Associate")); // Creative Minds Agency
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[19], "Associate", "Non-Local")); // Eco Home Builders
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[20], "Chamber", "Industrial")); // Tech Innovate
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[21], "Government Education", "Other")); // Summit Adventures
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[22], "Local")); // Fresh Bites Catering
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[23], "Associate", "Chamber")); // Bright Star Media
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[24], "Industrial", "Non-Local")); // SafeGuard Insurance
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[25], "Chamber", "Government Education")); // Prime Fitness
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[26], "Local")); // Golden Harvest Foods
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[27], "Government Education", "Other")); // Apex Automotive
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[28], "Associate", "Local")); // ClearView Analytics
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[29], "Chamber", "Industrial")); // BlueWave Technologies
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[30], "Government Education", "Other")); // Harmony Music School
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[31], "Local")); // Coastal Shipping Lines
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[32], "Chamber")); // Digital Marketing Pro
                        memberMembershipTypes.AddRange(AssignMembershipTypes(members[33], "Government Education")); // Quality Construction Group

                        // Add the many-to-many relationships
                        context.MemberMembershipTypes.AddRange(memberMembershipTypes);
                        context.SaveChanges();
                    }

                    // Seed IndustryMembers
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
                            new MemberIndustry { IndustryID = 2, MemberID = 10 },
                            new MemberIndustry { IndustryID = 2, MemberID = 11 }, // Green Energy Solutions (Multiple Industries)
                            new MemberIndustry { IndustryID = 3, MemberID = 12 }, // Bright Future Education
                            new MemberIndustry { IndustryID = 4, MemberID = 13 }, // Oceanic Travels
                            new MemberIndustry { IndustryID = 5, MemberID = 14 }, // NextGen Robotics
                            new MemberIndustry { IndustryID = 6, MemberID = 15 }, // Urban Fashion Hub
                            new MemberIndustry { IndustryID = 7, MemberID = 16 }, // Sky High Drones
                            new MemberIndustry { IndustryID = 8, MemberID = 17 }, // Pure Water Tech
                            new MemberIndustry { IndustryID = 9, MemberID = 18 }, // Global Logistics Inc.
                            new MemberIndustry { IndustryID = 55, MemberID = 19 }, // Creative Minds Agency
                            new MemberIndustry { IndustryID = 11, MemberID = 20 }, // Eco Home Builders
                            new MemberIndustry { IndustryID = 12, MemberID = 21 }, // Tech Innovate
                            new MemberIndustry { IndustryID = 32, MemberID = 22 }, // Summit Adventures
                            new MemberIndustry { IndustryID = 14, MemberID = 23 }, // Fresh Bites Catering
                            new MemberIndustry { IndustryID = 15, MemberID = 24 }, // Bright Star Media
                            new MemberIndustry { IndustryID = 45, MemberID = 25 }, // SafeGuard Insurance
                            new MemberIndustry { IndustryID = 47, MemberID = 26 }, // Prime Fitness
                            new MemberIndustry { IndustryID = 58, MemberID = 27 }, // Golden Harvest Foods
                            new MemberIndustry { IndustryID = 19, MemberID = 28 }, // Apex Automotive
                            new MemberIndustry { IndustryID = 20, MemberID = 29 }, // ClearView Analytics
                            new MemberIndustry { IndustryID = 21, MemberID = 30 }, // BlueWave Technologies
                            new MemberIndustry { IndustryID = 33, MemberID = 31 }, // Harmony Music School
                            new MemberIndustry { IndustryID = 44, MemberID = 32 }, // Coastal Shipping Lines
                            new MemberIndustry { IndustryID = 56, MemberID = 33 }, // Digital Marketing Pro
                            new MemberIndustry { IndustryID = 67, MemberID = 34 }  // Quality Construction Group
                        );
                        context.SaveChanges();
                    }

                    // Seed Addresses
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
                            },
                            new Address
                            {
                                Street = "123 Maple Ave",
                                City = "Toronto",
                                Province = Province.Ontario,
                                PostalCode = "M5V 3L9"
                            },
                            new Address
                            {
                                Street = "456 Elm St",
                                City = "Vancouver",
                                Province = Province.BritishColumbia,
                                PostalCode = "V6B 1A1"
                            },
                            new Address
                            {
                                Street = "789 Pine Rd",
                                City = "Calgary",
                                Province = Province.Alberta,
                                PostalCode = "T2P 1J9"
                            },
                            new Address
                            {
                                Street = "101 Birch Ln",
                                City = "Montreal",
                                Province = Province.Quebec,
                                PostalCode = "H3Z 2Y7"
                            },
                            new Address
                            {
                                Street = "202 Cedar Blvd",
                                City = "Ottawa",
                                Province = Province.Ontario,
                                PostalCode = "K1P 5E2"
                            },
                            new Address
                            {
                                Street = "303 Spruce Dr",
                                City = "Edmonton",
                                Province = Province.Alberta,
                                PostalCode = "T5J 2Z7"
                            },
                            new Address
                            {
                                Street = "404 Willow Way",
                                City = "Winnipeg",
                                Province = Province.Manitoba,
                                PostalCode = "R3C 4T6"
                            },
                            new Address
                            {
                                Street = "505 Oakwood Cres",
                                City = "Quebec City",
                                Province = Province.Quebec,
                                PostalCode = "G1R 2J7"
                            },
                            new Address
                            {
                                Street = "606 Birchwood Ave",
                                City = "Hamilton",
                                Province = Province.Ontario,
                                PostalCode = "L8P 1A1"
                            },
                            new Address
                            {
                                Street = "707 Pinecrest Rd",
                                City = "Victoria",
                                Province = Province.BritishColumbia,
                                PostalCode = "V8W 1P6"
                            },
                            new Address
                            {
                                Street = "808 Maplewood Dr",
                                City = "Halifax",
                                Province = Province.NovaScotia,
                                PostalCode = "B3H 1P9"
                            },
                            new Address
                            {
                                Street = "909 Elmwood Blvd",
                                City = "Saskatoon",
                                Province = Province.Saskatchewan,
                                PostalCode = "S7K 1J9"
                            },
                            new Address
                            {
                                Street = "1010 Cedarcrest Ln",
                                City = "Regina",
                                Province = Province.Saskatchewan,
                                PostalCode = "S4P 3Y2"
                            },
                            new Address
                            {
                                Street = "1111 Sprucewood Ave",
                                City = "St. John's",
                                Province = Province.NewfoundlandAndLabrador,
                                PostalCode = "A1C 5B9"
                            },
                            new Address
                            {
                                Street = "1212 Willowcrest Rd",
                                City = "Charlottetown",
                                Province = Province.PrinceEdwardIsland,
                                PostalCode = "C1A 4P3"
                            }
                        );
                        context.SaveChanges();
                    }

                    // Seed Contacts
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
                            }, new Contact
                            {
                                FirstName = "Emma",
                                LastName = "Johnson",
                                Email = "emmajohnson@universaltechhub.com",
                                Phone = "7788990011",
                                Linkedin = "https://linkedin.com/in/emmajohnson"
                            },
new Contact
{
    FirstName = "Lucas",
    LastName = "Martinez",
    Email = "lucasmartinez@universaltechhub.com",
    Phone = "9988776655",
    Linkedin = "https://linkedin.com/in/lucasmartinez"
},
new Contact
{
    FirstName = "Ava",
    LastName = "Garcia",
    Email = "avagarcia@universaltechhub.com",
    Phone = "6677889900",
    Linkedin = "https://linkedin.com/in/avagarcia"
},
new Contact
{
    FirstName = "Mason",
    LastName = "Lee",
    Email = "masonlee@universaltechhub.com",
    Phone = "5544332211",
    Linkedin = "https://linkedin.com/in/masonlee"
},
new Contact
{
    FirstName = "Isabella",
    LastName = "Harris",
    Email = "isabellaharris@universaltechhub.com",
    Phone = "1122334455",
    Linkedin = "https://linkedin.com/in/isabellaharris"
},
new Contact
{
    FirstName = "Ethan",
    LastName = "Clark",
    Email = "ethanclark@universaltechhub.com",
    Phone = "9988776655",
    Linkedin = "https://linkedin.com/in/ethanclark"
},
new Contact
{
    FirstName = "Sophia",
    LastName = "Lewis",
    Email = "sophialewis@universaltechhub.com",
    Phone = "6677889900",
    Linkedin = "https://linkedin.com/in/sophialewis"
},
new Contact
{
    FirstName = "James",
    LastName = "Walker",
    Email = "jameswalker@universaltechhub.com",
    Phone = "5544332211",
    Linkedin = "https://linkedin.com/in/jameswalker"
}
                        );
                        context.SaveChanges();
                    }

                    // Seed MemberAddresses
                    if (!context.MemberAddresses.Any())
                    {
                        context.MemberAddresses.AddRange(
                            // Existing MemberAddresses (1-10)
                            new MemberAddress { MemberID = 1, AddressID = 1, AddressType = AddressType.Office },
                            new MemberAddress { MemberID = 2, AddressID = 2, AddressType = AddressType.Billing },
                            new MemberAddress { MemberID = 3, AddressID = 3, AddressType = AddressType.Shipping },
                            new MemberAddress { MemberID = 4, AddressID = 4, AddressType = AddressType.Office },
                            new MemberAddress { MemberID = 5, AddressID = 5, AddressType = AddressType.Billing },
                            new MemberAddress { MemberID = 6, AddressID = 1, AddressType = AddressType.Shipping },
                            new MemberAddress { MemberID = 7, AddressID = 2, AddressType = AddressType.Office },
                            new MemberAddress { MemberID = 8, AddressID = 3, AddressType = AddressType.Billing },
                            new MemberAddress { MemberID = 9, AddressID = 4, AddressType = AddressType.Shipping },
                            new MemberAddress { MemberID = 10, AddressID = 5, AddressType = AddressType.Office },

                            // New MemberAddresses (11-30)
                            new MemberAddress { MemberID = 11, AddressID = 6, AddressType = AddressType.Office }, // Green Energy Solutions
                            new MemberAddress { MemberID = 12, AddressID = 7, AddressType = AddressType.Billing }, // Bright Future Education
                            new MemberAddress { MemberID = 13, AddressID = 8, AddressType = AddressType.Shipping }, // Oceanic Travels
                            new MemberAddress { MemberID = 14, AddressID = 9, AddressType = AddressType.Office }, // NextGen Robotics
                            new MemberAddress { MemberID = 15, AddressID = 10, AddressType = AddressType.Billing }, // Urban Fashion Hub
                            new MemberAddress { MemberID = 16, AddressID = 11, AddressType = AddressType.Shipping }, // Sky High Drones
                            new MemberAddress { MemberID = 17, AddressID = 12, AddressType = AddressType.Office }, // Pure Water Tech
                            new MemberAddress { MemberID = 18, AddressID = 13, AddressType = AddressType.Billing }, // Global Logistics Inc.
                            new MemberAddress { MemberID = 19, AddressID = 14, AddressType = AddressType.Shipping }, // Creative Minds Agency
                            new MemberAddress { MemberID = 20, AddressID = 15, AddressType = AddressType.Office }, // Eco Home Builders
                            new MemberAddress { MemberID = 21, AddressID = 16, AddressType = AddressType.Billing }, // Tech Innovate
                            new MemberAddress { MemberID = 22, AddressID = 17, AddressType = AddressType.Shipping }, // Summit Adventures
                            new MemberAddress { MemberID = 23, AddressID = 18, AddressType = AddressType.Office }, // Fresh Bites Catering
                            new MemberAddress { MemberID = 24, AddressID = 19, AddressType = AddressType.Billing }, // Bright Star Media
                            new MemberAddress { MemberID = 25, AddressID = 20, AddressType = AddressType.Shipping }, // SafeGuard Insurance
                            new MemberAddress { MemberID = 26, AddressID = 19, AddressType = AddressType.Office }, // Prime Fitness
                            new MemberAddress { MemberID = 27, AddressID = 18, AddressType = AddressType.Billing }, // Golden Harvest Foods
                            new MemberAddress { MemberID = 28, AddressID = 2, AddressType = AddressType.Shipping }, // Apex Automotive
                            new MemberAddress { MemberID = 29, AddressID = 4, AddressType = AddressType.Office }, // ClearView Analytics
                            new MemberAddress { MemberID = 30, AddressID = 5, AddressType = AddressType.Billing } // BlueWave Technologies
                        );
                        context.SaveChanges();
                    }
                    // Seed MemberContacts
                    if (!context.MemberContacts.Any())
                    {
                        context.MemberContacts.AddRange(
                            // Existing MemberContacts (1-10)
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
                            new MemberContact { MemberID = 1, ContactID = 10, ContactType = ContactType.General },

                            // New MemberContacts (11-30)
                            new MemberContact { MemberID = 11, ContactID = 11, ContactType = ContactType.VIP }, // Green Energy Solutions
                            new MemberContact { MemberID = 12, ContactID = 12, ContactType = ContactType.General }, // Bright Future Education
                            new MemberContact { MemberID = 13, ContactID = 13, ContactType = ContactType.VIP }, // Oceanic Travels
                            new MemberContact { MemberID = 14, ContactID = 14, ContactType = ContactType.General }, // NextGen Robotics
                            new MemberContact { MemberID = 15, ContactID = 15, ContactType = ContactType.VIP }, // Urban Fashion Hub
                            new MemberContact { MemberID = 16, ContactID = 16, ContactType = ContactType.General }, // Sky High Drones
                            new MemberContact { MemberID = 17, ContactID = 17, ContactType = ContactType.VIP }, // Pure Water Tech
                            new MemberContact { MemberID = 18, ContactID = 18, ContactType = ContactType.General }, // Global Logistics Inc.
                            new MemberContact { MemberID = 19, ContactID = 1, ContactType = ContactType.VIP }, // Creative Minds Agency
                            new MemberContact { MemberID = 20, ContactID = 2, ContactType = ContactType.General }, // Eco Home Builders
                            new MemberContact { MemberID = 21, ContactID = 3, ContactType = ContactType.VIP }, // Tech Innovate
                            new MemberContact { MemberID = 22, ContactID = 4, ContactType = ContactType.General }, // Summit Adventures
                            new MemberContact { MemberID = 23, ContactID = 5, ContactType = ContactType.VIP }, // Fresh Bites Catering
                            new MemberContact { MemberID = 24, ContactID = 14, ContactType = ContactType.General }, // Bright Star Media
                            new MemberContact { MemberID = 25, ContactID = 2, ContactType = ContactType.VIP }, // SafeGuard Insurance
                            new MemberContact { MemberID = 26, ContactID = 6, ContactType = ContactType.General }, // Prime Fitness
                            new MemberContact { MemberID = 27, ContactID = 17, ContactType = ContactType.VIP }, // Golden Harvest Foods
                            new MemberContact { MemberID = 28, ContactID = 18, ContactType = ContactType.General }, // Apex Automotive
                            new MemberContact { MemberID = 29, ContactID = 12, ContactType = ContactType.VIP }, // ClearView Analytics
                            new MemberContact { MemberID = 30, ContactID = 11, ContactType = ContactType.General } // BlueWave Technologies
                        );
                        context.SaveChanges();
                    }
                    // Seed Opportunities
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
                            }, new Opportunity
                            {
                                Title = "E-Commerce Platform Integration",
                                Status = Status.Negotiating,
                                Description = "Negotiating with e-commerce platforms for integration with our solutions.",
                                Priority = PriorityType.High
                            },
new Opportunity
{
    Title = "Cloud Migration Strategy",
    Status = Status.Qualification,
    Description = "Developing a comprehensive strategy for migrating on-premise systems to the cloud.",
    Priority = PriorityType.Urgent
},
new Opportunity
{
    Title = "AI-Powered Customer Support",
    Status = Status.Qualification,
    Description = "Exploring the implementation of AI-driven chatbots for customer support.",
    Priority = PriorityType.Medium
},
new Opportunity
{
    Title = "Supply Chain Optimization",
    Status = Status.Negotiating,
    Description = "Negotiating with partners to optimize supply chain operations using advanced analytics.",
    Priority = PriorityType.High
},
new Opportunity
{
    Title = "Cybersecurity Upgrade",
    Status = Status.ClosedNewMember,
    Description = "Successfully upgraded cybersecurity infrastructure to protect against emerging threats.",
    Priority = PriorityType.Urgent
},
new Opportunity
{
    Title = "Mobile App Development",
    Status = Status.Qualification,
    Description = "Building a cross-platform mobile app to enhance customer engagement.",
    Priority = PriorityType.Medium
},
new Opportunity
{
    Title = "Data Analytics Dashboard",
    Status = Status.ClosedNotInterested,
    Description = "Exploring the development of a real-time data analytics dashboard for business insights.",
    Priority = PriorityType.Low
},
new Opportunity
{
    Title = "Green Energy Partnership",
    Status = Status.Negotiating,
    Description = "Negotiating a partnership with a green energy provider to reduce carbon footprint.",
    Priority = PriorityType.High
},
new Opportunity
{
    Title = "Employee Training Program",
    Status = Status.Qualification,
    Description = "Implementing a company-wide training program to upskill employees.",
    Priority = PriorityType.Medium
},
new Opportunity
{
    Title = "Product Launch: Smart Home Device",
    Status = Status.ClosedNotInterested,
    Description = "The launch of the new smart home device was postponed due to market conditions.",
    Priority = PriorityType.Urgent
}
                        );
                        context.SaveChanges();
                    }

                    // Seed MemberOpportunities
                    if (!context.MemberOpportunities.Any())
                    {
                        context.MemberOpportunities.AddRange(
                            // Existing MemberOpportunities (1-10)
                            new MemberOpportunity { MemberID = 1, OpportunityID = 1 },
                            new MemberOpportunity { MemberID = 2, OpportunityID = 2 },
                            new MemberOpportunity { MemberID = 3, OpportunityID = 3 },
                            new MemberOpportunity { MemberID = 4, OpportunityID = 4 },
                            new MemberOpportunity { MemberID = 5, OpportunityID = 5 },
                            new MemberOpportunity { MemberID = 6, OpportunityID = 6 },
                            new MemberOpportunity { MemberID = 7, OpportunityID = 7 },
                            new MemberOpportunity { MemberID = 8, OpportunityID = 8 },
                            new MemberOpportunity { MemberID = 9, OpportunityID = 9 },
                            new MemberOpportunity { MemberID = 10, OpportunityID = 10 },

                            // New MemberOpportunities (11-30)
                            new MemberOpportunity { MemberID = 11, OpportunityID = 11 }, // Green Energy Solutions
                            new MemberOpportunity { MemberID = 12, OpportunityID = 12 }, // Bright Future Education
                            new MemberOpportunity { MemberID = 13, OpportunityID = 13 }, // Oceanic Travels
                            new MemberOpportunity { MemberID = 14, OpportunityID = 14 }, // NextGen Robotics
                            new MemberOpportunity { MemberID = 15, OpportunityID = 15 }, // Urban Fashion Hub
                            new MemberOpportunity { MemberID = 16, OpportunityID = 16 }, // Sky High Drones
                            new MemberOpportunity { MemberID = 17, OpportunityID = 17 }, // Pure Water Tech
                            new MemberOpportunity { MemberID = 18, OpportunityID = 18 }, // Global Logistics Inc.
                            new MemberOpportunity { MemberID = 19, OpportunityID = 19 }, // Creative Minds Agency
                            new MemberOpportunity { MemberID = 20, OpportunityID = 20 }, // Eco Home Builders
                            new MemberOpportunity { MemberID = 21, OpportunityID = 11 }, // Tech Innovate (reuses OpportunityID 11)
                            new MemberOpportunity { MemberID = 22, OpportunityID = 12 }, // Summit Adventures (reuses OpportunityID 12)
                            new MemberOpportunity { MemberID = 23, OpportunityID = 13 }, // Fresh Bites Catering (reuses OpportunityID 13)
                            new MemberOpportunity { MemberID = 24, OpportunityID = 14 }, // Bright Star Media (reuses OpportunityID 14)
                            new MemberOpportunity { MemberID = 25, OpportunityID = 15 }, // SafeGuard Insurance (reuses OpportunityID 15)
                            new MemberOpportunity { MemberID = 26, OpportunityID = 16 }, // Prime Fitness (reuses OpportunityID 16)
                            new MemberOpportunity { MemberID = 27, OpportunityID = 17 }, // Golden Harvest Foods (reuses OpportunityID 17)
                            new MemberOpportunity { MemberID = 28, OpportunityID = 18 }, // Apex Automotive (reuses OpportunityID 18)
                            new MemberOpportunity { MemberID = 29, OpportunityID = 19 }, // ClearView Analytics (reuses OpportunityID 19)
                            new MemberOpportunity { MemberID = 30, OpportunityID = 20 }  // BlueWave Technologies (reuses OpportunityID 20)
                        );
                        context.SaveChanges();
                    }
                    // **Delete Existing Interaction Data**
                    if (context.InteractionMembers.Any() || context.Interactions.Any())
                    {
                        // Remove all InteractionMembers first to avoid foreign key constraints
                        context.InteractionMembers.RemoveRange(context.InteractionMembers);
                        context.SaveChanges();

                        // Then remove all Interactions
                        context.Interactions.RemoveRange(context.Interactions);
                        context.SaveChanges();
                    }

                    // Seed Interactions
                    if (!context.Interactions.Any())
                    {
                        // Fetch existing contacts to map names to IDs
                        var contacts = context.Contacts.ToList();
                        var contactDictionary = contacts
                        .GroupBy(c => $"{c.FirstName} {c.LastName}")
                        .ToDictionary(g => g.Key, g => g.First().ID);


                        var interactions = new List<Interaction>
                        {
                            new Interaction
                            {
                                InteractionDetails = "Initial Consultation Call",
                                Date = DateTime.Now.AddDays(-30),
                                ContactId = contactDictionary.ContainsKey("John Doe") ? contactDictionary["John Doe"] : (int?)null
                            },
                            new Interaction
                            {
                                InteractionDetails = "Technical Support Discussion",
                                Date = DateTime.Now.AddDays(-25),
                                ContactId = contactDictionary.ContainsKey("Jane Smith") ? contactDictionary["Jane Smith"] : (int?)null
                            },
                            new Interaction
                            {
                                InteractionDetails = "Annual Membership Review",
                                Date = DateTime.Now.AddDays(-20),
                                ContactId = contactDictionary.ContainsKey("Albert Green") ? contactDictionary["Albert Green"] : (int?)null
                            },
                            new Interaction
                            {
                                InteractionDetails = "New Product Launch Briefing",
                                Date = DateTime.Now.AddDays(-15),
                                ContactId = contactDictionary.ContainsKey("Maria Johnson") ? contactDictionary["Maria Johnson"] : (int?)null
                            },
                            new Interaction
                            {
                                InteractionDetails = "Marketing Strategy Meeting",
                                Date = DateTime.Now.AddDays(-10),
                                ContactId = contactDictionary.ContainsKey("Emily Wright") ? contactDictionary["Emily Wright"] : (int?)null
                            },
                            new Interaction
                            {
                                InteractionDetails = "Partnership Opportunity Discussion",
                                Date = DateTime.Now.AddDays(-5),
                                ContactId = contactDictionary.ContainsKey("James Carter") ? contactDictionary["James Carter"] : (int?)null
                            },
                            new Interaction
                            {
                                InteractionDetails = "Subscription Feedback Session",
                                Date = DateTime.Now.AddDays(-3),
                                ContactId = contactDictionary.ContainsKey("Daniel Lee") ? contactDictionary["Daniel Lee"] : (int?)null
                            },
                            new Interaction
                            {
                                InteractionDetails = "Technical Integration Workshop",
                                Date = DateTime.Now.AddDays(-2),
                                ContactId = contactDictionary.ContainsKey("Linda Miller") ? contactDictionary["Linda Miller"] : (int?)null
                            },
                            new Interaction
                            {
                                InteractionDetails = "Customer Success Call",
                                Date = DateTime.Now.AddDays(-1),
                                ContactId = contactDictionary.ContainsKey("Nina Roberts") ? contactDictionary["Nina Roberts"] : (int?)null
                            },
                            new Interaction
                            {
                                InteractionDetails = "Renewal Discussion",
                                Date = DateTime.Now,
                                ContactId = contactDictionary.ContainsKey("Oliver Thomas") ? contactDictionary["Oliver Thomas"] : (int?)null
                            },new Interaction
{
    InteractionDetails = "Product Demo",
    Date = DateTime.Now.AddDays(-5),
    ContactId = contactDictionary.ContainsKey("Emma Johnson") ? contactDictionary["Emma Johnson"] : (int?)null
},
new Interaction
{
    InteractionDetails = "Contract Negotiation",
    Date = DateTime.Now.AddDays(-3),
    ContactId = contactDictionary.ContainsKey("Lucas Martinez") ? contactDictionary["Lucas Martinez"] : (int?)null
},
new Interaction
{
    InteractionDetails = "Feedback Session",
    Date = DateTime.Now.AddDays(-2),
    ContactId = contactDictionary.ContainsKey("Ava Garcia") ? contactDictionary["Ava Garcia"] : (int?)null
},
new Interaction
{
    InteractionDetails = "Onboarding Call",
    Date = DateTime.Now.AddDays(-7),
    ContactId = contactDictionary.ContainsKey("Mason Lee") ? contactDictionary["Mason Lee"] : (int?)null
},
new Interaction
{
    InteractionDetails = "Technical Support",
    Date = DateTime.Now.AddDays(-4),
    ContactId = contactDictionary.ContainsKey("Isabella Harris") ? contactDictionary["Isabella Harris"] : (int?)null
},
new Interaction
{
    InteractionDetails = "Quarterly Review",
    Date = DateTime.Now.AddDays(-10),
    ContactId = contactDictionary.ContainsKey("Ethan Clark") ? contactDictionary["Ethan Clark"] : (int?)null
},
new Interaction
{
    InteractionDetails = "Training Session",
    Date = DateTime.Now.AddDays(-6),
    ContactId = contactDictionary.ContainsKey("Sophia Lewis") ? contactDictionary["Sophia Lewis"] : (int?)null
},
new Interaction
{
    InteractionDetails = "Project Kickoff",
    Date = DateTime.Now.AddDays(-8),
    ContactId = contactDictionary.ContainsKey("James Walker") ? contactDictionary["James Walker"] : (int?)null
},
new Interaction
{
    InteractionDetails = "Upsell Discussion",
    Date = DateTime.Now.AddDays(-9),
    ContactId = contactDictionary.ContainsKey("Oliver Thomas") ? contactDictionary["Oliver Thomas"] : (int?)null
},
new Interaction
{
    InteractionDetails = "Issue Resolution",
    Date = DateTime.Now.AddDays(-1),
    ContactId = contactDictionary.ContainsKey("Nina Roberts") ? contactDictionary["Nina Roberts"] : (int?)null
}
                        };

                        context.Interactions.AddRange(interactions);
                        context.SaveChanges();

                        // Assign InteractionMembers
                        var interactionMembers = new List<InteractionMember>
                        {
                            new InteractionMember { MemberID = 1, InteractionID = interactions[0].Id },
                            new InteractionMember { MemberID = 2, InteractionID = interactions[1].Id },
                            new InteractionMember { MemberID = 3, InteractionID = interactions[2].Id },
                            new InteractionMember { MemberID = 4, InteractionID = interactions[3].Id },
                            new InteractionMember { MemberID = 5, InteractionID = interactions[4].Id },
                            new InteractionMember { MemberID = 6, InteractionID = interactions[5].Id },
                            new InteractionMember { MemberID = 7, InteractionID = interactions[6].Id },
                            new InteractionMember { MemberID = 8, InteractionID = interactions[7].Id },
                            new InteractionMember { MemberID = 9, InteractionID = interactions[8].Id },
                            new InteractionMember { MemberID = 10, InteractionID = interactions[9].Id },
                             // New InteractionMembers (11-30)
        new InteractionMember { MemberID = 11, InteractionID = interactions[10].Id }, // Green Energy Solutions
        new InteractionMember { MemberID = 12, InteractionID = interactions[11].Id }, // Bright Future Education
        new InteractionMember { MemberID = 13, InteractionID = interactions[12].Id }, // Oceanic Travels
        new InteractionMember { MemberID = 14, InteractionID = interactions[13].Id }, // NextGen Robotics
        new InteractionMember { MemberID = 15, InteractionID = interactions[14].Id }, // Urban Fashion Hub
        new InteractionMember { MemberID = 16, InteractionID = interactions[15].Id }, // Sky High Drones
        new InteractionMember { MemberID = 17, InteractionID = interactions[16].Id }, // Pure Water Tech
        new InteractionMember { MemberID = 18, InteractionID = interactions[17].Id }, // Global Logistics Inc.
        new InteractionMember { MemberID = 19, InteractionID = interactions[18].Id }, // Creative Minds Agency
        new InteractionMember { MemberID = 20, InteractionID = interactions[19].Id }, // Eco Home Builders
        new InteractionMember { MemberID = 21, InteractionID = interactions[10].Id }, // Tech Innovate (reuses InteractionID 10)
        new InteractionMember { MemberID = 22, InteractionID = interactions[11].Id }, // Summit Adventures (reuses InteractionID 11)
        new InteractionMember { MemberID = 23, InteractionID = interactions[12].Id }, // Fresh Bites Catering (reuses InteractionID 12)
        new InteractionMember { MemberID = 24, InteractionID = interactions[13].Id }, // Bright Star Media (reuses InteractionID 13)
        new InteractionMember { MemberID = 25, InteractionID = interactions[14].Id }, // SafeGuard Insurance (reuses InteractionID 14)
        new InteractionMember { MemberID = 26, InteractionID = interactions[15].Id }, // Prime Fitness (reuses InteractionID 15)
        new InteractionMember { MemberID = 27, InteractionID = interactions[16].Id }, // Golden Harvest Foods (reuses InteractionID 16)
        new InteractionMember { MemberID = 28, InteractionID = interactions[17].Id }, // Apex Automotive (reuses InteractionID 17)
        new InteractionMember { MemberID = 29, InteractionID = interactions[18].Id }, // ClearView Analytics (reuses InteractionID 18)
        new InteractionMember { MemberID = 30, InteractionID = interactions[19].Id }  // BlueWave Technologies (reuses InteractionID 19)
                        };

                        context.InteractionMembers.AddRange(interactionMembers);
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception details
                    Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
        }
    }
}
