Components placed under TestComponents (i.e. in a TestComponents.* namespace) are loaded by the viewer.
The name does not need to contain "Test"; discovery is location-based.

Each component is addressable at /viewer/<path>, where <path> is its folder path relative to
TestComponents plus the type name (for example Menu/MenuTest1).

Helper or sub-components that are not meaningful to open on their own (for example dialog content shown
via the dialog service) can be hidden from the sidebar while staying routable by adding:

@attribute [ViewerHidden]

To show a description in the viewer, add a static __description__ field in the @code section:

@code {
    public static string __description__ = "...";
}
