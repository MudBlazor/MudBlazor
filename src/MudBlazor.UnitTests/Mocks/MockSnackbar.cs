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

        public void Add(SnackbarType type, string message, Action<SnackbarOptions> configure)
        {
            
        }

        public void Clear()
        {
            
        }

        public void Default(string message, Action<SnackbarOptions> configure = null)
        {
            
        }

        public void Dispose()
        {
            
        }

        public void Error(string message, Action<SnackbarOptions> configure = null)
        {
            
        }

        public void Info(string message, Action<SnackbarOptions> configure = null)
        {
            
        }

        public void Remove(Snackbar snackbar)
        {
            
        }

        public void Success(string message, Action<SnackbarOptions> configure = null)
        {
            
        }

        public void Warning(string message, Action<SnackbarOptions> configure = null)
        {
            
        }
    }
}
