﻿using Com.Danliris.Service.Packing.Inventory.Application.ToBeRefactored.GarmentShipping.GarmentShippingPackingList;
using Com.Danliris.Service.Packing.Inventory.Application.ToBeRefactored.Utilities;
using Com.Danliris.Service.Packing.Inventory.Infrastructure.IdentityProvider;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Packing.Inventory.Test.Controllers.GarmentShipping.GarmentShippingPackingList
{
    public class GarmentShippingPackingListControllerSetPostTest : GarmentShippingPackingListControllerTest
    {
        [Fact]
        public async Task Set_Post_Success()
        {
            var serviceMock = new Mock<IGarmentShippingPackingListService>();
            serviceMock
                .Setup(s => s.SetPost(It.IsAny<List<int>>()))
                .Verifiable();
            var service = serviceMock.Object;

            var validateServiceMock = new Mock<IValidateService>();
            var validateService = validateServiceMock.Object;

            var identityProviderMock = new Mock<IIdentityProvider>();
            var identityProvider = identityProviderMock.Object;

            var controller = GetController(service, identityProvider, validateService);

            var response = await controller.SetPost(It.IsAny<List<int>>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Set_Post_Exception_InternalServerError()
        {
            var serviceMock = new Mock<IGarmentShippingPackingListService>();
            serviceMock
                .Setup(s => s.SetPost(It.IsAny<List<int>>()))
                .ThrowsAsync(new Exception());
            var service = serviceMock.Object;

            var validateServiceMock = new Mock<IValidateService>();
            var validateService = validateServiceMock.Object;

            var identityProviderMock = new Mock<IIdentityProvider>();
            var identityProvider = identityProviderMock.Object;

            var controller = GetController(service, identityProvider, validateService);

            var response = await controller.SetPost(It.IsAny<List<int>>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
