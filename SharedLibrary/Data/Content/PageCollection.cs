using System.Collections;

namespace SharedLibrary.Data.Content;

public class PageCollection : IEnumerable<Page>
{
    private readonly IEnumerable<Page> _pages;
    public int Count => _pages.Count();

    public PageCollection(IEnumerable<Page> pages)
    {
        _pages = pages;
    }

    public Page First()
    {
        return _pages.First();
    }
    
    public IEnumerator<Page> GetEnumerator()
    {
        return new PageEnumerator(_pages);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
}