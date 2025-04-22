// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Moq;
using MudBlazor.Resources;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.Localization;

#nullable enable
[TestFixture]
public class DefaultLocalizationEnumInterceptorTests
{
    private enum TestEnum
    {
        [Display(Name = "Enum value is localized")]
        ValueWithDisplay,

        [Display(Name = LanguageResource.MudDataGrid_IsEmpty)]
        ValueWithDisplayLanguageResource,

        ValueWithoutDisplay,

        [Display(Name = "")]
        ValueWithEmptyDisplayName,

        [Display]
        ValueWithNullDisplayName
    }

    [Test]
    public void Constructor_ShouldThrowArgumentNullException_WhenInterceptorIsNull()
    {
        // Arrange
        ILocalizationInterceptor? interceptor = null;

        // Act
        var construct = () => new DefaultLocalizationEnumInterceptor(interceptor!);

        // Assert
        construct.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Handle_ShouldReturnLocalizedDisplayName_WhenDisplayAttributeIsPresent_AndLanguageResourceNotFound()
    {
        // Arrange
        const TestEnum EnumValue = TestEnum.ValueWithDisplay;
        var localizationInterceptorMock = new Mock<ILocalizationInterceptor>();
        var enumInterceptor = new DefaultLocalizationEnumInterceptor(localizationInterceptorMock.Object);
        localizationInterceptorMock.Setup(i => i.Handle(It.IsAny<string>())).Returns(new LocalizedString("xxx", "random", resourceNotFound: true));

        // Act
        var result = enumInterceptor.Handle(EnumValue);

        // Assert
        result.Should().Be("Enum value is localized");
    }

    [Test]
    public void Handle_ShouldLanguageResourceLocalization_WhenDisplayAttributeIsPresent_AndLanguageResourceFound()
    {
        // Arrange
        const TestEnum EnumValue = TestEnum.ValueWithDisplayLanguageResource;
        var localizationInterceptorMock = new Mock<ILocalizationInterceptor>();
        var enumInterceptor = new DefaultLocalizationEnumInterceptor(localizationInterceptorMock.Object);
        localizationInterceptorMock.Setup(i => i.Handle(LanguageResource.MudDataGrid_IsEmpty)).Returns(new LocalizedString(LanguageResource.MudDataGrid_IsEmpty, "Is Empty", resourceNotFound: false));

        // Act
        var result = enumInterceptor.Handle(EnumValue);

        // Assert
        result.Should().Be("Is Empty");
    }

    [Test]
    public void Handle_ShouldReturnEnumName_WhenDisplayAttributeIsNotPresent()
    {
        // Arrange
        const TestEnum EnumValue = TestEnum.ValueWithoutDisplay;
        var localizationInterceptorMock = new Mock<ILocalizationInterceptor>();
        var enumInterceptor = new DefaultLocalizationEnumInterceptor(localizationInterceptorMock.Object);

        // Act
        var result = enumInterceptor.Handle(EnumValue);

        // Assert
        result.Should().Be(nameof(TestEnum.ValueWithoutDisplay));
    }

    [Test]
    public void Handle_ShouldReturnEnumName_WhenDisplayAttributeNameIsNull()
    {
        // Arrange
        const TestEnum EnumValue = TestEnum.ValueWithNullDisplayName;
        var localizationInterceptorMock = new Mock<ILocalizationInterceptor>();
        var enumInterceptor = new DefaultLocalizationEnumInterceptor(localizationInterceptorMock.Object);

        // Act
        var result = enumInterceptor.Handle(EnumValue);

        // Assert
        result.Should().Be(nameof(TestEnum.ValueWithNullDisplayName));
    }

    [Test]
    public void Handle_ShouldReturnEmpty_WhenDisplayAttributeNameIsEmpty()
    {
        // Arrange
        const TestEnum EnumValue = TestEnum.ValueWithEmptyDisplayName;
        var localizationInterceptorMock = new Mock<ILocalizationInterceptor>();
        var enumInterceptor = new DefaultLocalizationEnumInterceptor(localizationInterceptorMock.Object);

        // Act
        var result = enumInterceptor.Handle(EnumValue);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void Handle_ShouldReturnEmpty_WhenEnumNotExist()
    {
        // Arrange
        const TestEnum EnumValue = (TestEnum)999;
        var localizationInterceptorMock = new Mock<ILocalizationInterceptor>();
        var enumInterceptor = new DefaultLocalizationEnumInterceptor(localizationInterceptorMock.Object);

        // Act
        var result = enumInterceptor.Handle(EnumValue);

        // Assert
        result.Should().BeEmpty();
    }
}
