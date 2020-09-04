namespace Entities.RequestFeatures
{
    public class EmployeeParameters : RequestParameters
    {
        public EmployeeParameters()
        {
            OrderBy = "name"; // default sort
        }

        #region Filtering
        public uint MinAge { get; set; }
        public uint MaxAge { get; set; } = int.MaxValue;

        public bool ValidAgeRange => MaxAge > MinAge;
        #endregion

        #region Searching
        public string SearchTerm { get; set; }

        #endregion

    }
}
