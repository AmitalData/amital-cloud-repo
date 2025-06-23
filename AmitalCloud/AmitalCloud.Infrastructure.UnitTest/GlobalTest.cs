using System;
using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.UnitTest.BaseClasses;
using Xunit;
using Moq;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using Moq.Protected;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Infrastructure.UnitTest
{
	[Collection("Global collection")]
	public class GlobalTest
	{
		private readonly GlobalTestFixture _fixture;
		private int tenantId = 1;
		private string userId = "user";

		public GlobalTest(GlobalTestFixture fixture)
		{
			_fixture = fixture;
		}

		[Fact]
		public void GetTenantStatusPM_ShouldSetSuspendDaysLeft_WhenPaymentFailureWithFutureDate()
		{
			var tenantManagementPM = new TenantManagementPM
			{
				PaymentFailure = true,
				SuspendDate = DateTime.Now.AddDays(3),
				IsTrial = false,
				IsRecurring = true
			};

			var userPM = new UserPM();  

			var service = CreateService(tenantManagementPM, userPM);

			var result = service.GetTenantStatusPM(tenantId, userId);

			Assert.Equal(3, result.SuspendDaysLeft);
			Assert.False(result.DoBlocking);
		}

		[Fact]
		public void GetTenantStatusPM_ShouldBlock_WhenPaymentFailureWithPastDate()
		{
			var tenantManagementPM = new TenantManagementPM
			{
				PaymentFailure = true,
				SuspendDate = DateTime.Now.AddDays(-1),
				IsTrial = false,
				IsRecurring = true
			};

			var userPM = new UserPM();

			var service = CreateService(tenantManagementPM, userPM);

			var result = service.GetTenantStatusPM(tenantId, userId);

			Assert.True(result.DoBlocking);
			Assert.Equal(BlockingType.Suspend, result.BlockType);
		}
		[Fact]
		public void GetTenantStatusPM_ShouldSetTrialDaysLeft_WhenTrialWithFutureDate()
		{
			var tenantManagementPM = new TenantManagementPM
			{
				IsTrial = true,
				TrialEndDate = DateTime.Now.AddDays(7)
			};

			var userPM = new UserPM();

			var service = CreateService(tenantManagementPM, userPM);

			var result = service.GetTenantStatusPM(tenantId, userId);

			Assert.Equal(7, result.TrialDaysLeft);
			Assert.False(result.DoBlocking);
		}
		[Fact]
		public void GetTenantStatusPM_ShouldBlock_WhenTrialWithPastDate()
		{
			var tenantManagementPM = new TenantManagementPM
			{
				IsTrial = true,
				TrialEndDate = DateTime.Now.AddDays(-2)
			};

			var userPM = new UserPM();

			var service = CreateService(tenantManagementPM, userPM);

			var result = service.GetTenantStatusPM(tenantId, userId);

			Assert.True(result.DoBlocking);
			Assert.Equal(BlockingType.Company, result.BlockType);
		}
		[Fact]
		public void GetTenantStatusPM_ShouldSetPaidDaysLeft_WhenNotRecurringWithFutureDate()
		{
			var tenantManagementPM = new TenantManagementPM
			{
				IsTrial = false,
				IsRecurring = false,
				PaidUntilDate = DateTime.Now.AddDays(4)
			};

			var userPM = new UserPM();

			var service = CreateService(tenantManagementPM, userPM);

			var result = service.GetTenantStatusPM(tenantId, userId);

			Assert.Equal(4, result.PaidDaysLeft);
			Assert.False(result.DoBlocking);
		}
		[Fact]
		public void GetTenantStatusPM_ShouldBlock_WhenNotRecurringWithPastDate()
		{
			var tenantManagementPM = new TenantManagementPM
			{
				IsTrial = false,
				IsRecurring = false,
				PaidUntilDate = DateTime.Now.AddDays(-3)
			};

			var userPM = new UserPM();

			var service = CreateService(tenantManagementPM, userPM);

			var result = service.GetTenantStatusPM(tenantId, userId);

			Assert.True(result.DoBlocking);
			Assert.Equal(BlockingType.Company, result.BlockType);
		}
		[Fact]
		public void GetTenantStatusPM_ShouldSetExpirationDaysLeft_WhenUserHasFutureExpiration()
		{
			var tenantManagementPM = new TenantManagementPM();

			var userPM = new UserPM
			{
				ExpirationDate = DateTime.Now.AddDays(6)
			};

			var service = CreateService(tenantManagementPM, userPM);

			var result = service.GetTenantStatusPM(tenantId, userId);

			Assert.Equal(6, result.ExpirationDaysLeft);
			Assert.Equal(userPM.ExpirationDate, result.ExpirationDate);
			Assert.False(result.DoBlocking);
		}

		[Fact]
		public void GetTenantStatusPM_ShouldBlock_WhenUserHasPastExpiration()
		{
			var tenantManagementPM = new TenantManagementPM();

			var userPM = new UserPM
			{
				ExpirationDate = DateTime.Now.AddDays(-1)
			};

			var service = CreateService(tenantManagementPM, userPM);

			var result = service.GetTenantStatusPM(tenantId, userId);

			Assert.True(result.DoBlocking);
			Assert.Equal(BlockingType.User, result.BlockType);
		}
		[Fact]
		public void GetTenantStatusPM_ShouldNotChange_WhenUserHasNoExpiration()
		{
			var tenantManagementPM = new TenantManagementPM();

			var userPM = new UserPM
			{
				ExpirationDate = null
			};

			var service = CreateService(tenantManagementPM, userPM);

			var result = service.GetTenantStatusPM(tenantId, userId);

			Assert.Null(result.ExpirationDate);
			Assert.Null(result.ExpirationDaysLeft);
			Assert.False(result.DoBlocking);
		}




		private TenantManagementQueryService CreateService(TenantManagementPM tenantManagementPM, UserPM userPM)
		{
			var tenantManagementQueryMock = new Mock<IBaseQueryService<TenantManagementPM, TenantManagement, int>>();
			tenantManagementQueryMock.Setup(r => r.GetSingle(It.IsAny<int>(), true, true)).Returns(tenantManagementPM);

			var userQueryMock = new Mock<IBaseQueryService<UserPM, User, string>>();
			userQueryMock.Setup(r => r.GetSingle(It.IsAny<string>(), true, true)).Returns(userPM);

			return new TenantManagementQueryService(tenantManagementQueryMock.Object, userQueryMock.Object);
		}


	}
}
