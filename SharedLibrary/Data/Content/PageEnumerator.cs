using System.Collections;

namespace SharedLibrary.Data.Content;

internal class PageEnumerator : IEnumerator<Page>
{
    private readonly Page[] _pages;
    private int CurrentIndex { get; set; } = -1;
    private PageEnumerator? _childEnumerator;

    public PageEnumerator(IEnumerable<Page> pages)
    {
        _pages = pages.ToArray();
    }
    
    public bool MoveNext()
    {
        if(_childEnumerator != null && _childEnumerator.MoveNext())
        {
            return true;
        }
        
        
        if (CurrentIndex < _pages.Length - 1)
        {
            CurrentIndex++;
            if (_pages[CurrentIndex].Children.Any())
            {
                _childEnumerator = _pages[CurrentIndex].Children.GetEnumerator() as PageEnumerator;
            }
            else
            {
                _childEnumerator?.Dispose();
                _childEnumerator = null; // if the childEnumerator is done, _childEnumerator = null.
            }
                        
            return true; 
        }
        
        return false;
    }

    public void Reset()
    {
        CurrentIndex = -1;
        _childEnumerator = null;
    }

    public Page Current => _childEnumerator is { CurrentIndex: > -1 } ? _childEnumerator.Current : _pages[CurrentIndex];

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        _childEnumerator?.Dispose();
    }
}