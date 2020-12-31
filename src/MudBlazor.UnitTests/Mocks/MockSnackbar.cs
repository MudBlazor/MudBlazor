using System;
using System.Collections.Generic;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockSnackbar : ISnackbar
    {
        public IEnumerable<Snackbar> ShownSnackbars => new Snackbar[0];

        public SnackbarConfiguration Configuration => new SnackbarConfiguration();

#pragma warning disable CS0067 // justification implementing interface 
        public event Action OnSnackbarsUpdated;
#pragma warning restore CS0067

        public void Add(string message, Severity severity, Action<SnackbarOptions> configure)
        {
            
        }

        public void AddNew(Severity severity, string message, Action<SnackbarOptions> configure)
        {

        }

        public void Clear()
        {
            
        }


        public void Dispose()
        {
            
        }


        public void Remove(Snackbar snackbar)
        {
            
        }

    }
}
