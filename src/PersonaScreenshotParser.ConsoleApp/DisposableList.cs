namespace PersonaScreenshotParser.ConsoleApp;

public sealed class DisposableList<TDisposableElement> : List<TDisposableElement>, IDisposable
    where TDisposableElement : IDisposable
{
    private bool _isDisposed = false;
    
    public void Dispose()
    {
        if (!_isDisposed)
        {
            var exceptionList = new List<Exception>();
            
            foreach (var element in this)
            {
                try
                {
                    element?.Dispose();
                }
                catch(Exception e)
                {
                    exceptionList.Add(e);
                }
            }

            switch (exceptionList.Count)
            {
                case 0:
                    break;
                case 1:
                    throw exceptionList.First();
                default:
                    throw new AggregateException(exceptionList);
            }
        }

        _isDisposed = true;
    }
}