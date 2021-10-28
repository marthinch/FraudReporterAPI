using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit.Sdk;
using FraudReporterAPI.Services;
using FraudReporterAPI.Paginations;
using FraudReporterAPI.Data;
using Microsoft.EntityFrameworkCore;
using FraudReporterAPI.Models;
using AutoMapper;
using FraudReporterAPI.Mappers;
using FraudReporterAPI.DTOs;
using FraudReporterAPI.Enums;
using System;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace FraudReporterAPI.Controllers.Tests
{
    [TestClass()]
    public class FraudsControllerTests
    {
        private DbContextOptionsBuilder<FraudReporterAPIContext> dbContextOptionsBuilder;
        private readonly IMapper mapper;
        private readonly IDataProtector protector;

        public FraudsControllerTests()
        {
            // Setup data protector
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();
            var services = serviceCollection.BuildServiceProvider();
            protector = services.GetDataProtector("Secure");

            // Setup db context builder
            dbContextOptionsBuilder = new DbContextOptionsBuilder<FraudReporterAPIContext>().UseInMemoryDatabase("FraudReporter")
                                                                                            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            // Setup automapper
            MapperConfiguration mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            this.mapper = mapper;
        }

        [TestInitialize]
        public void Initialize()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                try
                {
                    for (int i = 0; i < 40; i++)
                    {
                        if (i < 10)
                        {
                            // Not reported
                            context.Fraud.Add(new Fraud { Id = i + 1, Category = 0, Phone = "xxx", Provider = "xxx", Message = "xxx", Status = 0 });
                        }
                        else if (i >= 10 && i < 20)
                        {
                            // Pending reported
                            context.Fraud.Add(new Fraud { Id = i + 1, Category = 0, Phone = "xxx", Provider = "xxx", Message = "xxx", Status = 1 });
                        }
                        else if (i >= 20 && i < 30)
                        {
                            // Reported
                            context.Fraud.Add(new Fraud { Id = i + 1, Category = 0, Phone = "xxx", Provider = "xxx", Message = "xxx", Status = 2 });
                        }
                        else if (i >= 30 && i < 40)
                        {
                            // Cancel
                            context.Fraud.Add(new Fraud { Id = i + 1, Category = 0, Phone = "xxx", Provider = "xxx", Message = "xxx", Status = 3 });
                        }
                    }

                    context.SaveChanges();
                }
                catch (Exception execption)
                {
                    // Do nothing here
                }
            }
        }

        [TestCleanup]
        public void Clean()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                context.RemoveRange();
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void List_Fraud_Pagination_Should_Return_Value()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var items = fraudService.GetFrauds(new FraudPagination { Index = 0, Item = 10 });

                // Assert
                Assert.IsNotNull(items);
            }
        }

        [TestMethod]
        public void List_Fraud_Pagination_Should_Count_Zero()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var items = fraudService.GetFrauds(new FraudPagination { Index = 10, Item = 10 });

                // Assert
                Assert.AreEqual(0, items.Count);
            }
        }

        [TestMethod]
        public void Detail_Fraud_Should_Return_Value()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var item = fraudService.GetFraudDetail(1);

                // Assert
                Assert.IsNotNull(item);
            }
        }

        [TestMethod]
        public void Detail_Fraud_Should_Return_Null()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var item = fraudService.GetFraudDetail(100);

                // Assert
                Assert.IsNull(item);
            }
        }

        [TestMethod]
        public void Save_Fraud()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Arrange 
                var newFraud = new FraudDTO { Category = 0, Phone = "xxx", Provider = "xxx", Message = "xxx", Status = 0 };

                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.SaveFraud(newFraud);

                // Assert
                Assert.AreEqual(true, result);
            }
        }

        [TestMethod]
        public void Update_Fraud()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Arrange
                var updateFraud = new FraudDTO { Category = 0, Phone = "yyy", Provider = "yyy", Message = "update xxx", Status = 0 };

                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.UpdateFraud(1, updateFraud);

                // Assert
                Assert.AreEqual(true, result);
            }
        }

        [TestMethod]
        public void Delete_Fraud()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.DeleteFraud(2);

                // Assert
                Assert.AreEqual(true, result);
            }
        }

        [TestMethod]
        public void Update_FraudStatus_From_NotReported_To_PendingReported()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.UpdateFraudStatus(3, FraudStatus.PendingReported);

                // Assert
                Assert.AreEqual(true, result);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Status should be updated to pending reported")]
        public void Update_FraudStatus_From_NotReported_To_Reported()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.UpdateFraudStatus(4, FraudStatus.Reported);

                // Assert
                // The result should throw exception based on the test method's attribute 
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Status should be updated to reported")]
        public void Update_FraudStatus_From_NotReported_To_Cancel()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.UpdateFraudStatus(5, FraudStatus.Cancel);

                // Assert
                // The result should throw exception based on the test method's attribute 
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Unable to update status of cancelled report")]
        public void Update_FraudStatus_From_Cancel_To_NotReported()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.UpdateFraudStatus(40, FraudStatus.NotReported);

                // Assert
                // The result should throw exception based on the test method's attribute 
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Unable to update status of cancelled report")]
        public void Update_FraudStatus_From_Cancel_To_PendingReported()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.UpdateFraudStatus(40, FraudStatus.PendingReported);

                // Assert
                // The result should throw exception based on the test method's attribute 
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Unable to update status of cancelled report")]
        public void Update_FraudStatus_From_Cancel_To_Reported()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.UpdateFraudStatus(40, FraudStatus.Reported);

                // Assert
                // The result should throw exception based on the test method's attribute 
            }
        }

        [TestMethod]
        public void Update_FraudStatus_From_PendingReported_To_Reported()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.UpdateFraudStatus(11, FraudStatus.Reported);

                // Assert
                Assert.AreEqual(true, result);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Status should not be updated to not reported")]
        public void Update_FraudStatus_From_PendingReported_To_NotReported()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.UpdateFraudStatus(11, FraudStatus.NotReported);

                // Assert
                // The result should throw exception based on the test method's attribute 
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Status should not be updated to cancel")]
        public void Update_FraudStatus_From_PendingReported_To_Cancel()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.UpdateFraudStatus(12, FraudStatus.Cancel);

                // Assert
                // The result should throw exception based on the test method's attribute 
            }
        }

        [TestMethod]
        public void Cancel_Report()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.CancelReport(30, FraudStatus.Cancel);

                // Assert
                Assert.AreEqual(true, result);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Please check status for cancelling report")]
        public void Cancel_Report_From_Cancel_To_NotReported()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.CancelReport(31, FraudStatus.NotReported);

                // Assert
                // The result should throw exception based on the test method's attribute 
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Please check status for cancelling report")]
        public void Cancel_Report_From_Cancel_To_PendingReported()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.CancelReport(31, FraudStatus.PendingReported);

                // Assert
                // The result should throw exception based on the test method's attribute 
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Please check status for cancelling report")]
        public void Cancel_Report_From_Cancel_To_Reported()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.CancelReport(31, FraudStatus.Reported);

                // Assert
                // The result should throw exception based on the test method's attribute 
            }
        }

        [TestMethod]
        public void Cancel_CancelledReport()
        {
            using (var context = new FraudReporterAPIContext(dbContextOptionsBuilder.Options))
            {
                // Act
                var fraudService = new FraudService(context, mapper, protector);
                var result = fraudService.CancelReport(31, FraudStatus.Cancel);

                // Assert
                Assert.AreEqual(true, result);
            }
        }
    }
}