using System;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using Xunit;

namespace NotificationService.Tests.Domain;

public class NotificationTests
{

    [Fact]
    public void ShouldSendEmail_When_StatusIsSuccess_And_FlagIsY()
    {
        var notification = new Notification("123", "Matheus", "teste@teste.com", "v-1", "Video", NotificationType.Success, "Y", "N");
        Assert.True(notification.ShouldSendEmail());
    }

    [Fact]
    public void ShouldNotSendEmail_When_StatusIsSuccess_And_FlagIsN()
    {
        var notification = new Notification("123", "Matheus", "teste@teste.com", "v-1", "Video", NotificationType.Success, "N", "N");
        Assert.False(notification.ShouldSendEmail());
    }

    [Fact]
    public void ShouldSendEmail_When_StatusIsError_And_FlagIsY()
    {
        var notification = new Notification("123", "Matheus", "teste@teste.com", "v-1", "Video", NotificationType.Error, "N", "Y");
        Assert.True(notification.ShouldSendEmail());
    }

    [Fact]
    public void ShouldNotSendEmail_When_StatusIsError_And_FlagIsN()
    {
        var notification = new Notification("123", "Matheus", "teste@teste.com", "v-1", "Video", NotificationType.Error, "Y", "N");
        Assert.False(notification.ShouldSendEmail());
    }

    [Fact]
    public void ShouldNotSendEmail_When_StatusIsUnknown()
    {
        var notification = new Notification("123", "Matheus", "teste@teste.com", "v-1", "Video", (NotificationType)999, "Y", "Y");
        Assert.False(notification.ShouldSendEmail());
    }
}