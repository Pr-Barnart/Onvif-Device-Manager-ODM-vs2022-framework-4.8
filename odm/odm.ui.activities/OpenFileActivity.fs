namespace odm.ui.activities

open System
open System.Windows
open System.Windows.Threading
open Microsoft.Win32

open utils.fsharp

type OpenFileActivityResult = 
    | Selected of string
    | Canceled

type OpenFileActivity() =
    class
        static member Run(title:string, filter:string) : Async<OpenFileActivityResult> =
            Application.Current.Dispatcher.InvokeAsync(fun () ->
                let dlg = new OpenFileDialog()
                dlg.Title <- title
                dlg.Filter <- filter
                if dlg.ShowDialog() = Nullable(true) then
                    Selected dlg.FileName
                else
                    Canceled
            ).Task
            |> Async.AwaitTask
    end