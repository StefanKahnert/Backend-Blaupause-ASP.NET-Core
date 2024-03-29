﻿
using Backend_Blaupause.Helper;
using NUnit.Framework;
using System;

namespace Schulungstracker.backend.Test.AttributesTests
{
    class ScheduleAttributeTest
    {

        [Test]
        public void TestScheduleAttribute()
        {
            ScheduleAttribute attr = new ScheduleAttribute(DayOfWeek.Sunday, 10, 30, 2);

            Assert.AreEqual(DayOfWeek.Sunday, attr.day);
            Assert.AreEqual(10, attr.hour);
            Assert.AreEqual(30, attr.min);
            Assert.AreEqual(2, attr.interval);
        }

        [Test]
        public void TestScheduleDay()
        {
            ScheduleDayAttribute attr = new ScheduleDayAttribute(DayOfWeek.Sunday, 10, 30, 2);

            Assert.AreEqual(DayOfWeek.Sunday, attr.day);
            Assert.AreEqual(10, attr.hour);
            Assert.AreEqual(30, attr.min);
            Assert.AreEqual(2, attr.interval);

            Assert.AreEqual(Scheduler.DAYS, attr.intervalType);
        }

        [Test]
        public void TestScheduleHour()
        {
            ScheduleHourAttribute attr = new ScheduleHourAttribute(DayOfWeek.Sunday, 10, 30, 2);

            Assert.AreEqual(DayOfWeek.Sunday, attr.day);
            Assert.AreEqual(10, attr.hour);
            Assert.AreEqual(30, attr.min);
            Assert.AreEqual(2, attr.interval);

            Assert.AreEqual(Scheduler.HOURS, attr.intervalType);
        }


        [Test]
        public void TestScheduleMinute()
        {
            ScheduleMinuteAttribute attr = new ScheduleMinuteAttribute(DayOfWeek.Sunday, 10, 30, 2);

            Assert.AreEqual(DayOfWeek.Sunday, attr.day);
            Assert.AreEqual(10, attr.hour);
            Assert.AreEqual(30, attr.min);
            Assert.AreEqual(2, attr.interval);

            Assert.AreEqual(Scheduler.MINUTES, attr.intervalType);
        }

        [Test]
        public void TestScheduleSecond()
        {
            ScheduleSecondAttribute attr = new ScheduleSecondAttribute(DayOfWeek.Sunday, 10, 30, 2);

            Assert.AreEqual(DayOfWeek.Sunday, attr.day);
            Assert.AreEqual(10, attr.hour);
            Assert.AreEqual(30, attr.min);
            Assert.AreEqual(2, attr.interval);

            Assert.AreEqual(Scheduler.SECONDS, attr.intervalType);
        }
    }
}
