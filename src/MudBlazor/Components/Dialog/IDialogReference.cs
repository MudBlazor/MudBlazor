using System.Threading.Tasks;

namespace MudBlazor.Dialog
{
    public interface IDialogReference
    {
        Task<DialogResult> Result { get; }

        void Close();
        void Close(DialogResult result);
    }
}
