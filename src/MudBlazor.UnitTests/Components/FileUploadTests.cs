// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.Dummy;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents.FileUpload;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class FileUploadTests : BunitTest
    {
        /// <summary>
        /// Verifies that invalid T values are logged using the provided ILogger
        /// </summary>
        [Test]
        public void InvalidTLogWarning()
        {
            var provider = new MockLoggerProvider();
            var logger = provider.CreateLogger(GetType().FullName) as MockLogger;
            Context.Services.AddLogging(x => x.ClearProviders().AddProvider(provider)); //set up the logging provider
            var comp = Context.Render<MudFileUpload<MudTextField<string>>>();

            var entries = logger.GetEntries();
            entries.Count.Should().Be(1);
            entries[0].Level.Should().Be(LogLevel.Warning);
            entries[0].Message.Should().Be(string.Format("T must be of type {0} or {1}",
                typeof(IReadOnlyList<IBrowserFile>), typeof(IBrowserFile)));
        }

        /// <summary>
        /// Checks the FileUpload CSS classes
        /// </summary>
        [Test]
        public void FileUpload_CSS()
        {
            var comp = Context.Render<MudFileUpload<IBrowserFile>>(parameters => parameters
                .Add(x => x.Class, "outer-test")
                .Add(x => x.InputClass, "inner-test"));

            comp.Find(".mud-input-control.mud-file-upload.outer-test"); //find outer div

            var innerClasses = comp.Find("input").GetAttribute("class"); //find inner input
            innerClasses.Should().Be("inner-test");
        }

        /// <summary>
        /// Ensures the underlying input receives the multiple attribute
        /// </summary>
        [Test]
        public void FileUpload_Multiple()
        {
            var comp = Context.Render<MudFileUpload<IReadOnlyList<IBrowserFile>>>();

            var input = comp.Find("input");
            input.HasAttribute("multiple").Should().BeTrue();
        }

        /// <summary>
        /// Ensures the underlying input receives the hidden attribute (default case)
        /// </summary>
        [Test]
        public void FileUpload_HiddenTest1()
        {
            var comp = Context.Render<MudFileUpload<IReadOnlyList<IBrowserFile>>>();

            var input = comp.Find("input");
            input.HasAttribute("hidden").Should().BeTrue();
        }

        /// <summary>
        /// Ensures the underlying input does not receive the hidden attribute
        /// </summary>
        [Test]
        public void FileUpload_HiddenTest2()
        {
            var comp = Context.Render<MudFileUpload<IReadOnlyList<IBrowserFile>>>(parameters =>
                parameters.Add(x => x.Hidden, false));

            var input = comp.Find("input");
            input.HasAttribute("hidden").Should().BeFalse();
        }

        /// <summary>
        /// Ensures the underlying input receives the accept attribute
        /// </summary>
        [Test]
        public void FileUpload_Accept()
        {
            var comp = Context.Render<MudFileUpload<IBrowserFile>>(parameters => parameters
                .Add(x => x.Accept, ".png, .jpg"));

            var input = comp.Find("input");
            input.GetAttribute("accept").Should().Be(".png, .jpg");
        }

        /// <summary>
        /// Verifies the button template renders
        /// </summary>
        [Test]
        public void FileUpload_ButtonTemplateContextTest_Renders()
        {
            var comp = Context.Render<FileUploadWithDragAndDropActivatorTest>();

            var openFilePickerButton = comp.Find("button#open-file-picker-button");
            openFilePickerButton.ToMarkup().Should().Contain("Open file picker");

            var clearButton = comp.Find("button#clear-button");
            clearButton.ToMarkup().Should().Contain("Clear");
        }

        /// <summary>
        /// Verifies the ClearAsync function clears the Files property
        /// </summary>
        [Test]
        public async Task FileUpload_ClearAsync_Should_Clear_Files()
        {
            var fileName = "cat.jpg";
            var defaultFile = new DummyBrowserFile(fileName, DateTimeOffset.Now, 0, "image/jpeg", []);
            var comp = Context.Render<FileUploadWithDragAndDropActivatorTest>(parameters =>
                parameters.Add(x => x.File, defaultFile));
            var fileUploadComp = comp.FindComponent<MudFileUpload<IBrowserFile>>();
            var fileUploadInstance = fileUploadComp.Instance;

            fileUploadInstance.Files.Should().NotBeNull();
            fileUploadInstance.Files!.Name.Should().Be(fileName);

            await comp.Find("button#clear-button").ClickAsync();

            fileUploadInstance.Files.Should().BeNull();
        }

        /// <summary>
        /// Verifies the OpenFilePickerAsync method opens the file picker when the file picker button is clicked
        /// <remarks>
        /// Native HTML buttons trigger the onclick event when the space or enter keys are pressed.
        /// If users use something that does not render a native button, they will need to add the appropriate keyboard event handlers.
        /// </remarks>
        /// </summary>
        [Test]
        public async Task FileUpload_OpenFilePickerAsync_Should_OpenFilePicker_When_Clicked()
        {
            var comp = Context.Render<FileUploadWithDragAndDropActivatorTest>();

            await comp.Find("button#open-file-picker-button").ClickAsync();

            Context.JSInterop.Invocations.Should().ContainSingle(invocation => invocation.Identifier == "mudFileUpload.openFilePicker");
        }

        /// <summary>
        /// Tests the OnFilesChangedEvent
        /// </summary>
        [Test]
        public async Task FileUpload_OnFilesChanged()
        {
            var fileContent = InputFileContent.CreateFromText("Garderoben is a farmer!", "upload.txt");

            var comp = Context.Render<FileUploadOnFilesChangedTest>();

            var input = comp.FindComponent<InputFile>();
            input.UploadFiles(fileContent);

            comp.Instance.File.Name.Should().Be("upload.txt");
            var fileString = await comp.Instance.File.GetFileContents();

            fileString.Should().Be("Garderoben is a farmer!");
        }

        /// <summary>
        /// Tests the FileValueChanged event bound to a form
        /// </summary>
        [Test]
        public async Task FileUpload_FileValueChanged()
        {
            InputFileContent[] fileContent =
            [
                InputFileContent.CreateFromText("Garderoben is a farmer!", "upload.txt"),
                InputFileContent.CreateFromText("A Balrog, servant of Morgoth", "upload2.txt")
            ];

            var comp = Context.Render<FileUploadFormValidationTest>();

            var inputs = comp.FindComponents<InputFile>();
            inputs.Count.Should().Be(2);

            inputs[0].UploadFiles(fileContent[0]); //upload single file

            comp.Instance.Model.File.Should().NotBeNull();
            comp.Instance.Model.File.Name.Should().Be("upload.txt");
            var fileString = await comp.Instance.Model.File.GetFileContents();
            fileString.Should().Be("Garderoben is a farmer!");

            inputs[1].UploadFiles(fileContent); //upload both files

            comp.Instance.Model.Files.Count.Should().Be(2);
            comp.Instance.Model.Files[0].Name.Should().Be("upload.txt");
            comp.Instance.Model.Files[1].Name.Should().Be("upload2.txt");
            var fileString1 = await comp.Instance.Model.Files[0].GetFileContents();
            fileString1.Should().Be("Garderoben is a farmer!");
            var fileString2 = await comp.Instance.Model.Files[1].GetFileContents();
            fileString2.Should().Be("A Balrog, servant of Morgoth");
        }

        /// <summary>
        /// Tests the FileValueChanged event bound to a form with validation
        /// </summary>
        [Test]
        public async Task FileUpload_Validation()
        {
            InputFileContent[] fileContent =
            [
                InputFileContent.CreateFromText("Garderoben is a farmer!", "upload.txt"),
                InputFileContent.CreateFromText("A Balrog, servant of Morgoth", "upload2.txt")
            ];

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; //<<< rework this!
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            var comp = Context.Render<FileUploadFormValidationTest>();

            var form = comp.Instance.Form;
            await comp.InvokeAsync(() => form.Validate());

            form.IsValid.Should().BeFalse(); //form is invalid to start

            var single = comp.FindComponent<MudFileUpload<IBrowserFile>>();
            single.Instance.GetState(x => x.ErrorText).Should().Be("'File' must not be empty.");
            single.Markup.Should().Contain("'File' must not be empty.");

            var multiple = comp.FindComponent<MudFileUpload<IReadOnlyList<IBrowserFile>>>();
            multiple.Instance.GetState(x => x.ErrorText).Should().Be("'Files' must not be empty.");
            multiple.Markup.Should().Contain("'Files' must not be empty.");

            var singleInput = single.FindComponent<InputFile>();
            singleInput.UploadFiles(fileContent[0]); //upload first file

            await comp.InvokeAsync(() => form.Validate());

            single.Instance.GetState(x => x.ErrorText).Should().BeNull();
            single.Markup.Should().NotContain("'File' must not be empty.");

            form.IsValid.Should().BeFalse(); //form is still invalid

            var multipleInput = multiple.FindComponent<InputFile>();
            multipleInput.UploadFiles(fileContent); //upload second files

            await comp.InvokeAsync(() => form.Validate());

            single.Instance.GetState(x => x.ErrorText).Should().BeNull();
            single.Markup.Should().NotContain("'Files' must not be empty.");

            form.IsValid.Should().BeTrue(); //form is now valid
        }

        /// <summary>
        /// Tests that more than 10 files can be uploaded
        /// </summary>
        [Test]
        public void FileUpload_MaximumFileCount()
        {
            List<InputFileContent> files = [];
            for (var i = 0; i < 11; i++)
            {
                files.Add(InputFileContent.CreateFromText("Garderoben is a farmer!", $"upload{i}.txt"));
            }

            files.Count.Should().Be(11); //ensure there are 11 files

            var comp = Context.Render<FileUploadMultipleFilesTest>();

            var multiple = comp.FindComponent<MudFileUpload<IReadOnlyList<IBrowserFile>>>();
            var multipleInput = multiple.FindComponent<InputFile>();
            multipleInput.UploadFiles([.. files]); //upload second files

            comp.Instance.Files.Count.Should()
                .Be(11); //if no error occurs, we have successfully uploaded more than 10 files
        }

        /// <summary>
        /// Makes sure the file upload is disabled
        /// </summary>
        [Test]
        public async Task FileUploadDisabled()
        {
            var comp = Context.Render<FileUploadDisabledTest>();
            comp.FindComponent<MudFileUpload<IBrowserFile>>().Find("input").HasAttribute("disabled").Should().BeFalse();
            comp.FindComponent<MudFileUpload<IBrowserFile>>().Find("button").HasAttribute("disabled").Should().BeFalse();

            await comp.SetParametersAndRenderAsync(parameters =>
                parameters.Add(x => x.Disabled,
                    true)); //The input and child button should be disabled when file upload is disabled

            comp.FindComponent<MudFileUpload<IBrowserFile>>().Find("input").HasAttribute("disabled").Should().BeTrue();
            comp.FindComponent<MudFileUpload<IBrowserFile>>().Find("button").HasAttribute("disabled").Should()
                .BeTrue(); //we need to test for a button as the MudButton replaces disabled labels with buttons
        }

        /// <summary>
        /// Verifies files are appended correctly
        /// </summary>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FileUploadAppendMultiple(bool appendMultiple)
        {
            var comp = Context.Render<FileUploadAppendMultipleTest>(p =>
                p.Add(x => x.AppendMultipleFiles, appendMultiple));

            var input = comp.FindComponent<InputFile>();
            input.UploadFiles(GenerateFile(), GenerateFile(), GenerateFile()); //upload first file
            comp.Instance.Files.Count.Should().Be(3);

            input.UploadFiles(GenerateFile());
            comp.Instance.Files.Count.Should().Be(appendMultiple ? 4 : 1);

            static InputFileContent GenerateFile()
            {
                return InputFileContent.CreateFromText("snakex64 is Canadian", $"{Guid.NewGuid()}.txt");
            }
        }

        /// <summary>
        /// Optional FileUpload should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalFileUpload_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.Render<MudFileUpload<IBrowserFile>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required FileUpload should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredFileUpload_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<MudFileUpload<IBrowserFile>>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required FileUpload attributes should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredFileUploadAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<MudFileUpload<IBrowserFile>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// FileUpload should generate new InputFile on file change.
        /// </summary>
        [Test]
        public async Task Generate_new_InputFile_on_file_change()
        {
            var comp = Context.Render<MudFileUpload<IBrowserFile>>();

            // only 1 input element should be present
            comp.FindAll("input").Should().HaveCount(1);

            // trigger an OnChange on the internal InputFile
            var defaultFile = new DummyBrowserFile("filename.jpg", DateTimeOffset.Now, 0, "image/jpeg", []);
            await comp.InvokeAsync(() => comp.FindComponent<InputFile>().Instance.OnChange.InvokeAsync(new InputFileChangeEventArgs([defaultFile])));

            // 2 input elements should now be present
            // one should be visible
            comp.FindAll("input:not(.d-none)").Should().HaveCount(1);
            // and the other should no longer be visible
            comp.FindAll("input.d-none").Should().HaveCount(1);
        }

        /// <summary>
        /// FileUpload should trigger the FilesChanged and OnFilesChanged callbacks when appropriate.
        /// </summary>
        [Test]
        public async Task Should_trigger_file_change_callbacks_as_expected()
        {
            var comp = Context.Render<FileUploadChangeCountTests>();

            // first file change should trigger both callbacks
            var fileContent = new byte[5];
            // fill file content with random bytes
            new Random().NextBytes(fileContent);
            var firstFile = new DummyBrowserFile("filename.jpg", DateTimeOffset.Now, 0, "image/jpeg", fileContent);
            await comp.InvokeAsync(() => comp.FindComponents<InputFile>()[0].Instance.OnChange.InvokeAsync(new InputFileChangeEventArgs([firstFile])));

            comp.Instance.FilesChangedCount.Should().Be(1);
            comp.Instance.OnFilesChangedCount.Should().Be(1);

            // since a new InputFile is generated with each upload, we can get the last InputFile in the render chain to emulate a new upload
            // so when a new file reference is uploaded, both file change callbacks should be triggered
            new Random().NextBytes(fileContent);
            var secondFile = new DummyBrowserFile("filename.jpg", DateTimeOffset.Now, 0, "image/jpeg", fileContent);
            await comp.InvokeAsync(() => comp.FindComponents<InputFile>()[^1].Instance.OnChange.InvokeAsync(new InputFileChangeEventArgs([secondFile])));

            comp.Instance.FilesChangedCount.Should().Be(2);
            comp.Instance.OnFilesChangedCount.Should().Be(2);
        }

        /// <summary>
        /// Tests drag and drop functionality
        /// </summary>
        [Test]
        public async Task FileUpload_DragAndDrop_Test()
        {
            var comp = Context.Render<MudFileUpload<IBrowserFile>>(parameters => parameters
                .Add(x => x.DragAndDrop, true));

            // Verify drag area exists
            comp.Find(".mud-file-upload-dragarea").Should().NotBeNull();

            // Test drag enter
            await comp.Find(".mud-file-upload-dragarea").DragEnterAsync();
            comp.Find(".mud-file-upload-dragarea").ClassList.Should().Contain("mud-border-primary");

            // Test drag leave
            await comp.Find("input").DragLeaveAsync();
            comp.Find(".mud-file-upload-dragarea").ClassList.Should().NotContain("mud-border-primary");

            // Test drag end
            await comp.Find("input").DragEndAsync();
            comp.Find(".mud-file-upload-dragarea").ClassList.Should().NotContain("mud-border-primary");
        }

        /// <summary>
        /// Ensures the default drag-and-drop activator uses a semantic button.
        /// </summary>
        [Test]
        public void FileUpload_DragAndDrop_DefaultActivator_Should_Render_Button()
        {
            var comp = Context.Render<MudFileUpload<IBrowserFile>>(parameters => parameters
                .Add(x => x.DragAndDrop, true));

            var dragAreaButton = comp.Find("button.mud-file-upload-dragarea");
            dragAreaButton.GetAttribute("type").Should().Be("button");
        }

        /// <summary>
        /// Ensures clicking the default drag-and-drop activator opens the native file picker.
        /// </summary>
        [Test]
        public async Task FileUpload_DragAndDrop_DefaultActivator_Click_Should_OpenFilePicker()
        {
            var comp = Context.Render<MudFileUpload<IBrowserFile>>(parameters => parameters
                .Add(x => x.DragAndDrop, true));

            await comp.Find("button.mud-file-upload-dragarea").ClickAsync();

            Context.JSInterop.Invocations.Should().ContainSingle(invocation => invocation.Identifier == "mudFileUpload.openFilePicker");
        }

        /// <summary>
        /// Ensures the default drag-and-drop activator reflects disabled state.
        /// </summary>
        [Test]
        public void FileUpload_DragAndDrop_DefaultActivator_Should_Respect_Disabled_State()
        {
            var comp = Context.Render<MudFileUpload<IBrowserFile>>(parameters => parameters
                .Add(x => x.DragAndDrop, true)
                .Add(x => x.Disabled, true));

            var dragAreaButton = comp.Find("button.mud-file-upload-dragarea");
            dragAreaButton.HasAttribute("disabled").Should().BeTrue();
            dragAreaButton.ClassList.Should().NotContain("mud-file-upload-dragarea-clickable");
        }

        /// <summary>
        /// Tests RemoveFileAsync functionality for single file
        /// </summary>
        [Test]
        public async Task FileUpload_RemoveFileAsync_SingleFile_Test()
        {
            var fileName = "test.txt";
            var defaultFile = new DummyBrowserFile(fileName, DateTimeOffset.Now, 0, "text/plain", []);
            var comp = Context.Render<MudFileUpload<IBrowserFile>>(parameters => parameters
                .Add(x => x.Files, defaultFile));

            // Verify initial state
            comp.Instance.Files.Should().NotBeNull();
            comp.Instance.GetFilenames().Should().ContainSingle(x => x == fileName);

            // Remove file
            await comp.InvokeAsync(() => comp.Instance.RemoveFileAsync(fileName));

            // Verify file was removed
            comp.Instance.GetState(x => x.Files).Should().BeNull();
            comp.Instance.GetFilenames().Should().BeEmpty();
        }

        /// <summary>
        /// Tests RemoveFileAsync functionality for multiple files
        /// </summary>
        [Test]
        public async Task FileUpload_RemoveFileAsync_MultipleFiles_Test()
        {
            var files = new List<IBrowserFile>
            {
                new DummyBrowserFile("test1.txt", DateTimeOffset.Now, 0, "text/plain", []),
                new DummyBrowserFile("test2.txt", DateTimeOffset.Now, 0, "text/plain", [])
            };

            var comp = Context.Render<MudFileUpload<IReadOnlyList<IBrowserFile>>>(parameters => parameters
                .Add(x => x.Files, files));

            // Verify initial state
            comp.Instance.Files.Should().HaveCount(2);
            comp.Instance.GetFilenames().Should().HaveCount(2);

            // Remove one file
            await comp.InvokeAsync(() => comp.Instance.RemoveFileAsync("test1.txt"));

            // Verify file was removed
            comp.Instance.GetState(x => x.Files).Should().HaveCount(1);
            comp.Instance.GetFilenames().Should().ContainSingle(x => x == "test2.txt");
        }

        /// <summary>
        /// Tests SelectedTemplate rendering
        /// </summary>
        [Test]
        public async Task FileUpload_SelectedTemplate_Test()
        {
            // Arrange
            var files = new List<IBrowserFile>
            {
                new DummyBrowserFile("test1.txt", DateTimeOffset.Now, 0, "text/plain", []),
                new DummyBrowserFile("test2.txt", DateTimeOffset.Now, 0, "text/plain", [])
            };

            // Create the component with initial template
            var comp = Context.Render<MudFileUpload<IReadOnlyList<IBrowserFile>>>(parameters => parameters
                .Add(x => x.SelectedTemplate, context => builder =>
                {
                    builder.AddContent(0, $"Selected files: {context?.Count ?? 0}");
                }));

            // Initial state should show 0 files
            comp.Markup.Should().Contain("Selected files: 0");

            // Simulate file upload by triggering OnChange
            var inputFile = comp.FindComponent<InputFile>();
            await comp.InvokeAsync(() => inputFile.Instance.OnChange.InvokeAsync(
                new InputFileChangeEventArgs(files)));

            // Re-render and verify
            comp.Render();
            comp.Markup.Should().Contain("Selected files: 2");
        }

        /// <summary>
        /// Tests the SuppressOnChangeWhenInvalid behavior in the FileUpload component
        /// </summary>
        [Test]
        public async Task FileUpload_SuppressOnChangeWhenInvalidTest()
        {
            // Arrange
            var suppressOnChangeWhenInvalid = true;
            var files = new List<IBrowserFile>
            {
                new DummyBrowserFile("valid.txt", DateTimeOffset.Now, 1024, "text/plain", []),
                new DummyBrowserFile("invalid.txt", DateTimeOffset.Now, 10485761, "text/plain", [])
            };

            var comp = Context.Render<FileUploadFormValidationTest>(parameters => parameters
                .Add(p => p.SuppressOnChangeWhenInvalid, suppressOnChangeWhenInvalid));

            // Act 1: Upload a valid file
            await comp.InvokeAsync(() => comp.FindComponents<InputFile>()[0].Instance.OnChange.InvokeAsync(new InputFileChangeEventArgs([files[0]])));

            // Assert: The valid file should trigger OnFilesChanged
            comp.Instance.Model.File.Should().NotBeNull();
            comp.Instance.Model.File.Name.Should().Be("valid.txt");
            comp.Instance.OnFilesChangedCount.Should().Be(1);

            // Act 2: Upload an invalid file
            await comp.InvokeAsync(() => comp.FindComponents<InputFile>()[0].Instance.OnChange.InvokeAsync(new InputFileChangeEventArgs([files[1]])));

            // Assert: The invalid file should NOT trigger OnFilesChanged
            comp.Instance.Model.File.Should().NotBeNull();
            comp.Instance.Model.File.Name.Should().Be("invalid.txt");
            comp.Instance.OnFilesChangedCount.Should().Be(1);
        }

        [Test]
        [TestCase(typeof(IBrowserFile))]
        [TestCase(typeof(IReadOnlyList<IBrowserFile>))]
        [TestCase(null)]
        public void GetFilenames_ShouldNotThrow_ForDifferentTypes(Type type)
        {
            // Act & Assert
            Action action = type switch
            {
                not null when type == typeof(IBrowserFile) => () => Context.Render<MudFileUpload<IBrowserFile>>().Instance.GetFilenames(),
                not null when type == typeof(IReadOnlyList<IBrowserFile>) => () => Context.Render<MudFileUpload<IReadOnlyList<IBrowserFile>>>().Instance.GetFilenames(),
                _ => () => Context.Render<MudFileUpload<object>>().Instance.GetFilenames()
            };

            action.Should().NotThrow();
        }

        private static InputFileContent CreateDummyFile(string fileName, long size)
        {
            var content = new byte[size];
            var file = new DummyBrowserFile(fileName, DateTimeOffset.Now, size, "application/octet-stream", content);

            return InputFileContent.CreateFromBinary(file.Content, file.Name, null, file.ContentType);
        }

        [Test]
        public void MaxFileSize_SingleFile_WithinLimit()
        {
            var comp = Context.Render<FileUploadSingleFileTest>(parameters => parameters.Add(p => p.MaxFileSize, 100L));

            var file = CreateDummyFile("test.txt", 50);
            var input = comp.FindComponent<InputFile>();

            input.UploadFiles(file);

            comp.Instance.File.Should().NotBeNull();
            comp.Instance.File.Name.Should().Be("test.txt");
            comp.Instance.File.Size.Should().Be(50);
        }

        [Test]
        public void MaxFileSize_SingleFile_ExceedsLimit()
        {
            var comp = Context.Render<FileUploadSingleFileTest>(parameters => parameters.Add(p => p.MaxFileSize, 100L));

            var file = CreateDummyFile("test.txt", 150);
            var input = comp.FindComponent<InputFile>();
            var fileUpload = comp.FindComponent<MudFileUpload<IBrowserFile>>().Instance;

            input.UploadFiles(file);

            comp.Instance.File.Should().BeNull(); // File should be rejected
            fileUpload.GetState(x => x.Error).Should().BeTrue();
            fileUpload.GetState(x => x.ErrorText).Should().Be("File 'test.txt' exceeds the maximum allowed size of 100 bytes.");
        }

        [Test]
        public void MaxFileSize_SingleFile_NoLimit()
        {
            var comp = Context.Render<FileUploadSingleFileTest>(parameters => parameters.Add(p => p.MaxFileSize, null));

            var file = CreateDummyFile("test.txt", 200);
            var input = comp.FindComponent<InputFile>();
            var fileUpload = comp.FindComponent<MudFileUpload<IBrowserFile>>().Instance;

            input.UploadFiles(file);

            comp.Instance.File.Should().NotBeNull();
            comp.Instance.File.Name.Should().Be("test.txt");
            comp.Instance.File.Size.Should().Be(200);
            fileUpload.GetState(x => x.Error).Should().BeFalse();
            fileUpload.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
        }

        [Test]
        public void MaxFileSize_MultipleFiles_AllWithinLimit()
        {
            var comp = Context.Render<FileUploadMultipleFilesTest>(parameters => parameters.Add(p => p.MaxFileSize, 100L));

            var file1 = CreateDummyFile("test1.txt", 50);
            var file2 = CreateDummyFile("test2.txt", 70);

            var input = comp.FindComponent<InputFile>();
            var fileUpload = comp.FindComponent<MudFileUpload<IReadOnlyList<IBrowserFile>>>().Instance;

            input.UploadFiles(file1, file2);

            comp.Instance.Files.Should().NotBeNull();
            comp.Instance.Files.Count.Should().Be(2);
            comp.Instance.Files[0].Name.Should().Be("test1.txt");
            comp.Instance.Files[1].Name.Should().Be("test2.txt");
            fileUpload.GetState(x => x.Error).Should().BeFalse();
            fileUpload.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
        }

        [Test]
        public void MaxFileSize_MultipleFiles_SomeExceedLimit()
        {
            var comp = Context.Render<FileUploadMultipleFilesTest>(parameters => parameters.Add(p => p.MaxFileSize, 100L));

            var file1 = CreateDummyFile("test1.txt", 50);
            var file2 = CreateDummyFile("test2.txt", 120);
            var file3 = CreateDummyFile("test3.txt", 70);

            var input = comp.FindComponent<InputFile>();
            var fileUpload = comp.FindComponent<MudFileUpload<IReadOnlyList<IBrowserFile>>>().Instance;

            input.UploadFiles(file1, file2, file3);

            // Assertions after OnChangeAsync
            comp.Instance.Files.Should().NotBeNull();
            comp.Instance.Files.Count.Should().Be(2);
            comp.Instance.Files.Should().Contain(f => f.Name == "test1.txt");
            comp.Instance.Files.Should().Contain(f => f.Name == "test3.txt");
            fileUpload.GetState(x => x.Error).Should().BeTrue();
            fileUpload.GetState(x => x.ErrorText).Should().Be("File 'test2.txt' exceeds the maximum allowed size of 100 bytes.");
        }

        [Test]
        public void MaxFileSize_MultipleFiles_AllExceedLimit()
        {
            var comp = Context.Render<FileUploadMultipleFilesTest>(parameters => parameters.Add(p => p.MaxFileSize, 100L));

            var file1 = CreateDummyFile("test1.txt", 120);
            var file2 = CreateDummyFile("test2.txt", 150);

            var input = comp.FindComponent<InputFile>();

            input.UploadFiles(file1, file2);
            var fileUpload = comp.FindComponent<MudFileUpload<IReadOnlyList<IBrowserFile>>>().Instance;

            comp.Instance.Files.Should().NotBeNull(); // It will be an empty list
            comp.Instance.Files.Count.Should().Be(0);
            fileUpload.GetState(x => x.Error).Should().BeTrue();

            var validationErrors = fileUpload.ValidationErrors;
            validationErrors.Should().HaveCount(2);
            validationErrors.Should().Contain("File 'test1.txt' exceeds the maximum allowed size of 100 bytes.");
            validationErrors.Should().Contain("File 'test2.txt' exceeds the maximum allowed size of 100 bytes.");
        }

        [Test]
        public void MaxFileSize_MultipleFiles_NoLimit()
        {
            var comp = Context.Render<FileUploadMultipleFilesTest>(parameters => parameters.Add(p => p.MaxFileSize, null));

            var file1 = CreateDummyFile("test1.txt", 200);
            var file2 = CreateDummyFile("test2.txt", 300);

            var input = comp.FindComponent<InputFile>();
            var fileUpload = comp.FindComponent<MudFileUpload<IReadOnlyList<IBrowserFile>>>().Instance;

            input.UploadFiles(file1, file2);

            comp.Instance.Files.Should().NotBeNull();
            comp.Instance.Files.Count.Should().Be(2);
            comp.Instance.Files[0].Name.Should().Be("test1.txt");
            comp.Instance.Files[1].Name.Should().Be("test2.txt");
            fileUpload.GetState(x => x.Error).Should().BeFalse();
            fileUpload.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
        }

        [Test]
        public async Task MaxFileSize_ClearValidationAfterError()
        {
            var comp = Context.Render<FileUploadMultipleFilesTest>(parameters => parameters.Add(p => p.MaxFileSize, 100));

            var file1 = CreateDummyFile("test1.txt", 200);
            var file2 = CreateDummyFile("test2.txt", 300);

            var input = comp.FindComponent<InputFile>();

            input.UploadFiles(file1, file2);

            // Assert initial error state
            comp.Instance.Files.Should().BeEmpty();

            var fileUpload = comp.FindComponent<MudFileUpload<IReadOnlyList<IBrowserFile>>>().Instance;

            fileUpload.GetState(x => x.Error).Should().BeTrue();
            fileUpload.GetState(x => x.ErrorText).Should().Be("File 'test1.txt' exceeds the maximum allowed size of 100 bytes.");

            await comp.InvokeAsync(fileUpload.ClearAsync);

            // Assert cleared state
            comp.Instance.Files.Should().BeNull();
            fileUpload.GetState(x => x.Error).Should().BeFalse(); // Errors should be cleared
            fileUpload.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            fileUpload.ValidationErrors.Should().BeEmpty(); // ValidationErrors related to MaxFileSize should be cleared
        }

        [Test]
        public async Task MaxFileSize_ResetValidationAfterError()
        {
            var comp = Context.Render<FileUploadSingleFileTest>(parameters => parameters.Add(p => p.MaxFileSize, 100));

            var file1 = CreateDummyFile("test1.txt", 200);

            var input = comp.FindComponent<InputFile>();
            var fileUpload = comp.FindComponent<MudFileUpload<IBrowserFile>>().Instance;

            input.UploadFiles(file1);

            // Assert initial error state
            comp.Instance.File.Should().BeNull();
            fileUpload.GetState(x => x.Error).Should().BeTrue();
            fileUpload.GetState(x => x.ErrorText).Should().Be("File 'test1.txt' exceeds the maximum allowed size of 100 bytes.");

            await comp.InvokeAsync(fileUpload.ResetValidationAsync);

            // Assert cleared state
            comp.Instance.File.Should().BeNull();
            fileUpload.GetState(x => x.Error).Should().BeFalse(); // Errors should be cleared
            fileUpload.GetState(x => x.ErrorText).Should().BeNullOrEmpty(); // ErrorText should be cleared
            fileUpload.ValidationErrors.Should().BeEmpty(); // ValidationErrors related to MaxFileSize should be cleared
        }
    }
}
