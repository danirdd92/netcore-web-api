namespace Entities.RequestFeatures
{
    public abstract class RequestParameters
    {
        #region Paging
        const int MAX_PAGE_SIZE = 50;

        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
            }
        }
        public int PageNumber { get; set; } = 1;
        #endregion

        #region Sorting
        public string OrderBy { get; set; }
        #endregion

    }
}
