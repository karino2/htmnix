open System
open System.Drawing
open System.IO
open System.Reflection
open System.Text.Json
open PhotinoNET

type Message = {Type: string; Body: string}


let sendMessage (wnd:PhotinoWindow) (message:Message) =
    let msg = JsonSerializer.Serialize(message)
    wnd.SendWebMessage(msg) |> ignore

let onMessage html (wnd:Object) (message:string) =
    let pwnd = wnd :?>PhotinoWindow
    let msg = JsonSerializer.Deserialize<Message>(message)
    match msg.Type with
    | "notifyLoaded" -> sendMessage pwnd {Type="showHtml"; Body=html}
    | "notifyCancel" -> Environment.Exit 1
    | "notifyDone" ->
        let result = JsonSerializer.Deserialize<string array>(msg.Body)
        result|> Array.iter (printfn "%s")
        Environment.Exit 0
    | "notifyDeb" -> printfn "deb: %s" msg.Body
    | _ -> failwithf "Unknown msg type %s" msg.Type

let launchBrowser (html : string)  =
    let onFinish (results:string array) = ()
    let onCancel () = ()


    let asm = Assembly.GetExecutingAssembly()

    let load (url:string) (prefix:string) =
        let fname = url.Substring(prefix.Length)
        asm.GetManifestResourceStream($"htmnix.assets.{fname}")

    let win = (new PhotinoWindow(null))

    win.LogVerbosity <- 0
    win.SetTitle("htmnix")
        .SetUseOsDefaultSize(false)
        .Center()
        .RegisterCustomSchemeHandler("resjs",
            PhotinoWindow.NetCustomSchemeDelegate(fun sender scheme url contentType ->
                contentType <- "text/javascript"
                load url "resjs:"))
        .RegisterCustomSchemeHandler("rescss", 
            PhotinoWindow.NetCustomSchemeDelegate(fun sender scheme url contentType ->
                contentType <- "text/css"
                load url "rescss:")) |> ignore
                
    let asm = Assembly.GetExecutingAssembly()
    use stream = asm.GetManifestResourceStream("htmnix.assets.index.html")
    use sr = new StreamReader(stream)
    let text = sr.ReadToEnd()
    // printfn "content: %s" text

    win.RegisterWebMessageReceivedHandler(System.EventHandler<string>(onMessage html))
        .Center()
        .SetSize(new Size(1200, 700))
        .LoadRawString(text)
        .WaitForClose()

let readLines () =
    fun _ -> Console.ReadLine()
    |>  Seq.initInfinite
    |>  Seq.takeWhile ((<>) null) 
    |>  Seq.toList

[<EntryPoint>]
let main argv =
    if argv.Length <> 0 then
        printfn "usage: htmnix"
        printfn "Read html from stdin. No argument."
        1
    else
        let lines =  readLines ()
        (*
        let lines =  ["""<div class="buttons level-right">""";
                        """<button class="button hn-cancel">Cancel</button>""";
                        """<button class="button hn-submit">Submit</button>""";
                        """</div>""";
                        """<div class="hn-multi-sel box" hn-value="1,2">Hello</div>"""; """<div class="hn-multi-sel box" hn-value="5,9">World</div>"""]
        *)

        let html = lines |> String.concat "\n"

        launchBrowser html

        0 // return an integer exit code
