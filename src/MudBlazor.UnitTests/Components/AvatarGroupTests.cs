﻿
using System;
using System.Linq;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents.AvatarGroup;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class AvatarGroupTests : BunitTest
    {
        [Test]
        public void AvatarGroupTest()
        {
            var comp = Context.RenderComponent<AvatarGroupTest>();
            // select elements needed for the test
            var group = comp.FindComponent<MudAvatarGroup>();
            var avatars = comp.FindAll(".mud-avatar").ToArray();

            // check group
            group.Instance.Max.Should().Be(4);
            group.Instance.Spacing.Should().Be(2);
            group.Instance._avatars.Should().HaveCount(6);

            // check avatars
            avatars.Should().HaveCount(5);  // 4 avatars since max is 4 + 1 that is max avatar
            avatars[0].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[1].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[2].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[3].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[4].ClassList.Should().Contain("mud-avatar-group-max-avatar");
        }

        [Test]
        public void AvatarGroupMaxDefaultTest()
        {
            var comp = Context.RenderComponent<AvatarGroupMaxDefaultTest>();
            // select elements needed for the test
            var group = comp.FindComponent<MudAvatarGroup>();
            var avatars = group.FindAll(".mud-avatar").ToArray();

            // check initial group settings
            group.Instance.Max.Should().Be(int.MaxValue); // should default to 2147483647 when not provided
            group.Instance._avatars.Should().HaveCount(7);

            // verify MaxGroupReached is false for each avatar
            foreach (var avatar in group.Instance._avatars)
            {
                group.Instance.MaxGroupReached(avatar).Should().Be(false);
            }

            // check initial avatars
            avatars.Should().HaveCount(7);  // all 7 avatars should display
            avatars[0].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[1].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[2].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[3].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[4].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[5].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[6].ClassList.Should().NotContain("mud-avatar-group-max-avatar");

        }

        [Test]
        public void AvatarGroupChangeMaxTest()
        {
            var comp = Context.RenderComponent<AvatarGroupChangeMaxTest>();
            // select elements needed for the test
            var group = comp.FindComponent<MudAvatarGroup>();
            var avatars = comp.FindAll(".mud-avatar").ToArray();

            // check initial group settings
            group.Instance.Max.Should().Be(3);
            group.Instance.Spacing.Should().Be(3);
            group.Instance._avatars.Should().HaveCount(7);

            // check initial avatars
            avatars.Should().HaveCount(4);  // 4 avatars since max is 3 + 1 that is max avatar
            avatars[0].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[1].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[2].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[3].ClassList.Should().Contain("mud-avatar-group-max-avatar");

            // click button to change max avatars to 4
            comp.Find("#change-max-button").Click();

            // find all avatars again after click
            avatars = comp.FindAll(".mud-avatar").ToArray();

            // check group max is changed and avatars added/removed
            group.Instance.Max.Should().Be(4);
            avatars.Should().HaveCount(5);
            avatars[0].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[1].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[2].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[3].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[4].ClassList.Should().Contain("mud-avatar-group-max-avatar");

            // click button to change max avatars to 7
            comp.Find("#change-max-button").Click();
            comp.Find("#change-max-button").Click();
            comp.Find("#change-max-button").Click();

            // find all avatars again after click
            avatars = comp.FindAll(".mud-avatar").ToArray();

            // check group max is changed and avatars added/removed
            group.Instance.Max.Should().Be(7);
            avatars.Should().HaveCount(7);
            avatars[0].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[1].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[2].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[3].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[4].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[5].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[6].ClassList.Should().NotContain("mud-avatar-group-max-avatar");

            // click button one more time to set it to 0
            comp.Find("#change-max-button").Click();

            // find all avatars again after click
            avatars = comp.FindAll(".mud-avatar").ToArray();

            // check group max is changed and avatars added/removed
            group.Instance.Max.Should().Be(0);
            avatars.Should().HaveCount(1);
            avatars[0].ClassList.Should().Contain("mud-avatar-group-max-avatar");

            // click button one more time to set it to 1
            comp.Find("#change-max-button").Click();

            // find all avatars again after click
            avatars = comp.FindAll(".mud-avatar").ToArray();

            // check group max is changed and avatars added/removed
            group.Instance.Max.Should().Be(1);
            avatars.Should().HaveCount(2);
            avatars[0].ClassList.Should().NotContain("mud-avatar-group-max-avatar");
            avatars[1].ClassList.Should().Contain("mud-avatar-group-max-avatar");
        }

        [Test]
        public void AvatarGroupRemoveTest()
        {
            var comp = Context.RenderComponent<AvatarGroupRemoveTest>();

            comp.Find("#empty-button").Click();

            comp.FindComponent<MudAvatarGroup>().Instance._avatars.Count.Should().Be(0);
        }

        [Test]
        public void AvatarGroupMaxAvatarsTemplateTest()
        {
            var comp = Context.RenderComponent<AvatarGroupMaxAvatarsTemplateTest>();

            comp.FindComponent<MudButton>().Should().NotBeNull();

            comp.FindComponent<MudAvatarGroup>().FindAll(".mud-avatar-group-max-avatar").Should().HaveCount(0);
        }
    }
}
