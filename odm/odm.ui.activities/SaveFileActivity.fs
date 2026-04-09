namespace odm.ui.activities

open System
open System.Windows
open System.Windows.Threading
open Microsoft.Win32

open utils.fsharp

type SaveFileActivityResult = 
    | Selected of string
    | Canceled

type SaveFileActivity() =
    class
        static member Run(filter:string) : Async<SaveFileActivityResult> =
            Application.Current.Dispatcher.InvokeAsync(fun () ->
                let dlg = new SaveFileDialog()
                dlg.Filter <- filter
                if dlg.ShowDialog() = Nullable(true) then
                    Selected dlg.FileName
                else
                    Canceled
            ).Task
            |> Async.AwaitTask
    end