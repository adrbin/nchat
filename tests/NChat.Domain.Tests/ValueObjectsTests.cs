using FluentAssertions;
using NChat.Web.Domain.Entities;
using NChat.Web.Domain.Exceptions;
using NChat.Web.Domain.ValueObjects;

namespace NChat.Domain.Tests;

public sealed class ValueObjectsTests
{
    [Fact]
    public void Username_Should_Reject_Invalid_Characters()
    {
        var act = () => Username.Create("john doe");
        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Username_Should_Accept_Expected_Format()
    {
        var result = Username.Create("john_doe-1");
        result.Value.Should().Be("john_doe-1");
    }

    [Fact]
    public void RoomName_Should_Trim_Value()
    {
        var room = RoomName.Create("  general  ");
        room.Value.Should().Be("general");
    }

    [Fact]
    public void MessageContent_Should_Reject_Empty()
    {
        var act = () => MessageContent.Create(" ");
        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void UserSession_Should_Require_Username_Before_Join()
    {
        var session = new UserSession("conn-1");
        var act = () => session.JoinRoom(Guid.NewGuid());
        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void UserSession_Should_Keep_Only_One_Room()
    {
        var session = new UserSession("conn-1");
        session.ClaimUsername(Username.Create("john"));

        var room1 = Guid.NewGuid();
        var room2 = Guid.NewGuid();

        session.JoinRoom(room1);
        session.CurrentRoomId.Should().Be(room1);

        session.JoinRoom(room2);
        session.CurrentRoomId.Should().Be(room2);
    }

    [Fact]
    public void ChatMessage_Should_Be_Created_With_Data()
    {
        var roomId = Guid.NewGuid();
        var message = ChatMessage.Create(roomId, Username.Create("john"), MessageContent.Create("hello"));

        message.RoomId.Should().Be(roomId);
        message.Username.Should().Be("john");
        message.Content.Should().Be("hello");
    }
}
