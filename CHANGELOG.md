# MudBlazor Changelog

## Upcoming version
Changes:

- MudTextField: don't show conversion error for empty text if type is nullable (i.e. int?)
 
## v1.2.0
Breaking Changes:

- MudSwitch and MudSelect are now generic with support for bool, bool?, int (0,1), int? (0,1,null) and string (null, "on", "true", "off", "false"). Add T="bool" if your code breaks.
- MudForm: performance improvement by suppressing re-rendering of whole form on fireing IsValid updates. This may not break anything, but if it does, SuppressRenderingOnValidation="false" switches it off.

Changes:

- Fixed margin between icon and text in FAB
- MudSwitch and MudSelect now support MudForm and EditForm validation
- MudForm.Reset(): fixed so that it resets the textfield texts to null
- MudSelect: added parameter Strict. When set to true the select will not display values that are not in its item list