// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using MudBlazor.Interfaces;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

#nullable enable
[TestFixture]
public class RowValidatorTests
{
    [Test]
    public async Task TableRowValidator_ValidateAsync_CollectsErrorsFromAsyncValidation()
    {
        var validator = new TableRowValidator();
        ((IForm)validator).Add(new TestFormComponent(isAsync: true, "async error"));

        await validator.ValidateAsync();

        validator.Errors.Should().BeEquivalentTo("async error");
    }

    [Test]
    public async Task TableRowValidator_ValidateAsync_WithoutErrors_IsValid()
    {
        var validator = new TableRowValidator();
        ((IForm)validator).Add(new TestFormComponent(isAsync: false));

        await validator.ValidateAsync();

        validator.Errors.Should().BeEmpty();
        validator.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task DataGridRowValidator_ValidateAsync_CollectsErrorsFromAsyncValidation()
    {
        var validator = new DataGridRowValidator();
        ((IForm)validator).Add(new TestFormComponent(isAsync: true, "async error"));

        await validator.ValidateAsync();

        validator.Errors.Should().BeEquivalentTo("async error");
    }

    [Test]
    public async Task DataGridRowValidator_ValidateAsync_WithoutErrors_IsValid()
    {
        var validator = new DataGridRowValidator();
        ((IForm)validator).Add(new TestFormComponent(isAsync: false));

        await validator.ValidateAsync();

        validator.Errors.Should().BeEmpty();
        validator.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// An <see cref="IForm"/> implementation that predates <see cref="IForm.ValidateAsync"/> must keep
    /// compiling and running: the default implementation completes without validating anything.
    /// </summary>
    [Test]
    public async Task IForm_ValidateAsync_DefaultImplementation_IsANoOp()
    {
        IForm form = new LegacyForm();

        await form.ValidateAsync();

        form.IsValid.Should().BeTrue();
        form.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// A minimal external implementer from before ValidateAsync was added to IForm.
    /// </summary>
    private sealed class LegacyForm : IForm
    {
        public bool IsValid => Errors.Length <= 0;

        public string[] Errors => [];

        public object? Model { get; set; }

        public void FieldChanged(IFormComponent formControl, object? newValue)
        {
        }

        void IForm.Add(IFormComponent formControl)
        {
        }

        void IForm.Remove(IFormComponent formControl)
        {
        }

        void IForm.Update(IFormComponent formControl)
        {
        }
    }

    /// <summary>
    /// A form component whose async validation only produces its errors after the
    /// awaited continuation resumes, so a fire-and-forget caller would miss them.
    /// </summary>
    private sealed class TestFormComponent : IFormComponent
    {
        private readonly bool _isAsync;
        private readonly string[] _errorsToRaise;

        public TestFormComponent(bool isAsync, params string[] errorsToRaise)
        {
            _isAsync = isAsync;
            _errorsToRaise = errorsToRaise;
        }

        public bool Required { get; set; }

        public bool Error { get; set; }

        public bool HasErrors => ValidationErrors.Count > 0;

        public bool Touched => false;

        public object? Validation { get; set; }

        public bool IsForNull => false;

        public List<string> ValidationErrors { get; set; } = new();

        public async Task ValidateAsync()
        {
            if (_isAsync)
            {
                await Task.Yield();
            }

            ValidationErrors = _errorsToRaise.ToList();
        }

        public Task ResetAsync()
        {
            ValidationErrors = new List<string>();
            return Task.CompletedTask;
        }

        public Task ResetValidationAsync()
        {
            ValidationErrors = new List<string>();
            return Task.CompletedTask;
        }
    }
}
