using System;
using System.Linq.Expressions;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Table.Utilities
{
    public class InBetweenFilterFixtureValidFromTo : TheoryData<Expression<Func<ColumnConfigFixtures.SubClass, object>>, dynamic, dynamic>
    {
        public InBetweenFilterFixtureValidFromTo()
        {
            Add(x => x.DateTime, DateTime.Today - TimeSpan.FromDays(1), DateTime.Now);

            DateTime? from = DateTime.Today - TimeSpan.FromDays(1);
            DateTime? to = DateTime.Now;
            Add(x => x.NullableDateTime, from, to);

            Add(x => x.TimeSpan, TimeSpan.MinValue, TimeSpan.MaxValue);

            TimeSpan? fromTime = TimeSpan.MinValue;
            TimeSpan? toTime = TimeSpan.MaxValue;
            Add(x => x.NullableTimeSpan, fromTime, toTime);

            Add(x => x.DoubleField, 10.1, 51.1);

            Add(x => x.IntField, 1, 10);

            int? fromInt = 1;
            int? toInt = 10;
            Add(x => x.NullableIntField, fromInt, toInt);

            Add(x => x.NullableIntField, 10, toInt);
            Add(x => x.NullableTimeSpan, TimeSpan.MinValue, toTime);
            Add(x => x.NullableDateTime, DateTime.Today - TimeSpan.FromDays(1), to);

            Add(x => x.IntField, 10, toInt);
            Add(x => x.TimeSpan, TimeSpan.MinValue, toTime);
            Add(x => x.DateTime, DateTime.Today - TimeSpan.FromDays(1), to);
        }
    }

    public class InBetweenFilterFixtureValidFromIsNullOrToIsNull : TheoryData<Expression<Func<ColumnConfigFixtures.SubClass, object>>, dynamic, dynamic>
    {
        public InBetweenFilterFixtureValidFromIsNullOrToIsNull()
        {
            Add(x => x.DateTime, DateTime.Today - TimeSpan.FromDays(1), DateTime.Now);

            DateTime? from = null;
            DateTime? to = DateTime.Now;
            Add(x => x.NullableDateTime, from, to);

            TimeSpan? fromTime = null;
            TimeSpan? toTime = TimeSpan.MaxValue;
            Add(x => x.NullableTimeSpan, fromTime, toTime);

            int? fromInt = null;
            int? toInt = 10;
            Add(x => x.NullableIntField, fromInt, toInt);

            from = DateTime.Today - TimeSpan.FromDays(1);
            to = null;
            Add(x => x.NullableDateTime, from, to);

            fromTime = TimeSpan.MinValue;
            toTime = null;
            Add(x => x.NullableTimeSpan, fromTime, toTime);

            fromInt = 1;
            toInt = null;
            Add(x => x.NullableIntField, fromInt, toInt);
        }
    }
}