﻿using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VBStatusReport;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VBStatusReport;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.VBStatusReport
{
    public class VBStatusReportServiceTest
    {
        private const string ENTITY = "VBStatusReprt";

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private FinanceDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<FinanceDbContext> optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            FinanceDbContext dbContext = new FinanceDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private VBStatusReportDataUtil _dataUtil(VBStatusReportService service)
        {
            return new VBStatusReportDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });


            return serviceProvider;
        }


        [Fact]
        public async Task Should_Success_GetReport()
        {
            VBStatusReportService service = new VBStatusReportService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestDataById();
            var Response = service.GetReport(data.UnitId, data.Id, true, data.Date.AddDays(-1), data.Date.AddDays(1), data.Date.AddDays(-1), data.Date.AddDays(1), 7);
            Assert.NotNull(Response);

            Response = service.GetReport(data.UnitId, data.Id, true, data.Date.AddDays(-1), data.Date.AddDays(1), data.Date.AddDays(-1), data.Date.AddDays(1), 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_GenerateExcel()
        {
            VBStatusReportService service = new VBStatusReportService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestDataById();
            var Response = service.GenerateExcel(data.UnitId, data.Id, true, data.Date.AddDays(-1), data.Date.AddDays(1), data.Date.AddDays(-1), data.Date.AddDays(1), 7);
            Assert.NotNull(Response);

            Response = service.GenerateExcel(data.UnitId, 0, null, null, null, null, null, 7);
            Assert.NotNull(Response);
        }
    }
}
