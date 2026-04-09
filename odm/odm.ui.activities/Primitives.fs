namespace odm.ui.activities

open System
open System.Windows
open System.Windows.Threading
open Microsoft.Practices.Unity

open odm.infra
open utils.fsharp

type Progress() =
    class
        static member Show(container:IUnityContainer, message:string) : Async<IDisposable> =
            Application.Current.Dispatcher.InvokeAsync(fun () ->
                let view = new ProgressView(message)
                let presenter = container.Resolve<IViewPresenter>()
                presenter.ShowView(view)
            ).Task
            |> Async.AwaitTask
    end