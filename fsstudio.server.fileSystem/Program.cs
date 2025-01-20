//Program.cs
using System.Diagnostics;
using System.Runtime.InteropServices;
using FsStudio.Server.FileSystem.Exec;
using FuncScript;
using FuncScript.Model;

namespace FsStudio.Server.FileSystem;
public static class Program
{
    public static void Main(string[] args)
    {
        InitFuncScript();
        if (IsCommandLineMode(args))
        {
            RunCommandLineMode(args);
            return;
        }

        var options = new WebApplicationOptions
        {
            Args = args,
            ApplicationName = "FsStudio.Server.FileSystem",
            ContentRootPath = AppContext.BaseDirectory,
            WebRootPath =  "wwwroot"
        };

        var builder = WebApplication.CreateBuilder(options);
        var env = builder.Environment;
        if (env.IsDevelopment())
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
        }

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });
        builder.Services.AddSingleton<RemoteLogger>();
        builder.Services.AddSingleton<SessionManager>();
        var app = builder.Build();
        if (env.IsDevelopment())
        {
            app.UseCors("AllowSpecificOrigin");
        }
        app.MapControllers();
        
        app.UseWebSockets();
        app.UseMiddleware<WebSocketMiddleware>();
        
        var defaultFileOptions = new DefaultFilesOptions();
        defaultFileOptions.DefaultFileNames.Clear(); 
        defaultFileOptions.DefaultFileNames.Add("index.html"); 
        app.UseDefaultFiles(defaultFileOptions);
        app.UseStaticFiles(); 
        
        if (env.IsEnvironment("Desktop"))
        {
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                try
                {
                    string url = "http://localhost:5091";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to launch browser: {ex.Message}");
                }
            });
        }
        app.Run();
    }

    private static void InitFuncScript()
    {
        DefaultFsDataProvider.LoadFromAssembly(typeof(FuncScript.Openai.ChatGptFunction).Assembly);
        DefaultFsDataProvider.LoadFromAssembly(typeof(FuncScript.Sql.Core.PgSqlFunction).Assembly);
    }

    static bool IsCommandLineMode(string[] args)
    {
        foreach (var arg in args)
        {
            if (arg.StartsWith("--project:", StringComparison.OrdinalIgnoreCase) ||
                arg.StartsWith("--file:", StringComparison.OrdinalIgnoreCase) ||
                arg.StartsWith("--exec-node:", StringComparison.OrdinalIgnoreCase) ||
                arg.StartsWith("--set-node-text:", StringComparison.OrdinalIgnoreCase) ||
                arg.StartsWith("--set-node-expression:", StringComparison.OrdinalIgnoreCase) ||
                arg.StartsWith("--input:", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    static void RunCommandLineMode(string[] args)
    {
        string? projectPath = null;
        string? fileToExecute = null;
        string? nodeToEvaluate = null;
        string? inputFile = null;
        string? outputFile = null;
        var nodeTextCommands = new List<string>();
        var nodeExpressionCommands = new List<string>();
        foreach (var arg in args)
        {
            if (arg.StartsWith("--project:", StringComparison.OrdinalIgnoreCase))
            {
                projectPath = arg.Substring("--project:".Length);
            }
            else if (arg.StartsWith("--file:", StringComparison.OrdinalIgnoreCase))
            {
                fileToExecute = arg.Substring("--file:".Length);
            }
            else if (arg.StartsWith("--exec-node:", StringComparison.OrdinalIgnoreCase))
            {
                nodeToEvaluate = arg.Substring("--exec-node:".Length);
            }
            else if (arg.StartsWith("--input:", StringComparison.OrdinalIgnoreCase))
            {
                inputFile = arg.Substring("--input:".Length);
            }
            else if (arg.StartsWith("--output:", StringComparison.OrdinalIgnoreCase))
            {
                outputFile = arg.Substring("--output:".Length);
            }
            else if (arg.StartsWith("--set-node-text:", StringComparison.OrdinalIgnoreCase))
            {
                nodeTextCommands.Add(arg.Substring("--set-node-text:".Length));
            }
            else if (arg.StartsWith("--set-node-expression:", StringComparison.OrdinalIgnoreCase))
            {
                nodeExpressionCommands.Add(arg.Substring("--set-node-expression:".Length));
            }
        }

        var sessionManager = new SessionManager(null);

        if (inputFile != null)
        {
            try
            {
                string content = File.ReadAllText(inputFile);
                var result = Helpers.Evaluate(content);
                var json = Helpers.FormatToJson(result);

                if (outputFile != null)
                {
                    File.WriteAllText(outputFile, json);
                }
                else
                {
                    Console.WriteLine(json);
                }
                return; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing input file: {ex.Message}");
                Environment.ExitCode = 1;
                return;
            }
        }

        // Project validation only if not processing input file
        if (projectPath == null)
        {
            Console.WriteLine("Missing --project:<root folder>");
            Environment.ExitCode = 1;
            return;
        }

        sessionManager.SetRootFolder(projectPath);

        if (fileToExecute == null)
        {
            Console.WriteLine("Missing --file:<the file to execute>");
            Environment.ExitCode = 1;
            return;
        }

        if (nodeToEvaluate == null)
        {
            Console.WriteLine("Missing --exec-node:<node path to evaluate>");
            Environment.ExitCode = 1;
            return;
        }

        var session = sessionManager.CreateOrGetSession(fileToExecute,false);

        foreach (var cmd in nodeTextCommands)
        {
            var idx1 = cmd.IndexOf(':');
            if (idx1 < 0) continue;
            var nodePath = cmd.Substring(0, idx1);
            var content = cmd.Substring(idx1 + 1).Trim().Trim('"');
            session.ChangeExpressionType(nodePath, ExpressionType.ClearText);
            session.UpdateExpression(nodePath, content);
        }

        foreach (var cmd in nodeExpressionCommands)
        {
            var idx1 = cmd.IndexOf(':');
            if (idx1 < 0) continue;
            var nodePath = cmd.Substring(0, idx1);
            var content = cmd.Substring(idx1 + 1).Trim().Trim('"');
            session.ChangeExpressionType(nodePath, ExpressionType.FuncScript);
            session.UpdateExpression(nodePath, content);
        }

        try
        {
            var res = session.EvaluateNode(nodeToEvaluate);
            var json=Helpers.FormatToJson(res);
            if (res is FsError error)
            {
                Console.WriteLine(json);
                Environment.ExitCode = 1;
            }
            else
            {
                
                Console.WriteLine("----start-output----");
                Console.WriteLine(json);
                Console.WriteLine("----end-output----");
            }
        }
        catch (Exception ex)
        {
            Exception? inner = ex;
            while (inner!=null)
            {
                Console.WriteLine( inner.Message+"\n"+inner.StackTrace);
                inner = inner.InnerException;
            }
            
            Environment.ExitCode = 1;
        }
    }
}