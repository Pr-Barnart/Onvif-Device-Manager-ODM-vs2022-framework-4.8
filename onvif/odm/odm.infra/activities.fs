namespace odm.infra

open System
open System.Threading
open System.Windows
open Microsoft.Practices.Unity
open System.Runtime.CompilerServices

// Interface for an activity context
type IActivityContext<'TResult> =
    abstract container: IUnityContainer
    abstract Success: result:'TResult -> unit
    abstract Error: error:Exception -> unit
    abstract Cancel: unit -> unit
    abstract RegisterCancellationCallback: callback:Action -> IDisposable

[<Extension>]
type UnityActivityExtensions() =

    /// Start a view activity on the UI thread with proper cancellation and continuations
    [<Extension>]
    static member StartViewActivity<'TResult>
        (container: IUnityContainer, callback: Action<IActivityContext<'TResult>>) =
        async {

            let! ct = Async.CancellationToken

            return!
                Async.FromContinuations(fun (success, error, cancel) ->

                    let context =
                        { new IActivityContext<'TResult> with
                            member _.container = container
                            member _.Success(result) = success result
                            member _.Error(err) = error err
                            member _.Cancel() = cancel (OperationCanceledException())
                            member _.RegisterCancellationCallback(callback: Action) =
                                ct.Register(callback) :> IDisposable }

                    let dispatcher =
                        if Application.Current <> null then Application.Current.Dispatcher
                        else null

                    if dispatcher = null then
                        try
                            callback.Invoke(context)
                        with
                        | :? OperationCanceledException as ex -> cancel ex
                        | ex -> error ex
                    else
                        let op = dispatcher.BeginInvoke(new Action(fun () ->
                            try
                                callback.Invoke(context)
                            with
                            | :? OperationCanceledException as ex -> cancel ex
                            | ex -> error ex
                        ))
                        ()
                )
        }