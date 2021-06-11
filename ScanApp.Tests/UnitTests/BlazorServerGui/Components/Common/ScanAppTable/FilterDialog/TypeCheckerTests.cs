using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScanApp.Components.Common.ScanAppTable.FilterDialog;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.FilterDialog
{
    public class TypeCheckerTests
    {
        [Fact]
        public void Will_correctly_check_for_integer_type()
        {
            Type typeTest = typeof(uint);
            Assert.True(TypeChecker.IsInteger(typeTest));
        }

        [Fact]
        public void Will_correctly_check_for_integer_type_when_given_object()
        {
            object objTest = new object();
            objTest = 5;
            Assert.True(objTest.GetType() == typeof(int));
            Assert.True(TypeChecker.IsInteger(objTest));
        }

        [Fact]
        public void Will_correctly_check_for_decimal_type()
        {
            Type typeTest = typeof(decimal);
            Assert.True(TypeChecker.IsDecimal(typeTest));
        }

        [Fact]
        public void Will_correctly_check_for_decimal_type_when_given_object()
        {
            object objTest = new object();
            objTest = 5.555;
            Assert.True(objTest.GetType() == typeof(double));
            Assert.True(TypeChecker.IsDecimal(objTest));
        }

        [Fact]
        public void Will_correctly_check_for_nullable_integer_type()
        {
            Type typeTest = typeof(int?);
            Assert.True(TypeChecker.IsInteger(typeTest));
        }

        [Fact]
        public void Will_correctly_check_for_datetime_type()
        {
            Type typeTest = typeof(DateTime);
            Assert.True(TypeChecker.IsDateTime(typeTest));

            typeTest = typeof(int);
            Assert.False(TypeChecker.IsDateTime(typeTest));
        }

        [Fact]
        public void Will_correctly_check_for_datetime_type_when_given_object()
        {
            object objTest = new object();
            objTest = new DateTime(1,1,1);
            Assert.True(objTest.GetType() == typeof(DateTime));
            Assert.True(TypeChecker.IsDateTime(objTest));
        }

        [Fact]
        public void Will_return_false_if_given_null_as_argument()
        {
            Assert.False(TypeChecker.IsInteger(null));
            Assert.False(TypeChecker.IsDecimal(null));
            Assert.False(TypeChecker.IsDateTime(null));
        }
    }
}
