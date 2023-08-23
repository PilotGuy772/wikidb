using System.Collections;

namespace SharedLibrary.Data.Content;

internal class PageEnumerator : IEnumerator<Page>
{
    private Page[] _pages;
    private int _currentIndex = -1;
    private IEnumerator<Page>? _childEnumerator;

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
        
        if (_currentIndex < _pages.Length - 1)
        {
            _currentIndex++;
            _childEnumerator = _pages[_currentIndex].Children.GetEnumerator();
            return _childEnumerator.MoveNext();
        }

        return false;
    }

    public void Reset()
    {
        _currentIndex = -1;
        _childEnumerator = null;
    }

    public Page Current => _childEnumerator?.Current ?? _pages[_currentIndex];

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        _childEnumerator?.Dispose();
    }
}