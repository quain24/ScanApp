using FluentValidation;
using Moq;
using ScanApp.Common;
using System;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.Options
{
    public static class ColumnValidatorFixtures
    {
        public static Mock<FluentValidationWrapper<T>> CreateDummyFluentValidationWrapperMock<T>()
        {
            return new(new Action<IRuleBuilderInitial<T, T>>(x => x.Cascade(CascadeMode.Continue)), null);
        }
    }
}