static class Programm {

    public static T GetMax<T>(this IEnumerable<T> e, Func<T, float> getParam) where T : class {
        var max = e.FirstOrDefault();
        foreach (var item in e) {
            var maxValue = getParam?.Invoke(max);
            var currentValue = getParam?.Invoke(item);
            if (maxValue < currentValue)
                max = item;
        }
        return max;
    }
    static List<Test> items = new List<Test>() {
                new Test(){Id = 1, Value1 ="a", Value2 = 1},
                new Test(){Id = 2, Value1 ="b", Value2 = 2},
                new Test(){Id = 3, Value1 ="c", Value2 = 3}
            };
    public static void Main(string[] args) {
        var max = items.GetMax(s => s.Value2);
        var name = typeof(Test);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"max: {max.Value2} (Id {max.Id})\n");
        Console.ForegroundColor = ConsoleColor.White;

        var fileCount = 0;
        var fileSearcher = new FileSearcher(@"C:\");
        fileSearcher.Found += (s, e) => Console.WriteLine(((FileArgs)e).Name);
        fileSearcher.Start();

        Thread.Sleep(500);

        fileSearcher.Stop();
        Console.ReadKey();
    }
}

public class Test {
    public int Id { get; set; }
    public string Value1 { get; set; }
    public float Value2 { get; set; }
}


public class FileSearcher {
    public event EventHandler Found;

    private string startDirectory;
    private CancellationTokenSource source;
    private Task worker;

    public FileSearcher(string startDirectory) {
        this.startDirectory = startDirectory;
    }

    public Task Start() {
        if (worker == null || worker?.Status != TaskStatus.Running) {
            source = new CancellationTokenSource();
            var token = source.Token;
            worker = Task.Run(() => { Scann(startDirectory); }, token);
        }
        return worker;
    }

    public void Stop() => source?.Cancel();

    private void Scann(string path) {
        Thread.Sleep(200);
        if (Directory.Exists(path)) {
            try {
                var fileNames = Directory.GetFiles(path);
                foreach (var name in fileNames)
                    Found?.Invoke(this, new FileArgs() { Name = name });

                var dir = Directory.GetDirectories(path);
                foreach (var d in dir)
                    Scann(d);
            } catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}

public class FileArgs : EventArgs {
    public string Name { get; set; }
}
