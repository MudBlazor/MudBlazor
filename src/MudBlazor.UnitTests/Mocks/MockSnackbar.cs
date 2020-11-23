using System;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockSnackbar : ISnackbar
    {
        public IEnumerable<Snackbar> ShownSnackbars => new Snackbar[0];

        public SnackbarConfiguration Configuration => new SnackbarConfiguration();

        public event Action OnSnackbarsUpdated;

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
